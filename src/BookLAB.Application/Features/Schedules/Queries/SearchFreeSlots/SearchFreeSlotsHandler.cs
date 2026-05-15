using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.SearchFreeSlots
{
    public class SearchFreeSlotsHandler : IRequestHandler<SearchFreeSlotsQuery, ResultMessage<List<FreeSlotDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchFreeSlotsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultMessage<List<FreeSlotDto>>> Handle(SearchFreeSlotsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Schedule> schedules = _unitOfWork.Repository<Schedule>().Entities.Include(x => x.LabRoom);

            if (request.BuildingId.HasValue)
                schedules = schedules.Where(s => s.LabRoom.BuildingId == request.BuildingId.Value);

            if (request.LabRoomId.HasValue)
                schedules = schedules.Where(s => s.LabRoomId == request.LabRoomId.Value);

            var startDate = request.StartDay.ToDateTime(new TimeOnly());
            var endDate = request.EndDay.ToDateTime(new TimeOnly());

            if (request.StartTime.HasValue && request.EndTime.HasValue)
            {
                startDate = request.StartDay.ToDateTime(request.StartTime.Value).ToUniversalTime();
                endDate = request.EndDay.ToDateTime(request.EndTime.Value).ToUniversalTime();
                schedules = schedules.Where(s => (s.StartTime > startDate && s.StartTime < endDate) || (s.EndTime > startDate && s.EndTime < endDate));
            } else
            {
                //startDate = new DateTime(7, 0, 0).ToUniversalTime();
                //schedules = schedules.Where(s => s.StartTime >= startDate);

                if (request.StartTime.HasValue)
                {
                    startDate = request.StartDay.ToDateTime(request.StartTime.Value).ToUniversalTime();
                    schedules = schedules.Where(s => s.StartTime >= startDate);
                }
                else
                {
                    startDate = new DateTime(7, 0, 0).ToUniversalTime();
                    schedules = schedules.Where(s => s.StartTime >= startDate);
                }

                if (request.EndTime.HasValue)
                {
                    endDate = request.EndDay.ToDateTime(request.EndTime.Value).ToUniversalTime();
                    schedules = schedules.Where(s => s.EndTime <= endDate);
                }
                else
                {
                    endDate = new DateTime(7, 0, 0).ToUniversalTime();
                    schedules = schedules.Where(s => s.EndTime <= endDate);
                }

            }

            var result = await schedules
                .OrderBy(x => x.LabRoomId)
                .ThenBy(x => x.StartTime)
                .ThenBy(x => x.EndTime)
                .ToListAsync(cancellationToken);

            var freeSlots = new List<FreeSlotDto>();

            if (result.Count <= 0)
            {
                LabRoom room = new LabRoom();
                for (DateOnly date = request.StartDay; date <= request.EndDay; date = date.AddDays(1))
                {
                    room = request.LabRoomId.HasValue ? await _unitOfWork.Repository<LabRoom>().GetByIdAsync(request.LabRoomId) : null;

                    freeSlots.Add(new FreeSlotDto
                    {
                        BuildingId = request.BuildingId,
                        RoomId = room == null ? room.Id : 0,
                        RoomName = room == null ? room.RoomName : "Room 0",
                        StartDate = request.StartDay,
                        EndDate = request.EndDay,
                        StartTime = request.StartTime ?? TimeOnly.FromTimeSpan(new TimeSpan(7, 0, 0)),
                        EndTime = request.EndTime ?? TimeOnly.FromTimeSpan(new TimeSpan(22, 0, 0))
                    });
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
                // Nếu từ 6h sáng đến lúc bắt đầu slot i+1 mà cách nhau trên 
                // thời gian người dùng yêu cầu hoặc 1 tiếng 
                if (result[0].StartTime.TimeOfDay - new TimeSpan(7, 0, 0) + DateTimeOffset.Now.Offset >=
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
                if (new TimeSpan(22, 0, 0) - result[0].EndTime.TimeOfDay + DateTimeOffset.Now.Offset >=
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

                //freeSlots.Add(new FreeSlotDto
                //{
                //    BuildingId = request.BuildingId,
                //    RoomId = result[0].LabRoomId,
                //    StartDate = DateOnly.FromDateTime(result[0].StartTime.Date),
                //    EndDate = DateOnly.FromDateTime(result[0].EndTime.Date),
                //    StartTime = TimeOnly.FromTimeSpan(result[0].StartTime.TimeOfDay),
                //    EndTime = TimeOnly.FromTimeSpan(result[0].EndTime.TimeOfDay)
                //});
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
                // Nếu schedule hiện tại không giống phòng lab thì bỏ qua
                if (result[i + 1].LabRoomId != currRoom)
                {
                    if (new TimeSpan(22, 0, 0) - result[i].EndTime.TimeOfDay + DateTimeOffset.Now.Offset >=
                        (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                        freeSlots.Add(new FreeSlotDto
                        {
                            BuildingId = result[i].LabRoom.BuildingId,
                            RoomId = result[i].LabRoomId,
                            RoomName = result[i].LabRoom.RoomName,
                            StartDate = DateOnly.FromDateTime(result[i].StartTime.ToLocalTime().Date),
                            EndDate = DateOnly.FromDateTime(result[i].EndTime.ToLocalTime().Date),
                            StartTime = TimeOnly.FromTimeSpan(result[i].EndTime.ToLocalTime().TimeOfDay),
                            EndTime = new TimeOnly(22, 0, 0)
                        });

                    currRoom = result[i].LabRoomId;
                    roomChangeBefore = true;
                    continue;
                }

                // Nếu từ lúc kết thúc slot i đến lúc bắt đầu slot i+1 mà chỉ cách nhau trên 
                // thời gian người dùng yêu cầu hoặc 1 tiếng 
                if (result[i + 1].StartTime.TimeOfDay - result[i].EndTime.TimeOfDay + DateTimeOffset.Now.Offset >=
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
                }
                // Nếu từ lúc kết thúc slot i đến lúc bắt đầu slot i+1 mà đi qua ngày
                else
                {
                    // Nếu từ 6h sáng đến lúc bắt đầu slot i+1 mà cách nhau trên 
                    // thời gian người dùng yêu cầu hoặc 1 tiếng 
                    if (result[i + 1].StartTime.TimeOfDay - new TimeSpan(7, 0, 0) + DateTimeOffset.Now.Offset >=
                        (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                        freeSlots.Add(new FreeSlotDto
                        {
                            BuildingId = result[i + 1].LabRoom.BuildingId,
                            RoomId = result[i + 1].LabRoomId,
                            RoomName = result[i + 1].LabRoom.RoomName,
                            StartDate = DateOnly.FromDateTime(result[i + 1].StartTime.ToLocalTime().Date),
                            EndDate = DateOnly.FromDateTime(result[i + 1].StartTime.ToLocalTime().Date),
                            StartTime = new TimeOnly(7, 0, 0),
                            EndTime = TimeOnly.FromTimeSpan(result[i + 1].StartTime.ToLocalTime().TimeOfDay)
                        });

                    // Nếu từ lúc kết thúc slot i đến 22h mà cách nhau trên 
                    // thời gian người dùng yêu cầu hoặc 1 tiếng 
                    if (new TimeSpan(22, 0, 0) - result[i].EndTime.TimeOfDay + DateTimeOffset.Now.Offset >=
                        (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                        freeSlots.Add(new FreeSlotDto
                        {
                            BuildingId = result[i].LabRoom.BuildingId,
                            RoomId = result[i].LabRoomId,
                            RoomName = result[i].LabRoom.RoomName,
                            StartDate = DateOnly.FromDateTime(result[i].EndTime.ToLocalTime().Date),
                            EndDate = DateOnly.FromDateTime(result[i].EndTime.ToLocalTime().Date),
                            StartTime = TimeOnly.FromTimeSpan(result[i].EndTime.ToLocalTime().TimeOfDay),
                            EndTime = new TimeOnly(22, 0, 0)
                        });
                }
                roomChangeBefore = false;
            }

            if (roomChangeBefore)
            {
                // Nếu từ 6h sáng đến lúc bắt đầu slot i+1 mà cách nhau trên 
                // thời gian người dùng yêu cầu hoặc 1 tiếng 
                if (result.Last().StartTime.TimeOfDay - new TimeSpan(7, 0, 0) + DateTimeOffset.Now.Offset >=
                    (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                    freeSlots.Add(new FreeSlotDto
                    {
                        BuildingId = result.Last().LabRoom.BuildingId,
                        RoomId = result.Last().LabRoomId,
                        RoomName = result.Last().LabRoom.RoomName,
                        StartDate = DateOnly.FromDateTime(result.Last().StartTime.ToLocalTime().Date),
                        EndDate = DateOnly.FromDateTime(result.Last().StartTime.ToLocalTime().Date),
                        StartTime = new TimeOnly(7, 0, 0),
                        EndTime = TimeOnly.FromTimeSpan(result.Last().StartTime.ToLocalTime().TimeOfDay)
                    });

                // Nếu từ lúc kết thúc slot i đến 22h mà cách nhau trên 
                // thời gian người dùng yêu cầu hoặc 1 tiếng 
                if (new TimeSpan(22, 0, 0) - result.Last().EndTime.TimeOfDay + DateTimeOffset.Now.Offset >=
                    (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                    freeSlots.Add(new FreeSlotDto
                    {
                        BuildingId = result.Last().LabRoom.BuildingId,
                        RoomId = result.Last().LabRoomId,
                        RoomName = result.Last().LabRoom.RoomName,
                        StartDate = DateOnly.FromDateTime(result.Last().EndTime.ToLocalTime().Date),
                        EndDate = DateOnly.FromDateTime(result.Last().EndTime.ToLocalTime().Date),
                        StartTime = TimeOnly.FromTimeSpan(result.Last().EndTime.ToLocalTime().TimeOfDay),
                        EndTime = new TimeOnly(22, 0, 0)
                    });
            }
            else
            {
                // Nếu từ lúc kết thúc slot i đến 22h mà cách nhau trên 
                // thời gian người dùng yêu cầu hoặc 1 tiếng 
                if (new TimeSpan(22, 0, 0) - result.Last().EndTime.TimeOfDay + DateTimeOffset.Now.Offset >=
                    (request.Duration.HasValue ? request.Duration.Value : new TimeSpan(1, 0, 0)))
                    freeSlots.Add(new FreeSlotDto
                    {
                        BuildingId = result.Last().LabRoom.BuildingId,
                        RoomId = result.Last().LabRoomId,
                        RoomName = result.Last().LabRoom.RoomName,
                        StartDate = DateOnly.FromDateTime(result.Last().EndTime.ToLocalTime().Date),
                        EndDate = DateOnly.FromDateTime(result.Last().EndTime.ToLocalTime().Date),
                        StartTime = TimeOnly.FromTimeSpan(result.Last().EndTime.ToLocalTime().TimeOfDay),
                        EndTime = new TimeOnly(22, 0, 0)
                    });
            }

            return new ResultMessage<List<FreeSlotDto>>
            {
                Success = true,
                Message = "Free slots retrieved successfully.",
                Data = freeSlots
            };
        }
    }
}
