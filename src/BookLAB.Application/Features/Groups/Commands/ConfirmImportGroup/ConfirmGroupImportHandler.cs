using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Groups.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Groups.Commands.ConfirmImportGroup
{
    public class ConfirmGroupImportHandler : IRequestHandler<ConfirmGroupImportCommand, ImportResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ConfirmGroupImportHandler(IUnitOfWork unitOfWork, IMediator mediator, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        public async Task<ImportResult> Handle(ConfirmGroupImportCommand request, CancellationToken ct)
        {
            // 1. Re-validate lần cuối để lấy dữ liệu đã được chuẩn hóa
            var result = new ImportValidationResult<GroupImportDto, GroupMember>();
            var data = request.Groups;

            // 1. Tải trước danh sách để tối ưu O(1) lookup
            var fileSubjectCodes = data
                .Select(d => d.SubjectCode.Trim().ToUpper())
                .Distinct()
                .ToList();
            var subjectCodes = await _unitOfWork.Repository<Subject>().Entities
                .Where(s => fileSubjectCodes.Contains(s.SubjectCode.Trim().ToUpper()))
                .Select(s => s.SubjectCode.Trim().ToUpper())
                .ToListAsync(ct);

            var groupNames = data.Select(d => d.GroupName.ToUpper()).Distinct().ToList();
            var groups = await _unitOfWork.Repository<Group>().Entities
                .Where(g => g.CampusId == request.CampusId && !g.IsDeleted && groupNames.Contains(g.GroupName)) // Chỉ check group trong cùng campus
                .ToDictionaryAsync(g => g.GroupName.ToUpper(), g => g.Id, ct);
            var existingGroupNames = groups.Keys.ToList();

            var studentCodes = data.Select(d => d.StudentCode.ToUpper()).Distinct().ToList();
            var existingUsers = await _unitOfWork.Repository<User>().Entities
                .Where(u => studentCodes.Contains(u.UserCode.ToUpper()))
                .ToDictionaryAsync(u => u.UserCode.ToUpper(), u => u.Id, ct);

            // 1. Thu thập danh sách ID để check một lần
            var allStudentIds = existingUsers.Values.ToList();
            var allGroupIds = groups.Values.ToList();
            // 2. Lấy danh sách đã tồn tại trong DB (Bulk Load)
            var existingMembersInDb = await _unitOfWork.Repository<GroupMember>().Entities
                .Where(gm => allGroupIds.Contains(gm.GroupId)
                          && allStudentIds.Contains(gm.UserId)
                          && subjectCodes.Contains(gm.SubjectCode.ToUpper()))
                .Select(gm => new { gm.GroupId, gm.UserId, gm.SubjectCode })
                .ToListAsync(ct);
            var dbSet = new HashSet<(Guid, Guid, string)>(existingMembersInDb.Select(x => (x.GroupId, x.UserId, x.SubjectCode)));
            // 3. Dùng HashSet để check trùng trong file (Thay cho .Any() lồng)
            var processedInFile = new HashSet<string>();

            for (int i = 0; i < data.Count; i++)
            {
                var row = data[i];
                var rowResult = new RowResult<GroupImportDto, GroupMember> { RowNumber = i+1, Data = row };
                var rowKey = $"{row.StudentCode}_{row.GroupName}_{row.SubjectCode}".ToUpper();
                // --- BƯỚC 1: CHECK GROUP (LỚP HỌC) ---
                if (!groups.TryGetValue(row.GroupName.ToUpper(), out var groupId))
                {
                    rowResult.Errors.Add(new RowError
                    {
                        FieldName = "GroupName",
                        Message = $"Nhóm {row.GroupName} chưa có. Hệ thống sẽ tự động tạo mới nhóm này.",
                        Severity = ErrorSeverity.Warning
                    });
                }

                // --- BƯỚC 2: CHECK SUBJECT (MÔN HỌC) ---
                if (!subjectCodes.Contains(row.SubjectCode.ToUpper()))
                {
                    rowResult.Errors.Add(new RowError
                    {
                        FieldName = "SubjectCode",
                        Message = $"Môn {row.SubjectCode} chưa có",
                        Severity = ErrorSeverity.Error
                    });
                }

                // --- BƯỚC 3: CHECK USER (SINH VIÊN) ---
                
                if (!existingUsers.TryGetValue(row.StudentCode.ToUpper(), out var studentId))
                {
                    rowResult.Errors.Add(new RowError
                    {
                        FieldName = "StudentCode",
                        Message = "Sinh viên mới, sẽ tự động tạo tài khoản.",
                        Severity = ErrorSeverity.Error
                    });
                }

                // --- BƯỚC 4: CHECK TRÙNG LẶP TRONG FILE (Bản thân các dòng đá nhau) ---
                // Ví dụ: Trong file có 2 dòng cùng (Student A, Group B, Subject C)
                var isDuplicateInFile = data.Take(i).Any(d =>
                    d.StudentCode == row.StudentCode &&
                    d.GroupName == row.GroupName &&
                    d.SubjectCode == row.SubjectCode);

                if (!processedInFile.Add(rowKey))
                {
                    rowResult.Errors.Add(new RowError
                    {
                        FieldName = "GroupName",
                        Message = "Dòng này bị trùng lặp dữ liệu với một dòng khác phía trên trong file.",
                        Severity = ErrorSeverity.Error
                    });
                }         

                //--- Check dublicated in database
                if (existingUsers.TryGetValue(row.StudentCode.ToUpper(), out var sId) && groups.TryGetValue(row.GroupName.ToUpper(), out var gId))
                {
                    if (dbSet.Contains((gId, sId, row.SubjectCode.ToUpper())))
                    {
                        rowResult.Errors.Add(new RowError
                        {
                            FieldName = "GroupName",
                            Message = "Sinh viên này đã có tên trong lớp và môn học này (Dữ liệu đã tồn tại trên hệ thống).",
                            Severity = ErrorSeverity.Warning // Để Warning nếu bạn muốn cho phép 'Update' thay vì chặn đứn
                        });
                        rowResult.Data.IsUpdated = true;

                    }
                }
                result.Rows.Add(rowResult);
            }


            if (!result.CanCommit)
            {
                return new ImportResult { Success = false };
            }

            var updatedCount = result.Rows.Count(r => r.Data.IsUpdated);
            var createdCount = result.Rows.Count - updatedCount;
            var importStat = new ImportResult
            {
                TotalProcessed = result.Rows.Count,
                Success = true,
                UpdatedCount = updatedCount,
                CreatedCount = createdCount,             
            };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var now = DateTimeOffset.UtcNow;
                var currentUserId = _currentUserService.UserId;

                var newGroupNames = groupNames.Where(g => !existingGroupNames.Any(s => s.ToUpper() == g.ToUpper())).ToList(); 
                var newGroupList = new List<Group>();
                var newGroupDictionary = new Dictionary<string, Guid>();
                var newGroupMembers = new List<GroupMember>();
                if(newGroupNames.Count > 0)
                {
                    foreach( var newGroupName in newGroupNames)
                    {
                        var newGroup = new Group
                        {
                            GroupName = newGroupName,
                            CampusId = request.CampusId,
                            OwnerId = _currentUserService.UserId ?? Guid.Empty,
                            CreatedAt = now,
                            CreatedBy = _currentUserService.UserId

                        };
                        newGroupList.Add(newGroup);
                    }
                    await _unitOfWork.Repository<Group>().AddRangeAsync(newGroupList);
                    newGroupDictionary = newGroupList.ToDictionary(g => g.GroupName, g => g.Id);
                }

                foreach (var existingGroup in groups)
                {
                    newGroupDictionary[existingGroup.Key] = existingGroup.Value;
                }

                foreach (var row in result.Rows)
                {
                    if(row.Data.IsUpdated)
                    {

                    }
                    else
                    {
                        if (newGroupDictionary.TryGetValue(row.Data.GroupName.ToUpper(), out var groupId)
                            && existingUsers.TryGetValue(row.Data.StudentCode.ToUpper(), out var studentId))
                        {
                            var newGroupMember = new GroupMember
                            {
                                GroupId = groupId,
                                UserId = studentId,
                                SubjectCode = row.Data.SubjectCode.ToUpper(),
                            };
                            newGroupMembers.Add(newGroupMember);
                        }
                    }
                }

                // Batch insert cho các thành viên mới
                if (newGroupMembers.Any())
                {
                    await _unitOfWork.Repository<GroupMember>().AddRangeAsync(newGroupMembers);
                }

                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync();

                return importStat;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
