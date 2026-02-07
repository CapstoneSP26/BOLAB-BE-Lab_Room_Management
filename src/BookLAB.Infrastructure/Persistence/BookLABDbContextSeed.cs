using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLAB.Infrastructure.Persistence;

public static class BookLABDbContextSeed
{
    public static async Task SeedAsync(BookLABDbContext context, ILogger logger)
    {
        try
        {
            // Seed Campuses
            if (!await context.Campuses.AnyAsync())
            {
                logger.LogInformation("Seeding campuses...");
                
                var campus = new Campus
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CampusName = "Main Campus",
                    Address = "123 University Street",
                    IsActive = true
                };

                await context.Campuses.AddAsync(campus);
                await context.SaveChangesAsync();
            }

            // Seed Buildings
            if (!await context.Buildings.AnyAsync())
            {
                logger.LogInformation("Seeding buildings...");
                
                var building = new Building
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    CampusId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    BuildingName = "Building A",
                    Description = "Main Laboratory Building",
                    BuildingImageUrl = "https://example.com/building-a.jpg"
                };

                await context.Buildings.AddAsync(building);
                await context.SaveChangesAsync();
            }

            // Seed Lab Rooms
            if (!await context.LabRooms.AnyAsync())
            {
                logger.LogInformation("Seeding lab rooms...");
                
                var rooms = new List<LabRoom>
                {
                    new()
                    {
                        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        BuildingId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        RoomName = "Lab 101",
                        Location = "Floor 1, Room 101",
                        OverrideNumber = 30,
                        HasEquipment = true,
                        Description = "Computer Lab with 30 workstations",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false
                    },
                    new()
                    {
                        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                        BuildingId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        RoomName = "Lab 102",
                        Location = "Floor 1, Room 102",
                        OverrideNumber = 25,
                        HasEquipment = true,
                        Description = "Network Lab with 25 workstations",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false
                    }
                };

                await context.LabRooms.AddRangeAsync(rooms);
                await context.SaveChangesAsync();
                
                logger.LogInformation("Seeded {Count} lab rooms", rooms.Count);
            }

            // Seed Purpose Types
            if (!await context.PurposeTypes.AnyAsync())
            {
                logger.LogInformation("Seeding purpose types...");
                
                var purposes = new List<PurposeType>
                {
                    new()
                    {
                        Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                        PurposeName = "Teaching"
                    },
                    new()
                    {
                        Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                        PurposeName = "Research"
                    }
                };

                await context.PurposeTypes.AddRangeAsync(purposes);
                await context.SaveChangesAsync();
                
                logger.LogInformation("Seeded {Count} purpose types", purposes.Count);
            }

            // Seed Test Booking
            if (!await context.Bookings.AnyAsync())
            {
                logger.LogInformation("Seeding test booking...");
                
                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    LabRoomId = Guid.Parse("33333333-3333-3333-3333-333333333333"), // Lab 101
                    StartTime = DateTime.UtcNow.AddHours(2),
                    EndTime = DateTime.UtcNow.AddHours(4),
                    Reason = "Test Google Calendar Integration",
                    BookingStatus = BookingStatus.Approved,
                    PurposeTypeId = Guid.Parse("55555555-5555-5555-5555-555555555555"), // Teaching
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty
                };

                await context.Bookings.AddAsync(booking);
                await context.SaveChangesAsync();
                
                logger.LogInformation("Seeded test booking with ID: {BookingId}", booking.Id);
                logger.LogInformation("Use this ID to test Calendar Sync: {BookingId}", booking.Id);
            }

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}
