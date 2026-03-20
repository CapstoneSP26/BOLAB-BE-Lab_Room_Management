// Quick debug script to check booking dates
using System;
using System.Linq;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

var now = DateTime.UtcNow;
var twelveMonthsAgo = now.AddMonths(-12);

Console.WriteLine($"Current UTC Time: {now}");
Console.WriteLine($"12 Months Ago: {twelveMonthsAgo}");

// The problematic code from handler
var bookings = new List<dynamic>();

// We need to check what Year/Month values are being extracted
// This is what the handler does:
// var bookings = await _unitOfWork.Repository<Booking>().Entities
//     .AsNoTracking()
//     .Select(b => new { Year = b.CreatedAt.Year, Month = b.CreatedAt.Month })
//     .ToListAsync(cancellationToken);

// The issue: It's selecting ONLY Year and Month, losing the full CreatedAt value!
// We can't even see what the actual dates are!

Console.WriteLine("Issue found: GetMonthlyBookings selects only Year and Month");
Console.WriteLine("It should also select the full CreatedAt to debug why months are 0");
