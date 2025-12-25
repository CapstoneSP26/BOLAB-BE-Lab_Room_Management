using Microsoft.EntityFrameworkCore;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Interfaces.Persistence
{
    public interface IBookLABDbContext
    {
        // ===== USER & AUTH =====
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<UserRole> UserRoles { get; }

        // ===== LOCATION =====
        DbSet<Campus> Campuses { get; }
        DbSet<Building> Buildings { get; }
        DbSet<LabRoom> LabRooms { get; }
        DbSet<RoomPolicy> RoomPolicies { get; }

        // ===== ACADEMIC =====
        DbSet<Semester> Semesters { get; }
        DbSet<SlotType> SlotTypes { get; }

        // ===== BOOKING =====
        DbSet<Booking> Bookings { get; }
        DbSet<BookingRequest> BookingRequests { get; }
        DbSet<BookingGroup> BookingGroups { get; }
        DbSet<BookingUser> BookingUsers { get; }

        // ===== GROUP =====
        DbSet<StudentGroup> StudentGroups { get; }
        DbSet<GroupMember> GroupMembers { get; }

        // ===== ATTENDANCE =====
        DbSet<AttendanceSummary> AttendanceSummaries { get; }
        DbSet<AttendanceDetail> AttendanceDetails { get; }

        // ===== EQUIPMENT =====
        DbSet<EquipmentItem> EquipmentItems { get; }
        DbSet<EquipmentMaintenance> EquipmentMaintenances { get; }
        DbSet<RequiredEquipment> RequiredEquipments { get; }

        // ===== FEEDBACK =====
        DbSet<Feedback> Feedbacks { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
