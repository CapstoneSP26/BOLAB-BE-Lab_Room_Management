using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookLAB.Infrastructure.Persistence
{
    public static class SeedData
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Deterministic ids for FK relationships
            var user1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var user2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var user3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

            var labOwner1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var labOwner2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var labOwner3Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

            var booking1Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var booking2Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var booking3Id = Guid.Parse("66666666-6666-6666-6666-666666666666");

            var bookingGroup1Id = Guid.Parse("77777777-7777-7777-7777-777777777777");
            var bookingGroup2Id = Guid.Parse("88888888-8888-8888-8888-888888888888");
            var bookingGroup3Id = Guid.Parse("99999999-9999-9999-9999-999999999999");

            var bookingRequest1Id = Guid.Parse("12121212-1212-1212-1212-121212121212");
            var bookingRequest2Id = Guid.Parse("13131313-1313-1313-1313-131313131313");
            var bookingRequest3Id = Guid.Parse("14141414-1414-1414-1414-141414141414");

            var attendance1Id = Guid.Parse("15151515-1515-1515-1515-151515151515");
            var attendance2Id = Guid.Parse("16161616-1616-1616-1616-161616161616");
            var attendance3Id = Guid.Parse("17171717-1717-1717-1717-171717171717");

            var group1Id = Guid.Parse("18181818-1818-1818-1818-181818181818");
            var group2Id = Guid.Parse("19191919-1919-1919-1919-191919191919");
            var group3Id = Guid.Parse("20202020-2020-2020-2020-202020202020");

            var report1Id = Guid.Parse("21212121-2121-2121-2121-212121212121");
            var report2Id = Guid.Parse("22222222-2222-2222-2222-222222222221");
            var report3Id = Guid.Parse("23232323-2323-2323-2323-232323232323");

            var reportImage1Id = Guid.Parse("24242424-2424-2424-2424-242424242424");
            var reportImage2Id = Guid.Parse("25252525-2525-2525-2525-252525252525");
            var reportImage3Id = Guid.Parse("26262626-2626-2626-2626-262626262626");

            var schedule1Id = Guid.Parse("27272727-2727-2727-2727-272727272727");
            var schedule2Id = Guid.Parse("28282828-2828-2828-2828-282828282828");
            var schedule3Id = Guid.Parse("29292929-2929-2929-2929-292929292929");

            var roomPolicy1Id = Guid.Parse("30303030-3030-3030-3030-303030303030");
            var roomPolicy2Id = Guid.Parse("31313131-3131-3131-3131-313131313131");
            var roomPolicy3Id = Guid.Parse("32323232-3232-3232-3232-323232323232");

            var labImage1Id = Guid.Parse("33333333-3333-3333-3333-333333333330");
            var labImage2Id = Guid.Parse("34343434-3434-3434-3434-343434343434");
            var labImage3Id = Guid.Parse("35353535-3535-3535-3535-353535353535");

            var groupMember1Id = Guid.Parse("36363636-3636-3636-3636-363636363636");
            var groupMember2Id = Guid.Parse("37373737-3737-3737-3737-373737373737");
            var groupMember3Id = Guid.Parse("38383838-3838-3838-3838-383838383838");

            // Int keys
            // Campuses
            modelBuilder.Entity<Campus>().HasData(
                new Campus { Id = 1, CampusName = "Main Campus", Address = "1 University Ave", CampusImageUrl = null, IsActive = true },
                new Campus { Id = 2, CampusName = "North Campus", Address = "100 North St", CampusImageUrl = null, IsActive = true },
                new Campus { Id = 3, CampusName = "West Campus", Address = "50 West Blvd", CampusImageUrl = null, IsActive = true }
            );

            // Buildings
            modelBuilder.Entity<Building>().HasData(
                new Building { Id = 1, CampusId = 1, BuildingName = "Science Building", Description = "Science faculty building", BuildingImageUrl = null },
                new Building { Id = 2, CampusId = 1, BuildingName = "Engineering Building", Description = "Engineering labs", BuildingImageUrl = null },
                new Building { Id = 3, CampusId = 2, BuildingName = "Admin Building", Description = "Administration", BuildingImageUrl = null }
            );

            // LabRooms (int PK)
            modelBuilder.Entity<LabRoom>().HasData(
                new LabRoom { Id = 1, BuildingId = 1, RoomName = "Lab A1", RoomNo = "Gamma 101", Location = "Floor 1", Description = "General purpose lab", OverrideNumber = 0, HasEquipment = true, CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero), CreatedBy = user1Id, IsActive = true, IsDeleted = false },
                new LabRoom { Id = 2, BuildingId = 1, RoomName = "Lab A2", RoomNo = "Gamma 102", Location = "Floor 2", Description = "Hardware lab", OverrideNumber = 0, HasEquipment = true, CreatedAt = new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero), CreatedBy = user1Id, IsActive = true, IsDeleted = false },
                new LabRoom { Id = 3, BuildingId = 2, RoomName = "Lab B1", RoomNo = "Alpha 101", Location = "Floor 3", Description = "Software lab", OverrideNumber = 0, HasEquipment = false, CreatedAt = new DateTimeOffset(2025, 1, 3, 0, 0, 0, TimeSpan.Zero), CreatedBy = user1Id, IsActive = true, IsDeleted = false }
            );

            // PurposeTypes
            modelBuilder.Entity<PurposeType>().HasData(
                new PurposeType { Id = 1, PurposeName = "Lecture" },
                new PurposeType { Id = 2, PurposeName = "Practical" },
                new PurposeType { Id = 3, PurposeName = "Workshop" }
            );

            // SlotTypes
            modelBuilder.Entity<SlotType>().HasData(
                new SlotType { Id = 1, Code = "S90", Name = "90-min slot", CampusId = 1 },
                new SlotType { Id = 2, Code = "S120", Name = "120-min slot", CampusId = 1 },
                new SlotType { Id = 3, Code = "S45", Name = "45-min slot", CampusId = 2 }
            );

            // SlotFrames
            modelBuilder.Entity<SlotFrame>().HasData(
                new SlotFrame { Id = 1, SlotTypeId = 1, StartTimeSlot = new TimeOnly(8, 0), EndTimeSlot = new TimeOnly(9, 30), OrderIndex = 1 },
                new SlotFrame { Id = 2, SlotTypeId = 1, StartTimeSlot = new TimeOnly(9, 45), EndTimeSlot = new TimeOnly(11, 15), OrderIndex = 2 },
                new SlotFrame { Id = 3, SlotTypeId = 2, StartTimeSlot = new TimeOnly(13, 0), EndTimeSlot = new TimeOnly(15, 0), OrderIndex = 1 }
            );

            // Roles (int PK)
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "Admin" },
                new Role { Id = 2, RoleName = "Manager" },
                new Role { Id = 3, RoleName = "Lecturer" }
            );

            // Users (Guid PK)
            modelBuilder.Entity<User>().HasData(
                new User { Id = user1Id, Email = "alice@example.edu", FullName = "Alice Tran", UserCode= "AliceT", CampusId = 1, CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero), CreatedBy = null, IsDeleted = false, IsActive = true },
                new User { Id = user2Id, Email = "bob@example.edu", FullName = "Bob Nguyen", UserCode = "BobN", CampusId = 1, CreatedAt = new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero), CreatedBy = null, IsDeleted = false, IsActive = true },
                new User { Id = user3Id, Email = "carol@example.edu", FullName = "Carol Le", UserCode = "CarolL", CampusId = 2, CreatedAt = new DateTimeOffset(2025, 1, 3, 0, 0, 0, TimeSpan.Zero), CreatedBy = null, IsDeleted = false, IsActive = true }
            );

            // UserRoles (composite)
            modelBuilder.Entity<UserRole>().HasData(
                new { UserId = user1Id, RoleId = 1 },
                new { UserId = user2Id, RoleId = 2 },
                new { UserId = user3Id, RoleId = 3 }
            );

            // LabOwners (BaseEntity -> Guid Id)
            modelBuilder.Entity<LabOwner>().HasData(
                new LabOwner { Id = labOwner1Id, UserId = user1Id, LabRoomId = 1 },
                new LabOwner { Id = labOwner2Id, UserId = user2Id, LabRoomId = 2 },
                new LabOwner { Id = labOwner3Id, UserId = user3Id, LabRoomId = 3 }
            );

            // LabImages
            modelBuilder.Entity<LabImage>().HasData(
                new LabImage { Id = labImage1Id, LabRoomId = 1, ImageUrl = "https://cdn.example/room1.jpg", Size = 1024, FileType = (FileType)0, IsAvatar = true },
                new LabImage { Id = labImage2Id, LabRoomId = 2, ImageUrl = "https://cdn.example/room2.jpg", Size = 2048, FileType = (FileType)0, IsAvatar = false },
                new LabImage { Id = labImage3Id, LabRoomId = 3, ImageUrl = "https://cdn.example/room3.jpg", Size = 512, FileType = (FileType)0, IsAvatar = false }
            );

            // RoomPolicies
            modelBuilder.Entity<RoomPolicy>().HasData(
                new RoomPolicy { Id = roomPolicy1Id, LabRoomId = 1, PolicyKey = "MaxCapacity", PolicyValue = "30", CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero), CreatedBy = user1Id, IsActive = true },
                new RoomPolicy { Id = roomPolicy2Id, LabRoomId = 2, PolicyKey = "Projector", PolicyValue = "Required", CreatedAt = new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero), CreatedBy = user1Id, IsActive = true },
                new RoomPolicy { Id = roomPolicy3Id, LabRoomId = 3, PolicyKey = "FoodAllowed", PolicyValue = "No", CreatedAt = new DateTimeOffset(2025, 1, 3, 0, 0, 0, TimeSpan.Zero), CreatedBy = user1Id, IsActive = true }
            );

            // Groups
            modelBuilder.Entity<Group>().HasData(
                new Group { Id = group1Id, GroupName = "Team Alpha", OwnerId = user1Id, IsDeleted = false, CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero), CreatedBy = user1Id },
                new Group { Id = group2Id, GroupName = "Team Beta", OwnerId = user2Id, IsDeleted = false, CreatedAt = new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero), CreatedBy = user2Id },
                new Group { Id = group3Id, GroupName = "Team Gamma", OwnerId = user3Id, IsDeleted = false, CreatedAt = new DateTimeOffset(2025, 1, 3, 0, 0, 0, TimeSpan.Zero), CreatedBy = user3Id }
            );

            // GroupMembers
            modelBuilder.Entity<GroupMember>().HasData(
                new GroupMember { Id = groupMember1Id, GroupId = group1Id, UserId = user2Id },
                new GroupMember { Id = groupMember2Id, GroupId = group2Id, UserId = user3Id },
                new GroupMember { Id = groupMember3Id, GroupId = group3Id, UserId = user1Id }
            );

            // Bookings
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    Id = booking1Id,
                    LabRoomId = 1,
                    StartTime = new DateTimeOffset(2025, 2, 1, 8, 0, 0, TimeSpan.Zero),
                    EndTime = new DateTimeOffset(2025, 2, 1, 10, 0, 0, TimeSpan.Zero),
                    SlotTypeId = 1,
                    BookingStatus = BookingStatus.PendingApproval,
                    BookingType = (BookingType)0,
                    StudentCount = 10,
                    Recur = 0,
                    Reason = "Intro lecture",
                    PurposeTypeId = 1,
                    //ScheduleId = schedule1Id,
                    CreatedAt = new DateTimeOffset(2025, 1, 10, 0, 0, 0, TimeSpan.Zero),
                    CreatedBy = user1Id
                },
                new Booking
                {
                    Id = booking2Id,
                    LabRoomId = 2,
                    StartTime = new DateTimeOffset(2025, 2, 2, 13, 0, 0, TimeSpan.Zero),
                    EndTime = new DateTimeOffset(2025, 2, 2, 15, 0, 0, TimeSpan.Zero),
                    SlotTypeId = 2,
                    BookingStatus = BookingStatus.PendingApproval,
                    BookingType = (BookingType)0,
                    StudentCount = 20,
                    Recur = 0,
                    Reason = "Practical session",
                    PurposeTypeId = 2,
                    //ScheduleId = schedule2Id,
                    CreatedAt = new DateTimeOffset(2025, 1, 11, 0, 0, 0, TimeSpan.Zero),
                    CreatedBy = user2Id
                },
                new Booking
                {
                    Id = booking3Id,
                    LabRoomId = 3,
                    StartTime = new DateTimeOffset(2025, 2, 3, 9, 0, 0, TimeSpan.Zero),
                    EndTime = new DateTimeOffset(2025, 2, 3, 10, 30, 0, TimeSpan.Zero),
                    SlotTypeId = 3,
                    BookingStatus = BookingStatus.Approved,
                    BookingType = (BookingType)0,
                    StudentCount = 10,
                    Recur = 0,
                    Reason = "Workshop",
                    PurposeTypeId = 3,
                    //ScheduleId = schedule3Id,
                    CreatedAt = new DateTimeOffset(2025, 1, 12, 0, 0, 0, TimeSpan.Zero),
                    CreatedBy = user3Id
                }
            );

            // BookingGroups
            modelBuilder.Entity<BookingGroup>().HasData(
                new BookingGroup { Id = bookingGroup1Id, BookingId = booking1Id, GroupId = group1Id },
                new BookingGroup { Id = bookingGroup2Id, BookingId = booking2Id, GroupId = group2Id },
                new BookingGroup { Id = bookingGroup3Id, BookingId = booking3Id, GroupId = group3Id }
            );

            // BookingRequests
            modelBuilder.Entity<BookingRequest>().HasData(
                new BookingRequest { Id = bookingRequest1Id, BookingId = booking1Id, RequestedByUserId = user1Id, ResponsedByUserId = null, BookingRequestStatus = BookingRequestStatus.Pending, ResponseContext = null, CreatedAt = new DateTimeOffset(2025, 1, 13, 0, 0, 0, TimeSpan.Zero), CreatedBy = user1Id },
                new BookingRequest { Id = bookingRequest2Id, BookingId = booking2Id, RequestedByUserId = user2Id, ResponsedByUserId = null, BookingRequestStatus = BookingRequestStatus.Pending, ResponseContext = null, CreatedAt = new DateTimeOffset(2025, 1, 14, 0, 0, 0, TimeSpan.Zero), CreatedBy = user2Id },
                new BookingRequest { Id = bookingRequest3Id, BookingId = booking3Id, RequestedByUserId = user3Id, ResponsedByUserId = null, BookingRequestStatus = BookingRequestStatus.Pending, ResponseContext = null, CreatedAt = new DateTimeOffset(2025, 1, 15, 0, 0, 0, TimeSpan.Zero), CreatedBy = user3Id }
            );

            // Attendances
            modelBuilder.Entity<Attendance>().HasData(
                new Attendance { Id = attendance1Id, ScheduleId = schedule1Id, UserId = user2Id, CheckInTime = null, CheckOutTime = null, CheckInMethod = AttendanceCheckInMethod.FaceId, AttendanceStatus = AttendanceStatus.NotYet, CreatedAt = new DateTimeOffset(2025, 1, 16, 0, 0, 0, TimeSpan.Zero), CreatedBy = user1Id },
                new Attendance { Id = attendance2Id, ScheduleId = schedule2Id, UserId = user3Id, CheckInTime = null, CheckOutTime = null, CheckInMethod = AttendanceCheckInMethod.QR, AttendanceStatus = AttendanceStatus.NotYet, CreatedAt = new DateTimeOffset(2025, 1, 17, 0, 0, 0, TimeSpan.Zero), CreatedBy = user2Id },
                new Attendance { Id = attendance3Id, ScheduleId = schedule3Id, UserId = user1Id, CheckInTime = null, CheckOutTime = null, CheckInMethod = AttendanceCheckInMethod.Manual, AttendanceStatus = AttendanceStatus.NotYet, CreatedAt = new DateTimeOffset(2025, 1, 18, 0, 0, 0, TimeSpan.Zero), CreatedBy = user3Id }
            );

            // EmailTemplates (int PK)
            modelBuilder.Entity<EmailTemplate>().HasData(
                new EmailTemplate { Id = 1, Content = "<h1>Booking Confirmed</h1>", Type = (EmailType)0 },
                new EmailTemplate { Id = 2, Content = "<h1>Booking Rejected</h1>", Type = (EmailType)0 },
                new EmailTemplate { Id = 3, Content = "<h1>Attendance Report</h1>", Type = (EmailType)0 }
            );

            // Schedules
            modelBuilder.Entity<Schedule>().HasData(
                new Schedule { Id = schedule1Id, LecturerId = user1Id, LabRoomId = 1, BookingId = booking1Id, GroupId = group1Id, SlotTypeId = 1, ScheduleType = ScheduleType.Booking, ScheduleStatus = ScheduleStatus.Active, StudentCount = 10, StartTime = new DateTimeOffset(2025, 2, 1, 8, 0, 0, TimeSpan.Zero), EndTime = new DateTimeOffset(2025, 2, 1, 10, 0, 0, TimeSpan.Zero), CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero), CreatedBy = user1Id, IsActive = true, IsDeleted = false },
                new Schedule { Id = schedule2Id, LecturerId = user2Id, LabRoomId = 2, BookingId = booking2Id, GroupId = group2Id, SlotTypeId = 2, ScheduleType = ScheduleType.Booking, ScheduleStatus = ScheduleStatus.Active, StudentCount = 20, StartTime = new DateTimeOffset(2025, 2, 2, 13, 0, 0, TimeSpan.Zero), EndTime = new DateTimeOffset(2025, 2, 2, 15, 0, 0, TimeSpan.Zero), CreatedAt = new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero), CreatedBy = user2Id, IsActive = true, IsDeleted = false },
                new Schedule { Id = schedule3Id, LecturerId = user3Id, LabRoomId = 3, BookingId = booking3Id, GroupId = group3Id, SlotTypeId = 3, ScheduleType = ScheduleType.Academic, ScheduleStatus = ScheduleStatus.Active, StudentCount = 10, StartTime = new DateTimeOffset(2025, 2, 3, 9, 0, 0, TimeSpan.Zero), EndTime = new DateTimeOffset(2025, 2, 3, 10, 30, 0, TimeSpan.Zero), CreatedAt = new DateTimeOffset(2025, 1, 3, 0, 0, 0, TimeSpan.Zero), CreatedBy = user3Id, IsActive = true, IsDeleted = false }
            );

            // Reports
            modelBuilder.Entity<Report>().HasData(
                new Report { Id = report1Id, ScheduleId = schedule1Id, ReportType = (ReportType)0, Description = "Projector not working", IsResolved = false, CreatedAt = new DateTimeOffset(2025, 1, 20, 0, 0, 0, TimeSpan.Zero), CreatedBy = user1Id },
                new Report { Id = report2Id, ScheduleId = schedule2Id, ReportType = (ReportType)0, Description = "Broken chair", IsResolved = false, CreatedAt = new DateTimeOffset(2025, 1, 21, 0, 0, 0, TimeSpan.Zero), CreatedBy = user2Id },
                new Report { Id = report3Id, ScheduleId = schedule3Id, ReportType = (ReportType)0, Description = "AC not cooling", IsResolved = false, CreatedAt = new DateTimeOffset(2025, 1, 22, 0, 0, 0, TimeSpan.Zero), CreatedBy = user3Id }
            );

            // ReportImages
            modelBuilder.Entity<ReportImage>().HasData(
                new ReportImage { Id = reportImage1Id, ReportId = report1Id, ImageUrl = "https://cdn.example/report1.jpg", Size = 1200, FileType = (FileType)0 },
                new ReportImage { Id = reportImage2Id, ReportId = report2Id, ImageUrl = "https://cdn.example/report2.jpg", Size = 800, FileType = (FileType)0 },
                new ReportImage { Id = reportImage3Id, ReportId = report3Id, ImageUrl = "https://cdn.example/report3.jpg", Size = 600, FileType = (FileType)0 }
            );
        }
    }
}
