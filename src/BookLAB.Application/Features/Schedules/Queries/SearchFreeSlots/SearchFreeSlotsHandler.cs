using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.SearchFreeSlots
{
    public class SearchFreeSlotsHandler : IRequestHandler<SearchFreeSlotsQuery, ResultMessage<List<FreeSlotDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public SearchFreeSlotsHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ResultMessage<List<FreeSlotDto>>> Handle(SearchFreeSlotsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Schedule> schedules = _unitOfWork.Repository<Schedule>().Entities.Include(x => x.LabRoom);
            IQueryable<LabRoom> labRooms = _unitOfWork.Repository<LabRoom>().Entities.Include(x => x.Building).Where(x => x.Building.CampusId == _currentUserService.CampusId && x.IsActive == true && x.IsDeleted == false);

            if (request.BuildingId.HasValue)
            {
                schedules = schedules.Where(s => s.LabRoom.BuildingId == request.BuildingId.Value);
                labRooms = labRooms.Where(x => x.BuildingId == request.BuildingId.Value);
            }
            
            if (request.LabRoomId.HasValue)
            {
                schedules = schedules.Where(s => s.LabRoomId == request.LabRoomId.Value);
                labRooms = labRooms.Where(x => x.Id == request.LabRoomId.Value);
            }
            
            var startDate = request.StartDay.ToDateTime(new TimeOnly());
            var endDate = request.EndDay.ToDateTime(new TimeOnly());
            var startTime = request.StartTime.HasValue ? TimeOnlyUniversalUtc(request.StartTime.Value).ToTimeSpan() : TimeOnlyUniversalUtc(new TimeOnly(7, 0, 0)).ToTimeSpan();
            var endTime = request.EndTime.HasValue ? TimeOnlyUniversalUtc(request.EndTime.Value).ToTimeSpan() : TimeOnlyUniversalUtc(new TimeOnly(22, 0, 0)).ToTimeSpan();

            if (request.StartTime.HasValue && request.EndTime.HasValue)
            {
                startDate = request.StartDay.ToDateTime(request.StartTime.Value).ToUniversalTime();
                endDate = request.EndDay.ToDateTime(request.EndTime.Value).ToUniversalTime();
                schedules = schedules.Where(s => (s.StartTime > startDate && s.StartTime < endDate) || (s.EndTime > startDate && s.EndTime < endDate));
            } else
            {                //startDate = new DateTime(7, 0, 0).ToUniversalTime();
                //schedules = schedules.Where(s => s.StartTime >= startDate);

                if (request.StartTime.HasValue)
                {
                    startDate = request.StartDay.ToDateTime(request.StartTime.Value).ToUniversalTime();
                    schedules = schedules.Where(s => s.StartTime >= startDate);
                }
                else
                {
                    startDate = request.StartDay.ToDateTime(new TimeOnly(7, 0, 0)).ToUniversalTime();
                    schedules = schedules.Where(s => s.StartTime >= startDate);
                }

                if (request.EndTime.HasValue)
                {
                    endDate = request.EndDay.ToDateTime(request.EndTime.Value).ToUniversalTime();
                    schedules = schedules.Where(s => s.EndTime <= endDate);
                }
                else
                {
                    endDate = request.EndDay.ToDateTime(new TimeOnly(22, 0, 0)).ToUniversalTime();
                    schedules = schedules.Where(s => s.EndTime <= endDate);
                }

            }

            var result = await schedules
                .OrderBy(x => x.LabRoomId)
                .ThenBy(x => x.StartTime)
                .ThenBy(x => x.EndTime)
                .ToListAsync(cancellationToken);

            var freeSlots = new List<FreeSlotDto>();

            var listLabRooms = await labRooms.ToListAsync();
            List<TempDictionary> tempLists = new List<TempDictionary>();
            
            if (listLabRooms.Count == 0)
                return new ResultMessage<List<FreeSlotDto>>
                {
                    Success = false,
                    Message = "There is 0 room exist"
                };

            if (result.Count <= 0)
            {
                foreach (var room in listLabRooms)
                {
                    for (DateOnly date = request.StartDay; date <= request.EndDay; date = date.AddDays(1))
                    {
                        freeSlots.Add(new FreeSlotDto
                        {
                            BuildingId = request.BuildingId,
                            RoomId = room != null ? room.Id : 0,
                            RoomName = room != null ? room.RoomName : "Room 0",
                            StartDate = date,
                            EndDate = date,
                            StartTime = request.StartTime ?? TimeOnly.FromTimeSpan(new TimeSpan(7, 0, 0)),
                            EndTime = request.EndTime ?? TimeOnly.FromTimeSpan(new TimeSpan(22, 0, 0))
                        });
                    }
                }
                

                return new ResultMessage<List<FreeSlotDto>>
                {
                    Success = true,
                    Message = "No schedules found, all slots are free.",
                    Data = freeSlots
                };
            }
            
            if (result.Count == 1)
            {
                foreach (var room in listLabRooms)
                {
                    for (DateOnly date = request.StartDay; date <= request.EndDay; date = date.AddDays(1))
                    {
                        if (DateOnly.FromDateTime(result[0].StartTime.Date) != date)
                        {
                            freeSlots.Add(new FreeSlotDto
                            {
                                BuildingId = request.BuildingId,
                                RoomId = room != null ? room.Id : 0,
                                RoomName = room != null ? room.RoomName : "Room 0",
                                StartDate = date,
                                EndDate = date,
                                StartTime = request.StartTime ?? TimeOnly.FromTimeSpan(new TimeSpan(7, 0, 0)),
                                EndTime = request.EndTime ?? TimeOnly.FromTimeSpan(new TimeSpan(22, 0, 0))
                            });

                            continue;
                        }

                        // Nếu từ 6h sáng đến lúc bắt đầu slot i+1 mà cách nhau trên 
                        // thời gian người dùng yêu cầu hoặc 1 tiếng 
                        if (result[0].StartTime.TimeOfDay - startTime >=
                            (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                            freeSlots.Add(new FreeSlotDto
                            {
                                BuildingId = result[0].LabRoom.BuildingId,
                                RoomId = result[0].LabRoomId,
                                RoomName = result[0].LabRoom.RoomName,
                                StartDate = DateOnly.FromDateTime(result[0].StartTime.ToLocalTime().Date),
                                EndDate = DateOnly.FromDateTime(result[0].StartTime.ToLocalTime().Date),
                                StartTime = new TimeOnly(7, 0, 0),
                                EndTime = TimeOnly.FromTimeSpan(result[0].StartTime.ToLocalTime().TimeOfDay)
                            });

                        // Nếu từ lúc kết thúc slot i đến 22h mà cách nhau trên 
                        // thời gian người dùng yêu cầu hoặc 1 tiếng 
                        if (new TimeSpan(22, 0, 0) - result[0].EndTime.TimeOfDay >=
                            (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                            freeSlots.Add(new FreeSlotDto
                            {
                                BuildingId = result[0].LabRoom.BuildingId,
                                RoomId = result[0].LabRoomId,
                                RoomName = result[0].LabRoom.RoomName,
                                StartDate = DateOnly.FromDateTime(result[0].EndTime.ToLocalTime().Date),
                                EndDate = DateOnly.FromDateTime(result[0].EndTime.ToLocalTime().Date),
                                StartTime = TimeOnly.FromTimeSpan(result[0].EndTime.ToLocalTime().TimeOfDay),
                                EndTime = new TimeOnly(22, 0, 0)
                            });
                    }
                }
                
                return new ResultMessage<List<FreeSlotDto>>
                {
                    Success = true,
                    Message = "Free slots retrieved successfully",
                    Data = freeSlots
                };
            }

            var currRoom = result[0].LabRoomId;
            bool roomChangeBefore = true;

            // Đi dò từng schedule đã tồn tại
            for (int i = 0; i <= result.Count - 2; i++)
            {
                // Nếu schedule tiếp đó không giống phòng lab schedule hiện tại thì vào đây
                if (result[i + 1].LabRoomId != currRoom)
                {
                    if (roomChangeBefore)
                    {
                        if (result[i].StartTime.TimeOfDay - startTime >=
                        (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                            freeSlots.Add(new FreeSlotDto
                            {
                                BuildingId = result[i].LabRoom.BuildingId,
                                RoomId = result[i].LabRoomId,
                                RoomName = result[i].LabRoom.RoomName,
                                StartDate = DateOnly.FromDateTime(result[i].StartTime.ToLocalTime().Date),
                                EndDate = DateOnly.FromDateTime(result[i].StartTime.ToLocalTime().Date),
                                StartTime = request.StartTime.Value,
                                EndTime = TimeOnly.FromTimeSpan(result[i].StartTime.ToLocalTime().TimeOfDay)
                            });
                    }

                    if (endTime - result[i].EndTime.TimeOfDay >=
                            (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                        freeSlots.Add(new FreeSlotDto
                        {
                            BuildingId = result[i].LabRoom.BuildingId,
                            RoomId = result[i].LabRoomId,
                            RoomName = result[i].LabRoom.RoomName,
                            StartDate = DateOnly.FromDateTime(result[i].StartTime.ToLocalTime().Date),
                            EndDate = DateOnly.FromDateTime(result[i].EndTime.ToLocalTime().Date),
                            StartTime = TimeOnly.FromTimeSpan(result[i].EndTime.ToLocalTime().TimeOfDay),
                            EndTime = request.EndTime.Value
                        });

                    currRoom = result[i].LabRoomId;
                    roomChangeBefore = true;

                    if (!tempLists.Any(x => x.LabRoom == result[i].LabRoom && x.Date == DateOnly.FromDateTime(result[i].StartTime.Date)))
                        tempLists.Add(new TempDictionary { LabRoom = result[i].LabRoom, Date = DateOnly.FromDateTime(result[i].StartTime.Date) });
                    continue;
                }

                // đây là slot đầu tiên được kiểm tra trong phòng
                if (roomChangeBefore)
                {
                    if (result[i].StartTime.TimeOfDay - startTime >=
                    (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                    {
                        freeSlots.Add(new FreeSlotDto
                        {
                            BuildingId = result[i].LabRoom.BuildingId,
                            RoomId = result[i].LabRoomId,
                            RoomName = result[i].LabRoom.RoomName,
                            StartDate = DateOnly.FromDateTime(result[i].StartTime.ToLocalTime().Date),
                            EndDate = DateOnly.FromDateTime(result[i].EndTime.ToLocalTime().Date),
                            StartTime = request.StartTime.Value,
                            EndTime = TimeOnly.FromTimeSpan(result[i].StartTime.ToLocalTime().TimeOfDay)
                        });
                    }

                    if (!tempLists.Any(x => x.LabRoom == result[i].LabRoom && x.Date == DateOnly.FromDateTime(result[i].StartTime.Date)))
                        tempLists.Add(new TempDictionary { LabRoom = result[i].LabRoom, Date = DateOnly.FromDateTime(result[i].StartTime.Date) });
                }

                // Nếu từ lúc kết thúc slot i đến lúc bắt đầu slot i+1 mà chỉ cách nhau trên 
                // thời gian người dùng yêu cầu hoặc 1 tiếng 
                if (result[i + 1].StartTime.Date == result[i].EndTime.Date &&
                    result[i + 1].StartTime - result[i].EndTime >=
                    (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                {
                    freeSlots.Add(new FreeSlotDto
                    {
                        BuildingId = result[i].LabRoom.BuildingId,
                        RoomId = result[i].LabRoomId,
                        RoomName = result[i].LabRoom.RoomName,
                        StartDate = DateOnly.FromDateTime(result[i].EndTime.ToLocalTime().Date),
                        EndDate = DateOnly.FromDateTime(result[i + 1].StartTime.ToLocalTime().Date),
                        StartTime = TimeOnly.FromTimeSpan(result[i].EndTime.ToLocalTime().TimeOfDay),
                        EndTime = TimeOnly.FromTimeSpan(result[i + 1].StartTime.ToLocalTime().TimeOfDay)
                    });

                    if (!tempLists.Any(x => x.LabRoom == result[i].LabRoom && x.Date == DateOnly.FromDateTime(result[i].StartTime.Date)))
                        tempLists.Add(new TempDictionary { LabRoom = result[i].LabRoom, Date = DateOnly.FromDateTime(result[i].StartTime.Date) });
                }
                // Nếu từ lúc kết thúc slot i đến lúc bắt đầu slot i+1 mà đi qua ngày
                else if (result[i + 1].StartTime.Date > result[i].EndTime.Date)
                {

                    // Nếu từ lúc kết thúc slot i đến 22h mà cách nhau trên 
                    // thời gian người dùng yêu cầu hoặc 1 tiếng 
                    if (endTime - result[i].EndTime.TimeOfDay >=
                        (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                    {
                        freeSlots.Add(new FreeSlotDto
                        {
                            BuildingId = result[i].LabRoom.BuildingId,
                            RoomId = result[i].LabRoomId,
                            RoomName = result[i].LabRoom.RoomName,
                            StartDate = DateOnly.FromDateTime(result[i].EndTime.ToLocalTime().Date),
                            EndDate = DateOnly.FromDateTime(result[i].EndTime.ToLocalTime().Date),
                            StartTime = TimeOnly.FromTimeSpan(result[i].EndTime.ToLocalTime().TimeOfDay),
                            EndTime = request.EndTime.Value,
                        });

                        if (!tempLists.Any(x => x.LabRoom == result[i].LabRoom && x.Date == DateOnly.FromDateTime(result[i].EndTime.Date)))
                            tempLists.Add(new TempDictionary { LabRoom = result[i].LabRoom, Date = DateOnly.FromDateTime(result[i].EndTime.Date) });
                    }
                        

                    // Nếu từ startTime đến lúc bắt đầu slot i+1 mà cách nhau trên 
                    // thời gian người dùng yêu cầu hoặc 1 tiếng 
                    if (result[i + 1].StartTime.TimeOfDay - startTime >=
                        (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                    {
                        freeSlots.Add(new FreeSlotDto
                        {
                            BuildingId = result[i + 1].LabRoom.BuildingId,
                            RoomId = result[i + 1].LabRoomId,
                            RoomName = result[i + 1].LabRoom.RoomName,
                            StartDate = DateOnly.FromDateTime(result[i + 1].StartTime.ToLocalTime().Date),
                            EndDate = DateOnly.FromDateTime(result[i + 1].StartTime.ToLocalTime().Date),
                            StartTime = request.StartTime.Value,
                            EndTime = TimeOnly.FromTimeSpan(result[i + 1].StartTime.ToLocalTime().TimeOfDay)
                        });

                        if (!tempLists.Any(x => x.LabRoom == result[i].LabRoom && x.Date == DateOnly.FromDateTime(result[i + 1].StartTime.Date)))
                            tempLists.Add(new TempDictionary { LabRoom = result[i].LabRoom, Date = DateOnly.FromDateTime(result[i + 1].StartTime.Date) });
                    }
                }
                roomChangeBefore = false;
            }

            if (roomChangeBefore)
            {
                // Nếu từ 6h sáng đến lúc bắt đầu slot i+1 mà cách nhau trên 
                // thời gian người dùng yêu cầu hoặc 1 tiếng 
                if (result.Last().StartTime.TimeOfDay - startTime >=
                    (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                {
                    freeSlots.Add(new FreeSlotDto
                    {
                        BuildingId = result.Last().LabRoom.BuildingId,
                        RoomId = result.Last().LabRoomId,
                        RoomName = result.Last().LabRoom.RoomName,
                        StartDate = DateOnly.FromDateTime(result.Last().StartTime.ToLocalTime().Date),
                        EndDate = DateOnly.FromDateTime(result.Last().StartTime.ToLocalTime().Date),
                        StartTime = request.StartTime.Value,
                        EndTime = TimeOnly.FromTimeSpan(result.Last().StartTime.ToLocalTime().TimeOfDay)
                    });

                    if (!tempLists.Any(x => x.LabRoom == result.Last().LabRoom && x.Date == DateOnly.FromDateTime(result.Last().StartTime.Date)))
                        tempLists.Add(new TempDictionary { LabRoom = result.Last().LabRoom, Date = DateOnly.FromDateTime(result.Last().StartTime.Date) });
                }
                    
            }
            // Nếu từ lúc kết thúc slot i đến 22h mà cách nhau trên 
            // thời gian người dùng yêu cầu hoặc 1 tiếng 
            if (endTime - result.Last().EndTime.TimeOfDay >=
                (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
            {
                freeSlots.Add(new FreeSlotDto
                {
                    BuildingId = result.Last().LabRoom.BuildingId,
                    RoomId = result.Last().LabRoomId,
                    RoomName = result.Last().LabRoom.RoomName,
                    StartDate = DateOnly.FromDateTime(result.Last().EndTime.ToLocalTime().Date),
                    EndDate = DateOnly.FromDateTime(result.Last().EndTime.ToLocalTime().Date),
                    StartTime = TimeOnly.FromTimeSpan(result.Last().EndTime.ToLocalTime().TimeOfDay),
                    EndTime = request.EndTime.Value
                });
                if (!tempLists.Any(x => x.LabRoom == result.Last().LabRoom && x.Date == DateOnly.FromDateTime(result.Last().StartTime.Date)))
                    tempLists.Add(new TempDictionary { LabRoom = result.Last().LabRoom, Date = DateOnly.FromDateTime(result.Last().StartTime.Date) });
            }
            
            foreach (var room in listLabRooms)
            {
                for (DateOnly date = request.StartDay; date <= request.EndDay; date = date.AddDays(1))
                {
                    if (!tempLists.Any(x => x.LabRoom == room && x.Date == date))
                    {
                        freeSlots.Add(new FreeSlotDto
                        {
                            BuildingId = request.BuildingId,
                            RoomId = room != null ? room.Id : 0,
                            RoomName = room != null ? room.RoomName : "Room 0",
                            StartDate = date,
                            EndDate = date,
                            StartTime = request.StartTime ?? TimeOnly.FromTimeSpan(new TimeSpan(7, 0, 0)),
                            EndTime = request.EndTime ?? TimeOnly.FromTimeSpan(new TimeSpan(22, 0, 0))
                        });
                    }
                }
            }

            freeSlots = freeSlots.OrderBy(x => x.StartDate).ThenBy(x => x.StartTime)
                .ThenBy(x => x.EndDate).ThenBy(x => x.EndTime)
                .ThenBy(x => x.RoomId).ToList();

            return new ResultMessage<List<FreeSlotDto>>
            {
                Success = true,
                Message = "Free slots retrieved successfully.",
                Data = freeSlots
            };
        }

        private TimeOnly TimeOnlyUniversalUtc(TimeOnly time)
        {
            // Lấy ngày hiện tại
            DateTime localDateTime = DateTime.Today.Add(time.ToTimeSpan());

            // Lấy múi giờ hệ thống (ví dụ: Việt Nam UTC+7)
            TimeZoneInfo localZone = TimeZoneInfo.Local;

            // Tạo DateTimeOffset với múi giờ hiện tại
            DateTimeOffset localOffset = new DateTimeOffset(localDateTime, localZone.GetUtcOffset(localDateTime));

            // Chuyển sang UTC
            DateTimeOffset utcTime = localOffset.ToUniversalTime();

            // Lấy lại TimeOnly từ UTC
            TimeOnly utcTimeOnly = TimeOnly.FromDateTime(utcTime.DateTime);

            return utcTimeOnly;
        }
    }

    internal class TempDictionary
    {
        public LabRoom LabRoom { get; set; }
        public DateOnly Date { get; set; }
    }
}
