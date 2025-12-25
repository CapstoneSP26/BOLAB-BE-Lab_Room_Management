using Microsoft.EntityFrameworkCore;
using BookLAB.Domain.Entities;
using BookLAB.Application.Common.Interfaces.Persistence;

namespace BookLAB.Infrastructure.Persistence
{
    public class BookLABDbContext : DbContext, IBookLABDbContext
    {
        public BookLABDbContext(DbContextOptions<BookLABDbContext> options)
            : base(options)
        {
        }

        // ===== USER & AUTH =====
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        // ===== LOCATION =====
        public DbSet<Campus> Campuses => Set<Campus>();
        public DbSet<Building> Buildings => Set<Building>();
        public DbSet<LabRoom> LabRooms => Set<LabRoom>();
        public DbSet<RoomPolicy> RoomPolicies => Set<RoomPolicy>();

        // ===== ACADEMIC =====
        public DbSet<Semester> Semesters => Set<Semester>();
        public DbSet<SlotType> SlotTypes => Set<SlotType>();

        // ===== BOOKING =====
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingRequest> BookingRequests => Set<BookingRequest>();

        public DbSet<BookingGroup> BookingGroups => Set<BookingGroup>();
        public DbSet<BookingUser> BookingUsers => Set<BookingUser>();

        // ===== GROUP =====
        public DbSet<StudentGroup> StudentGroups => Set<StudentGroup>();
        public DbSet<GroupMember> GroupMembers => Set<GroupMember>();

        // ===== ATTENDANCE =====
        public DbSet<AttendanceSummary> AttendanceSummaries => Set<AttendanceSummary>();
        public DbSet<AttendanceDetail> AttendanceDetails => Set<AttendanceDetail>();

        // ===== EQUIPMENT =====
        public DbSet<EquipmentItem> EquipmentItems => Set<EquipmentItem>();
        public DbSet<EquipmentMaintenance> EquipmentMaintenances => Set<EquipmentMaintenance>();
        public DbSet<RequiredEquipment> RequiredEquipments => Set<RequiredEquipment>();

        // ===== FEEDBACK =====
        public DbSet<Feedback> Feedbacks => Set<Feedback>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookLABDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
