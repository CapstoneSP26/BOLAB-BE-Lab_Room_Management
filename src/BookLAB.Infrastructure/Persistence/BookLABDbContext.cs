using Microsoft.EntityFrameworkCore;
using BookLAB.Domain.Entities;

namespace BookLAB.Infrastructure.Persistence
{
    public class BookLABDbContext : DbContext
    {
        public BookLABDbContext(DbContextOptions<BookLABDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Campus> Campuses => Set<Campus>();
        public DbSet<Building> Buildings => Set<Building>();
        public DbSet<LabRoom> LabRooms => Set<LabRoom>();
        public DbSet<RoomPolicy> RoomPolicies => Set<RoomPolicy>();
        public DbSet<SlotType> SlotTypes => Set<SlotType>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingRequest> BookingRequests => Set<BookingRequest>();
        public DbSet<BookingGroup> BookingGroups => Set<BookingGroup>();
        public DbSet<Group> StudentGroups => Set<Group>();
        public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<LabOwner> LabOwners => Set<LabOwner>();
        public DbSet<SlotFrame> SlotFrames => Set<SlotFrame>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<PurposeType> PurposeTypes => Set<PurposeType>();
        public DbSet<Schedule> Schedules => Set<Schedule>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookLABDbContext).Assembly);
            modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }
    }
}
