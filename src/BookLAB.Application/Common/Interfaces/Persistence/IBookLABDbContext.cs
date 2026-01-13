using Microsoft.EntityFrameworkCore;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Interfaces.Persistence
{
    public interface IBookLABDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<UserRole> UserRoles { get; }
        DbSet<Campus> Campuses { get; }
        DbSet<Building> Buildings { get; }
        DbSet<LabRoom> LabRooms { get; }
        DbSet<LabOwner> LabOwners { get; }
        DbSet<RoomPolicy> RoomPolicies { get; }
        DbSet<SlotType> SlotTypes { get; }
        DbSet<SlotFrame> SlotFrames { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<BookingRequest> BookingRequests { get; }
        DbSet<BookingGroup> BookingGroups { get; }
        DbSet<Group> Groups { get; }
        DbSet<GroupMember> GroupMembers { get; }
        DbSet<Attendance> Attendances { get; }
        DbSet<PurposeType> PurposeTypes { get; }
        DbSet<Schedule> Schedules { get; }
        DbSet<Report> Reports { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
