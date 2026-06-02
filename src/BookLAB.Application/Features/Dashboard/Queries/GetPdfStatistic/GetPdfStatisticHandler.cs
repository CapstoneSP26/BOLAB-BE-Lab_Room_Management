using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ScottPlot;
using ScottPlot.TickGenerators.TimeUnits;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookLAB.Application.Features.Dashboard.Queries.GetPdfStatistic
{
    public class GetPdfStatisticHandler : IRequestHandler<GetPdfStatisticQuery, byte[]>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPdfStatisticHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<byte[]> Handle(GetPdfStatisticQuery request, CancellationToken cancellationToken)
        {
            GetPdfStatisticQueryReturn InfoCenter = new GetPdfStatisticQueryReturn();

            var labRooms = await _unitOfWork.Repository<LabRoom>().Entities
                .ProjectTo<LabRoomDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            InfoCenter.LabRooms = labRooms;
            InfoCenter.TotalLabRoom = labRooms.Count;
            InfoCenter.TimeType = request.TimeType;

            var time = new DateTimeOffset();
            var timeBefore = new DateTimeOffset();
            var today = DateTimeOffset.Now;

            int totalNewSlot = 0;
            int totalOldSlot = 0;
            int totalFlexibleHour = 0;

            string[] xAxis = [];

            switch (request.TimeType)
            {
                case "1d":
                    {
                        time = new DateTimeOffset(today.Year, today.Month, today.Day, 0, 0, 0, today.Offset);
                        timeBefore = time.AddDays(-1);

                        xAxis = xAxis.Append(time.Date.ToString("dd/MM/yyyy")).ToArray();

                        break;
                    }
                case "1w":
                    {
                        var aWeekAgo = today.AddDays(-7);
                        time = new DateTimeOffset(aWeekAgo.Year, aWeekAgo.Month, aWeekAgo.Day, 0, 0, 0, today.Offset);
                        timeBefore = time.AddDays(-7);

                        for (var date = time.Date; date <= today.Date; date = date.AddDays(1))
                        {
                            xAxis = xAxis.Append(date.ToString("dd/MM/yyyy")).ToArray();
                        }
                        break;
                    }
                case "4m":
                    {
                        var semester1 = today.Month - 1;
                        var semester2 = today.Month - 5;
                        var semester3 = today.Month - 9;

                        if (semester3 >= 0)
                        {
                            time = new DateTimeOffset(today.Year, 9, 1, 0, 0, 0, today.Offset);
                            timeBefore = new DateTimeOffset(today.Year, 5, 1, 0, 0, 0, today.Offset);
                            
                        } else if (semester2 >= 0)
                        {
                            time = new DateTimeOffset(today.Year, 5, 1, 0, 0, 0, today.Offset);
                            timeBefore = new DateTimeOffset(today.Year, 1, 1, 0, 0, 0, today.Offset);
                            
                        } else if (semester1 >= 0)
                        {
                            time = new DateTimeOffset(today.Year, 1, 1, 0, 0, 0, today.Offset);
                            timeBefore = new DateTimeOffset(today.Year - 1, 9, 1, 0, 0, 0, today.Offset);
                            
                        }

                        if (time == new DateTimeOffset())
                            throw new Exception("Invalid time type");

                        for (var date = time.Date; date <= today.Date; date = date.AddMonths(1))
                        {
                            xAxis = xAxis.Append(date.ToString("MM/yyyy")).ToArray();
                        }

                        break;
                    }
                case "8m":
                    {
                        var semester1 = today.Month - 1;
                        var semester2 = today.Month - 5;
                        var semester3 = today.Month - 9;

                        if (semester3 >= 0)
                        {
                            time = new DateTimeOffset(today.Year, 5, 1, 0, 0, 0, today.Offset);
                            timeBefore = new DateTimeOffset(today.Year - 1, 9, 1, 0, 0, 0, today.Offset);
                            
                        } else if (semester2 >= 0)
                        {
                            time = new DateTimeOffset(today.Year, 1, 1, 0, 0, 0, today.Offset);
                            timeBefore = new DateTimeOffset(today.Year - 1, 5, 1, 0, 0, 0, today.Offset);
                            
                        } else if (semester1 >= 0)
                        {
                            time = new DateTimeOffset(today.Year - 1, 9, 1, 0, 0, 0, today.Offset);
                            timeBefore = new DateTimeOffset(today.Year - 1, 1, 1, 0, 0, 0, today.Offset);
                            
                        }

                        if (time == new DateTimeOffset())
                            throw new Exception("Invalid time type");

                        for (var date = time.Date; date <= today.Date; date = date.AddMonths(1))
                        {
                            xAxis = xAxis.Append(date.ToString("MM/yyyy")).ToArray();
                        }

                        break;
                    }

                case "1y":
                    {
                        time = new DateTimeOffset(today.Year - 1, 1, 1, 0, 0, 0, today.Offset);

                        for (var date = time.Date; date <= today.Date; date = date.AddMonths(1))
                        {
                            xAxis = xAxis.Append(date.ToString("MM/yyyy")).ToArray();
                        }

                        break;
                    }
            }

            totalNewSlot = CountTotalNewSlot(time, today, "new");
            totalOldSlot = CountTotalNewSlot(time, today, "old");
            totalFlexibleHour = CountTotalHours(time, today, null);

            var reports = await _unitOfWork.Repository<Report>().Entities
                .Include(x => x.Schedule)
                .Where(x => x.CreatedAt >= time.ToOffset(TimeSpan.Zero)).ToListAsync();

            var reviewedBookings = await _unitOfWork.Repository<Booking>().Entities
                .Include(x => x.SlotType)
                .Where(x => x.CreatedAt >= time.ToOffset(TimeSpan.Zero) &&
                    (x.BookingStatus == Domain.Enums.BookingStatus.Approved || x.BookingStatus == Domain.Enums.BookingStatus.Rejected))
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();

            var reviewedBookingsBefore = await _unitOfWork.Repository<Booking>().Entities
                .Where(x => x.CreatedAt >= timeBefore.ToOffset(TimeSpan.Zero) && x.CreatedAt < time.ToOffset(TimeSpan.Zero) &&
                    (x.BookingStatus == Domain.Enums.BookingStatus.Approved || x.BookingStatus == Domain.Enums.BookingStatus.Rejected))
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();

            var approvedBookings = reviewedBookings.Where(x => x.BookingStatus == Domain.Enums.BookingStatus.Approved).ToList();
            var approvedBookingsBefore = reviewedBookingsBefore.Where(x => x.BookingStatus == Domain.Enums.BookingStatus.Approved).ToList();

            var approvedBookingsCount = approvedBookings.Count;
            var approvedReviewedBookingsBeforeCount = approvedBookingsBefore.Count;

            var totalBookingsFlexibleHours = CountTotalHoursFromBookings(approvedBookings.Where(x => x.SlotType == null).ToList());

            InfoCenter.BookingFrequencyByNewSlot = totalNewSlot == 0 ? 0 : Math.Round((double)approvedBookings.Count(x => x.SlotType != null && x.SlotType.Code == "NEW SLOT") / totalNewSlot, 2);
            InfoCenter.BookingFrequencyByOldSlot = totalOldSlot == 0 ? 0 : Math.Round((double)approvedBookings.Count(x => x.SlotType != null && x.SlotType.Code == "OLD SLOT") / totalOldSlot, 2);
            InfoCenter.BookingFrequencyByFlexibleSlot = totalFlexibleHour == 0 ? 0 : Math.Round((double)totalBookingsFlexibleHours / totalFlexibleHour, 2);
            InfoCenter.BookingFrequencyByAllSlot = (approvedBookingsCount) == 0 ? 0 : Math.Round((double)approvedBookings.Count() / approvedBookingsCount, 2);

            InfoCenter.TotalApprovedBookings = approvedBookingsCount;
            InfoCenter.TotalApprovedBookingsBefore = approvedReviewedBookingsBeforeCount;
            InfoCenter.RejectedBookings = reviewedBookings.Count(x => x.BookingStatus == Domain.Enums.BookingStatus.Rejected);
            InfoCenter.AverageBookingsPerTime = xAxis.Count() == 0 ? approvedBookingsCount : approvedBookingsCount / xAxis.Count();

            Dictionary<int, int[]> yAxisList = new Dictionary<int, int[]>();
            Dictionary<int, int> bookingPerRoomBefore = new Dictionary<int, int>();
            int[] yAxis = [];
            List<LabRoomExtraInfo> labRoomExtraInfos = new List<LabRoomExtraInfo>();

            switch (request.TimeType)
            {
                case "1d":
                    {
                        foreach (var room in labRooms)
                        {
                            yAxisList.Add(room.Id, []);
                            for (var date = time.Date; date <= today.Date; date = date.AddDays(1))
                            {
                                yAxisList[room.Id] = yAxisList[room.Id].Append(approvedBookings.Count(x => x.LabRoomId == room.Id && x.CreatedAt.Date == date)).ToArray();
                            }
                            bookingPerRoomBefore.Add(room.Id, 0);
                            bookingPerRoomBefore[room.Id] = approvedBookingsBefore.Count(x => x.LabRoomId == room.Id && x.CreatedAt.Date >= timeBefore.Date && x.CreatedAt.Date < time.Date);

                            var usageNewSlot = CountTotalReviewedBookingSlot(approvedBookings.Where(x => x.LabRoomId == room.Id).ToList(), "new") / (double)totalNewSlot;
                            var usageOldSlot = CountTotalReviewedBookingSlot(approvedBookings.Where(x => x.LabRoomId == room.Id).ToList(), "old") / (double)totalOldSlot;
                            var usageFlexibleSlot = CountTotalHoursFromBookings(approvedBookings.Where(x => x.LabRoomId == room.Id && x.SlotType == null).ToList()) / (double)totalFlexibleHour;

                            var RoomReports = reports.Where(x => x.Schedule.LabRoomId == room.Id).ToList();
                            var UnresolvedRoomReports = RoomReports.Where(x => x.IsResolved == false).ToList();
                            var ResolvedRoomReports = RoomReports.Where(x => x.IsResolved == true).ToList();

                            var TotalFixingHours = ResolvedRoomReports.Select(x => (x.UpdatedAt - x.CreatedAt).Value.TotalHours).Sum();

                            labRoomExtraInfos.Add(new LabRoomExtraInfo
                            {
                                LabRoomId = room.Id,
                                UsageRated = usageNewSlot + usageOldSlot + usageFlexibleSlot,
                                IncreasedRate = bookingPerRoomBefore[room.Id] == 0 ? "+" + yAxisList[room.Id].Sum() : "↑" + Math.Round((double)((yAxisList[room.Id].Sum() - bookingPerRoomBefore[room.Id]) / bookingPerRoomBefore[room.Id]) * 100, 2) + "%",
                                NumberOfIncidents = RoomReports.Count(),
                                AverageFixingTime = ResolvedRoomReports.Count() == 0 ? 0 : Math.Round(TotalFixingHours / (double)ResolvedRoomReports.Count(), 2),
                                UnresolvedIncidents = UnresolvedRoomReports.Count()
                            });
                        }

                        break;
                    }
                case "1w":
                    {
                        foreach (var room in labRooms)
                        {
                            yAxisList.Add(room.Id, []);
                            for (var date = time.Date; date <= today.Date; date = date.AddDays(1))
                            {
                                yAxisList[room.Id] = yAxisList[room.Id].Append(approvedBookings.Count(x => x.LabRoomId == room.Id && x.CreatedAt.Date == date)).ToArray();
                            }
                            bookingPerRoomBefore.Add(room.Id, 0);
                            bookingPerRoomBefore[room.Id] = approvedBookingsBefore.Count(x => x.LabRoomId == room.Id && x.CreatedAt.Date >= timeBefore.Date && x.CreatedAt.Date < time.Date);

                            var usageNewSlot = CountTotalReviewedBookingSlot(approvedBookings.Where(x => x.LabRoomId == room.Id).ToList(), "new") / (double)totalNewSlot;
                            var usageOldSlot = CountTotalReviewedBookingSlot(approvedBookings.Where(x => x.LabRoomId == room.Id).ToList(), "old") / (double)totalOldSlot;
                            var usageFlexibleSlot = CountTotalHoursFromBookings(approvedBookings.Where(x => x.LabRoomId == room.Id && x.SlotType == null).ToList()) / (double)totalFlexibleHour;

                            var RoomReports = reports.Where(x => x.Schedule.LabRoomId == room.Id).ToList();
                            var UnresolvedRoomReports = RoomReports.Where(x => x.IsResolved == false).ToList();
                            var ResolvedRoomReports = RoomReports.Where(x => x.IsResolved == true).ToList();

                            var TotalFixingHours = ResolvedRoomReports.Select(x => (x.UpdatedAt - x.CreatedAt).Value.TotalHours).Sum();

                            labRoomExtraInfos.Add(new LabRoomExtraInfo
                            {
                                LabRoomId = room.Id,
                                UsageRated = usageNewSlot + usageOldSlot + usageFlexibleSlot,
                                IncreasedRate = bookingPerRoomBefore[room.Id] == 0 ? "+" + yAxisList[room.Id].Sum() : "↑" + Math.Round((double)((yAxisList[room.Id].Sum() - bookingPerRoomBefore[room.Id]) / bookingPerRoomBefore[room.Id]) * 100, 2) + "%",
                                NumberOfIncidents = RoomReports.Count(),
                                AverageFixingTime = ResolvedRoomReports.Count() == 0 ? 0 : Math.Round(TotalFixingHours / (double)ResolvedRoomReports.Count(), 2),
                                UnresolvedIncidents = UnresolvedRoomReports.Count()
                            });
                        }
                        
                        break;
                    }
                case "4m":
                    {
                        foreach (var room in labRooms)
                        {
                            yAxisList.Add(room.Id, []);
                            for (var date = time.Date; date <= today.Date; date = date.AddMonths(1))
                            {
                                yAxisList[room.Id] = yAxisList[room.Id].Append(approvedBookings.Count(x => x.LabRoomId == room.Id && x.CreatedAt.Year == date.Year && x.CreatedAt.Month == date.Month)).ToArray();
                            }
                            bookingPerRoomBefore.Add(room.Id, 0);
                            bookingPerRoomBefore[room.Id] = approvedBookingsBefore.Count(x => x.LabRoomId == room.Id && x.CreatedAt.Date >= timeBefore.Date && x.CreatedAt.Date < time.Date);

                            var usageNewSlot = CountTotalReviewedBookingSlot(approvedBookings.Where(x => x.LabRoomId == room.Id).ToList(), "new") / (double)totalNewSlot;
                            var usageOldSlot = CountTotalReviewedBookingSlot(approvedBookings.Where(x => x.LabRoomId == room.Id).ToList(), "old") / (double)totalOldSlot;
                            var usageFlexibleSlot = CountTotalHoursFromBookings(approvedBookings.Where(x => x.LabRoomId == room.Id && x.SlotType == null).ToList()) / (double)totalFlexibleHour;

                            var RoomReports = reports.Where(x => x.Schedule.LabRoomId == room.Id).ToList();
                            var UnresolvedRoomReports = RoomReports.Where(x => x.IsResolved == false).ToList();
                            var ResolvedRoomReports = RoomReports.Where(x => x.IsResolved == true).ToList();

                            var TotalFixingHours = ResolvedRoomReports.Select(x => (x.UpdatedAt - x.CreatedAt).Value.TotalHours).Sum();

                            labRoomExtraInfos.Add(new LabRoomExtraInfo
                            {
                                LabRoomId = room.Id,
                                UsageRated = usageNewSlot + usageOldSlot + usageFlexibleSlot,
                                IncreasedRate = bookingPerRoomBefore[room.Id] == 0 ? "+" + yAxisList[room.Id].Sum() : "↑" + Math.Round((double)((yAxisList[room.Id].Sum() - bookingPerRoomBefore[room.Id]) / bookingPerRoomBefore[room.Id]) * 100, 2) + "%",
                                NumberOfIncidents = RoomReports.Count(),
                                AverageFixingTime = ResolvedRoomReports.Count() == 0 ? 0 : Math.Round(TotalFixingHours / (double)ResolvedRoomReports.Count(), 2),
                                UnresolvedIncidents = UnresolvedRoomReports.Count()
                            });
                        }
                        break;
                    }
                case "8m":
                    {
                        foreach (var room in labRooms)
                        {
                            yAxisList.Add(room.Id, []);
                            for (var date = time.Date; date <= today.Date; date = date.AddMonths(1))
                            {
                                yAxisList[room.Id] = yAxisList[room.Id].Append(approvedBookings.Count(x => x.LabRoomId == room.Id && x.CreatedAt.Year == date.Year && x.CreatedAt.Month == date.Month)).ToArray();
                            }
                            bookingPerRoomBefore.Add(room.Id, 0);
                            bookingPerRoomBefore[room.Id] = approvedBookingsBefore.Count(x => x.LabRoomId == room.Id && x.CreatedAt.Date >= timeBefore.Date && x.CreatedAt.Date < time.Date);

                            var usageNewSlot = CountTotalReviewedBookingSlot(approvedBookings.Where(x => x.LabRoomId == room.Id).ToList(), "new") / (double)totalNewSlot;
                            var usageOldSlot = CountTotalReviewedBookingSlot(approvedBookings.Where(x => x.LabRoomId == room.Id).ToList(), "old") / (double)totalOldSlot;
                            var usageFlexibleSlot = CountTotalHoursFromBookings(approvedBookings.Where(x => x.LabRoomId == room.Id && x.SlotType == null).ToList()) / (double)totalFlexibleHour;

                            var RoomReports = reports.Where(x => x.Schedule.LabRoomId == room.Id).ToList();
                            var UnresolvedRoomReports = RoomReports.Where(x => x.IsResolved == false).ToList();
                            var ResolvedRoomReports = RoomReports.Where(x => x.IsResolved == true).ToList();

                            var TotalFixingHours = ResolvedRoomReports.Select(x => (x.UpdatedAt - x.CreatedAt).Value.TotalHours).Sum();

                            labRoomExtraInfos.Add(new LabRoomExtraInfo
                            {
                                LabRoomId = room.Id,
                                UsageRated = usageNewSlot + usageOldSlot + usageFlexibleSlot,
                                IncreasedRate = bookingPerRoomBefore[room.Id] == 0 ? "+" + yAxisList[room.Id].Sum() : "↑" + Math.Round((double)((yAxisList[room.Id].Sum() - bookingPerRoomBefore[room.Id]) / bookingPerRoomBefore[room.Id]) * 100, 2) + "%",
                                NumberOfIncidents = RoomReports.Count(),
                                AverageFixingTime = ResolvedRoomReports.Count() == 0 ? 0 : Math.Round(TotalFixingHours / (double)ResolvedRoomReports.Count(), 2),
                                UnresolvedIncidents = UnresolvedRoomReports.Count()
                            });
                        }
                        break;
                    }
                case "1y":
                    {
                        foreach (var room in labRooms)
                        {
                            yAxisList.Add(room.Id, []);
                            for (var date = time.Date; date <= today.Date; date = date.AddMonths(1))
                            {
                                yAxisList[room.Id] = yAxisList[room.Id].Append(approvedBookings.Count(x => x.LabRoomId == room.Id && x.CreatedAt.Year == date.Year && x.CreatedAt.Month == date.Month)).ToArray();
                            }
                            bookingPerRoomBefore.Add(room.Id, 0);
                            bookingPerRoomBefore[room.Id] = approvedBookingsBefore.Count(x => x.LabRoomId == room.Id && x.CreatedAt.Date >= timeBefore.Date && x.CreatedAt.Date < time.Date);

                            var usageNewSlot = CountTotalReviewedBookingSlot(approvedBookings.Where(x => x.LabRoomId == room.Id).ToList(), "new") / (double)totalNewSlot;
                            var usageOldSlot = CountTotalReviewedBookingSlot(approvedBookings.Where(x => x.LabRoomId == room.Id).ToList(), "old") / (double)totalOldSlot;
                            var usageFlexibleSlot = CountTotalHoursFromBookings(approvedBookings.Where(x => x.LabRoomId == room.Id && x.SlotType == null).ToList()) / (double)totalFlexibleHour;

                            var RoomReports = reports.Where(x => x.Schedule.LabRoomId == room.Id).ToList();
                            var UnresolvedRoomReports = RoomReports.Where(x => x.IsResolved == false).ToList();
                            var ResolvedRoomReports = RoomReports.Where(x => x.IsResolved == true).ToList();

                            var TotalFixingHours = ResolvedRoomReports.Select(x => (x.UpdatedAt - x.CreatedAt).Value.TotalHours).Sum();

                            labRoomExtraInfos.Add(new LabRoomExtraInfo
                            {
                                LabRoomId = room.Id,
                                UsageRated = usageNewSlot + usageOldSlot + usageFlexibleSlot,
                                IncreasedRate = bookingPerRoomBefore[room.Id] == 0 ? "+" + yAxisList[room.Id].Sum() : "↑" + Math.Round((double)((yAxisList[room.Id].Sum() - bookingPerRoomBefore[room.Id]) / bookingPerRoomBefore[room.Id]) * 100, 2) + "%",
                                NumberOfIncidents = RoomReports.Count(),
                                AverageFixingTime = ResolvedRoomReports.Count() == 0 ? 0 : Math.Round(TotalFixingHours / (double)ResolvedRoomReports.Count(), 2),
                                UnresolvedIncidents = UnresolvedRoomReports.Count()
                            });
                        }
                        break;
                    }
            }

            //Dictionary<int, byte[]> chartList = new Dictionary<int, byte[]>();

            foreach (var room in labRooms)
            {
                var plt = new Plot();
                //chartList.Add(room.Id, []);
                double[] xs = Enumerable.Range(0, yAxisList[room.Id].Length).Select(i => (double)i).ToArray();

                // Vẽ biểu đồ đường
                plt.Add.Scatter(xs, yAxisList[room.Id]);
                plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(xs, xAxis);

                plt.Title("Bookings Frequency of room " + room.RoomNo);
                plt.XLabel("Time");
                plt.YLabel("Number of Bookings");

                //chartList[room.Id] = plt.GetImageBytes(800, 400);
                labRoomExtraInfos.First(x => x.LabRoomId == room.Id).Chart = plt.GetImageBytes(800, 400);
            }

            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);

                    page.Header().Text("Booking Statistics").FontSize(20).Bold();

                    page.Content()
                        .Column(col =>
                        {
                            col.Item().Text("");
                            col.Item().Text(text => 
                            {
                                text.Span("Total Lab Rooms: ");
                                text.Span(InfoCenter.TotalLabRoom.ToString()).Bold();
                            });
                            col.Item().Text(text =>
                            {
                                text.Span("Approved Request: ");
                                text.Span(InfoCenter.TotalApprovedBookings.ToString()).Bold();
                            });
                            col.Item().Text(text =>
                            {
                                text.Span("Rejected Request: ");
                                text.Span(InfoCenter.RejectedBookings.ToString()).Bold();
                            });
                            

                            col.Item().Text("");
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(60);
                                    columns.ConstantColumn(70);
                                    columns.ConstantColumn(60);
                                    columns.ConstantColumn(50);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Border(1).Padding(5).Text("Lab Room Name");
                                    header.Cell().Border(1).Padding(5).Text("Lab Room Code").AlignCenter();
                                    header.Cell().Border(1).Padding(5).Text("Usage Frequency").AlignCenter();
                                    header.Cell().Border(1).Padding(5).Text("Increase Rate").AlignCenter();
                                    header.Cell().Border(1).Padding(5).Text("Active").AlignCenter();
                                });

                                foreach (var room in labRooms)
                                {
                                    var labRoomExtra = labRoomExtraInfos.First(x => x.LabRoomId == room.Id);

                                    table.Cell().Border(1).Padding(5).Text(room.RoomName);
                                    table.Cell().Border(1).Padding(5).Text(room.RoomNo).AlignCenter();
                                    table.Cell().Border(1).Padding(5).Text(labRoomExtra.UsageRated != null ? (Math.Round(labRoomExtraInfos.First(x => x.LabRoomId == room.Id).UsageRated.Value * 100, 2)).ToString() + "%" : "0%").AlignCenter();
                                    table.Cell().Border(1).Padding(5).Text(labRoomExtra.IncreasedRate).AlignCenter();
                                    table.Cell().Border(1).Padding(5).Text(room.IsActive.ToString()).AlignCenter();
                                }
                            });
                            col.Item().PaddingTop(5).Text("Booking Lab Room Statistic").AlignCenter();

                            col.Item().Text("");

                            col.Item().Text("");
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(60);
                                    columns.ConstantColumn(60);
                                    columns.ConstantColumn(60);
                                    columns.ConstantColumn(65);
                                    columns.ConstantColumn(50);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Border(1).Padding(5).Text("Lab Room Name");
                                    header.Cell().Border(1).Padding(5).Text("Lab Room Code").AlignCenter();
                                    header.Cell().Border(1).Padding(5).Text("Received Incident").AlignCenter();
                                    header.Cell().Border(1).Padding(5).Text("Average Fixing Time (Hour)").AlignCenter();
                                    header.Cell().Border(1).Padding(5).Text("Unresolved Incident").AlignCenter();
                                    header.Cell().Border(1).Padding(5).Text("Active").AlignCenter();
                                });

                                foreach (var room in labRooms)
                                {
                                    var labRoomExtra = labRoomExtraInfos.First(x => x.LabRoomId == room.Id);

                                    table.Cell().Border(1).Padding(5).Text(room.RoomName);
                                    table.Cell().Border(1).Padding(5).Text(room.RoomNo).AlignCenter();
                                    table.Cell().Border(1).Padding(5).Text(labRoomExtra.NumberOfIncidents.ToString()).AlignCenter();
                                    table.Cell().Border(1).Padding(5).Text(labRoomExtra.AverageFixingTime.ToString()).AlignCenter();
                                    table.Cell().Border(1).Padding(5).Text(labRoomExtra.UnresolvedIncidents.ToString()).AlignCenter();
                                    table.Cell().Border(1).Padding(5).Text(room.IsActive.ToString()).AlignCenter();
                                }
                            });
                            col.Item().PaddingTop(5).Text("Report Incident Statistic").AlignCenter();

                            col.Item().Text("");
                            col.Item().Text("");
                            col.Item().Text("");
                            col.Item().Text("");
                            col.Item().Text("");
                            col.Item().Text("");
                            col.Item().Text("Statistical Chart").Bold().Underline();
                            foreach (var room in labRooms)
                            {
                                col.Item().Image(labRoomExtraInfos.First(x => x.LabRoomId == room.Id).Chart);
                                col.Item().Text("");
                            }
                            
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ").FontSize(10);
                            x.CurrentPageNumber().FontSize(10);
                        });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();

            return pdfBytes;
        }

        public int CountTotalNewSlot(DateTimeOffset from, DateTimeOffset to, string slotType)
        {
            var slots = new List<Slot>();

            if (slotType == "new")
            {
                slots = new List<Slot>
                {
                    new Slot { Start = new TimeSpan(7, 0, 0), End = new TimeSpan(9, 15, 0) },
                    new Slot { Start = new TimeSpan(9, 30, 0), End = new TimeSpan(11, 45, 0) },
                    new Slot { Start = new TimeSpan(12, 30, 0), End = new TimeSpan(14, 45, 0) },
                    new Slot { Start = new TimeSpan(15, 0, 0), End = new TimeSpan(17, 15, 0) }
                };
            }
            else if (slotType == "old")
            {
                slots = new List<Slot>
                {
                    new Slot { Start = new TimeSpan(7, 0, 0), End = new TimeSpan(8, 30, 0) },
                    new Slot { Start = new TimeSpan(8, 45, 0), End = new TimeSpan(10, 15, 0) },
                    new Slot { Start = new TimeSpan(10, 30, 0), End = new TimeSpan(12, 00, 0) },
                    new Slot { Start = new TimeSpan(12, 30, 0), End = new TimeSpan(14, 00, 0) },
                    new Slot { Start = new TimeSpan(14, 15, 0), End = new TimeSpan(15, 45, 0) },
                    new Slot { Start = new TimeSpan(16, 00, 0), End = new TimeSpan(17, 30, 0) }
                };
            }
            else
            {
                throw new Exception("Invalid slot type");
            }

            int count = 0;

            for (var day = from.Date; day <= to.Date; day = day.AddDays(1))
            {
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                foreach (var slot in slots)
                {
                    var slotStart = new DateTimeOffset(day + slot.Start, from.Offset);
                    var slotEnd = new DateTimeOffset(day + slot.End, from.Offset);

                    if (slotStart >= from && slotEnd <= to)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="timeType">only enter "new" or "old"</param>
        /// <returns></returns>
        public int CountTotalReviewedBookingSlot(List<Booking> bookings, string timeType)
        {
            if (bookings == null || bookings.Count <= 0 || timeType != "new" || timeType != "old")
                return 0;

            if (timeType == "new")
            {
                bookings = bookings.Where(x => x.SlotType != null && x.SlotType.Code == "NEW SLOT").OrderBy(x => x.StartTime).ToList();
            }
            else if (timeType == "old")
            {
                bookings = bookings.Where(x => x.SlotType != null && x.SlotType.Code == "OLD SLOT").OrderBy(x => x.StartTime).ToList();
            }

            int count = 0;
            for(int i = 0; i < bookings.Count; i++)
            {
                var booking = bookings[i];
                if (booking.StartTime != bookings[i - 1].StartTime)
                {
                    count++;
                }
            }
            return count;
        }

        public int CountTotalHours(DateTimeOffset from, DateTimeOffset to, string? timeType)
        {
            int totalHours = 0;
            var today = DateTimeOffset.Now;
            var plusHours = 0;

            if (timeType != null)
            {
                plusHours = 9;

                for (var day = from.Date; day <= to.Date; day = day.AddDays(1))
                {
                    if (day.DayOfWeek == DayOfWeek.Sunday || day.DayOfWeek == DayOfWeek.Saturday)
                        continue;

                    totalHours += plusHours; // Assuming 9 hours of booking time per day
                    if (day.Year == today.Year && day.Month == today.Month && day.Day == today.Day)
                    {
                        totalHours += (int)(today - day.AddHours(7)).TotalHours;
                        break;
                    }
                }
            } else
            {
                plusHours = 15;

                for (var day = from.Date; day <= to.Date; day = day.AddDays(1))
                {
                    totalHours += plusHours; // Assuming 15 hours of booking time per day
                    if (day.Year == today.Year && day.Month == today.Month && day.Day == today.Day)
                    {
                        totalHours += (int)(today - day.AddHours(7)).TotalHours;
                        break;
                    }
                }
            }
            
            return totalHours;
        }

        public double CountTotalHoursFromBookings(List<Booking> bookings)
        {
            double totalHours = 0;
            foreach (var booking in bookings)
            {
                var startTime = booking.StartTime;
                var endTime = booking.EndTime;
                if (endTime > startTime)
                {
                    totalHours += (endTime - startTime).TotalHours;
                }
            }
            return totalHours;
        }
    }
    public class Slot
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }
}
