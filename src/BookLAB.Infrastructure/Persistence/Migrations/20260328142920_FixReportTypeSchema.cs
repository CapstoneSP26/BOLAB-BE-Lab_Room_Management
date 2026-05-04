using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixReportTypeSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReportTypes",
                keyColumn: "ReportTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ReportTypes",
                keyColumn: "ReportTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ReportTypes",
                keyColumn: "ReportTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ReportTypes",
                keyColumn: "ReportTypeId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ReportTypes",
                keyColumn: "ReportTypeId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ReportTypes",
                keyColumn: "ReportTypeId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ReportTypes",
                keyColumn: "ReportTypeId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ReportTypes",
                keyColumn: "ReportTypeId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: new Guid("21212121-2121-2121-2121-212121212121"));

            migrationBuilder.DeleteData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"));

            migrationBuilder.DeleteData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: new Guid("23232323-2323-2323-2323-232323232323"));

            migrationBuilder.DeleteData(
                table: "ReportTypes",
                keyColumn: "ReportTypeId",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "Campuses",
                columns: new[] { "Id", "Address", "CampusImageUrl", "CampusName", "IsActive" },
                values: new object[,]
                {
                    { 1, "1 University Ave", null, "Main Campus", true },
                    { 2, "100 North St", null, "North Campus", true },
                    { 3, "50 West Blvd", null, "West Campus", true }
                });

            migrationBuilder.InsertData(
                table: "EmailTemplates",
                columns: new[] { "Id", "Content", "Type" },
                values: new object[,]
                {
                    { 1, "<h1>Booking Confirmed</h1>", "0" },
                    { 2, "<h1>Booking Rejected</h1>", "0" },
                    { 3, "<h1>Attendance Report</h1>", "0" }
                });

            migrationBuilder.InsertData(
                table: "PurposeTypes",
                columns: new[] { "Id", "PurposeName" },
                values: new object[,]
                {
                    { 1, "Lecture" },
                    { 2, "Practical" },
                    { 3, "Workshop" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Manager" },
                    { 3, "Lecturer" }
                });

            migrationBuilder.InsertData(
                table: "Buildings",
                columns: new[] { "Id", "BuildingImageUrl", "BuildingName", "CampusId", "Description" },
                values: new object[,]
                {
                    { 1, null, "Science Building", 1, "Science faculty building" },
                    { 2, null, "Engineering Building", 1, "Engineering labs" },
                    { 3, null, "Admin Building", 2, "Administration" }
                });

            migrationBuilder.InsertData(
                table: "SlotTypes",
                columns: new[] { "Id", "CampusId", "Code", "Name" },
                values: new object[,]
                {
                    { 1, 1, "S90", "90-min slot" },
                    { 2, 1, "S120", "120-min slot" },
                    { 3, 2, "S45", "45-min slot" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CampusId", "CreatedAt", "CreatedBy", "Email", "FullName", "IsActive", "Provider", "ProviderId", "UpdatedAt", "UpdatedBy", "UserCode", "UserImageUrl" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 1, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "alice@example.edu", "Alice Tran", true, null, null, null, null, "AliceT", "" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 1, new DateTimeOffset(new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "bob@example.edu", "Bob Nguyen", true, null, null, null, null, "BobN", "" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), 2, new DateTimeOffset(new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "carol@example.edu", "Carol Le", true, null, null, null, null, "CarolL", "" }
                });

            migrationBuilder.InsertData(
                table: "BookingRequests",
                columns: new[] { "Id", "BookingId", "BookingRequestStatus", "CreatedAt", "CreatedBy", "RequestedByUserId", "ResponseContext", "ResponsedByUserId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("12121212-1212-1212-1212-121212121212"), new Guid("44444444-4444-4444-4444-444444444444"), "Pending", new DateTimeOffset(new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), null, null, null, null },
                    { new Guid("13131313-1313-1313-1313-131313131313"), new Guid("55555555-5555-5555-5555-555555555555"), "Pending", new DateTimeOffset(new DateTime(2025, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("22222222-2222-2222-2222-222222222222"), null, null, null, null },
                    { new Guid("14141414-1414-1414-1414-141414141414"), new Guid("66666666-6666-6666-6666-666666666666"), "Pending", new DateTimeOffset(new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("33333333-3333-3333-3333-333333333333"), null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "GroupName", "OwnerId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("18181818-1818-1818-1818-181818181818"), new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), "Team Alpha", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("19191919-1919-1919-1919-191919191919"), new DateTimeOffset(new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("22222222-2222-2222-2222-222222222222"), "Team Beta", new Guid("22222222-2222-2222-2222-222222222222"), null, null },
                    { new Guid("20202020-2020-2020-2020-202020202020"), new DateTimeOffset(new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333333"), "Team Gamma", new Guid("33333333-3333-3333-3333-333333333333"), null, null }
                });

            migrationBuilder.InsertData(
                table: "LabRooms",
                columns: new[] { "Id", "BuildingId", "CreatedAt", "CreatedBy", "Description", "HasEquipment", "IsActive", "Location", "RoomName", "RoomNo", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 1, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), "General purpose lab", true, true, "Floor 1", "Lab A1", "Gamma 101", null, null },
                    { 2, 1, new DateTimeOffset(new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), "Hardware lab", true, true, "Floor 2", "Lab A2", "Gamma 102", null, null }
                });

            migrationBuilder.InsertData(
                table: "LabRooms",
                columns: new[] { "Id", "BuildingId", "CreatedAt", "CreatedBy", "Description", "IsActive", "Location", "RoomName", "RoomNo", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 3, 2, new DateTimeOffset(new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), "Software lab", true, "Floor 3", "Lab B1", "Alpha 101", null, null });

            migrationBuilder.InsertData(
                table: "SlotFrames",
                columns: new[] { "Id", "EndTimeSlot", "OrderIndex", "SlotTypeId", "StartTimeSlot" },
                values: new object[,]
                {
                    { 1, new TimeOnly(9, 30, 0), 1, 1, new TimeOnly(8, 0, 0) },
                    { 2, new TimeOnly(11, 15, 0), 2, 1, new TimeOnly(9, 45, 0) },
                    { 3, new TimeOnly(15, 0, 0), 1, 2, new TimeOnly(13, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 2, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 3, new Guid("33333333-3333-3333-3333-333333333333") }
                });

            migrationBuilder.InsertData(
                table: "BookingGroups",
                columns: new[] { "Id", "BookingId", "GroupId" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("18181818-1818-1818-1818-181818181818") },
                    { new Guid("88888888-8888-8888-8888-888888888888"), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("19191919-1919-1919-1919-191919191919") },
                    { new Guid("99999999-9999-9999-9999-999999999999"), new Guid("66666666-6666-6666-6666-666666666666"), new Guid("20202020-2020-2020-2020-202020202020") }
                });

            migrationBuilder.InsertData(
                table: "GroupMembers",
                columns: new[] { "Id", "GroupId", "SubjectCode", "UserId" },
                values: new object[,]
                {
                    { new Guid("36363636-3636-3636-3636-363636363636"), new Guid("18181818-1818-1818-1818-181818181818"), "", new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("37373737-3737-3737-3737-373737373737"), new Guid("19191919-1919-1919-1919-191919191919"), "", new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("38383838-3838-3838-3838-383838383838"), new Guid("20202020-2020-2020-2020-202020202020"), "", new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "LabImages",
                columns: new[] { "Id", "FileType", "ImageUrl", "IsAvatar", "LabRoomId", "Size" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333330"), 0, "https://cdn.example/room1.jpg", true, 1, 1024 });

            migrationBuilder.InsertData(
                table: "LabImages",
                columns: new[] { "Id", "FileType", "ImageUrl", "LabRoomId", "Size" },
                values: new object[,]
                {
                    { new Guid("34343434-3434-3434-3434-343434343434"), 0, "https://cdn.example/room2.jpg", 2, 2048 },
                    { new Guid("35353535-3535-3535-3535-353535353535"), 0, "https://cdn.example/room3.jpg", 3, 512 }
                });

            migrationBuilder.InsertData(
                table: "LabOwners",
                columns: new[] { "Id", "LabRoomId", "UserId" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 1, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 2, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 3, new Guid("33333333-3333-3333-3333-333333333333") }
                });

            migrationBuilder.InsertData(
                table: "Schedules",
                columns: new[] { "Id", "BookingId", "CalendarEventId", "CreatedAt", "CreatedBy", "EndTime", "GroupId", "IsActive", "LabRoomId", "LecturerId", "ScheduleStatus", "ScheduleType", "SlotTypeId", "StartTime", "StudentCount", "SubjectCode", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("27272727-2727-2727-2727-272727272727"), new Guid("44444444-4444-4444-4444-444444444444"), null, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), new DateTimeOffset(new DateTime(2025, 2, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("18181818-1818-1818-1818-181818181818"), true, 1, new Guid("11111111-1111-1111-1111-111111111111"), "Active", "Booking", 1, new DateTimeOffset(new DateTime(2025, 2, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 10, null, null, null },
                    { new Guid("28282828-2828-2828-2828-282828282828"), new Guid("55555555-5555-5555-5555-555555555555"), null, new DateTimeOffset(new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("22222222-2222-2222-2222-222222222222"), new DateTimeOffset(new DateTime(2025, 2, 2, 15, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("19191919-1919-1919-1919-191919191919"), true, 2, new Guid("22222222-2222-2222-2222-222222222222"), "Active", "Booking", 2, new DateTimeOffset(new DateTime(2025, 2, 2, 13, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 20, null, null, null },
                    { new Guid("29292929-2929-2929-2929-292929292929"), new Guid("66666666-6666-6666-6666-666666666666"), null, new DateTimeOffset(new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333333"), new DateTimeOffset(new DateTime(2025, 2, 3, 10, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("20202020-2020-2020-2020-202020202020"), true, 3, new Guid("33333333-3333-3333-3333-333333333333"), "Active", "Academic", 3, new DateTimeOffset(new DateTime(2025, 2, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 10, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Attendances",
                columns: new[] { "Id", "AttendanceStatus", "CheckInMethod", "CheckInTime", "CheckOutTime", "CreatedAt", "CreatedBy", "ScheduleId", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("15151515-1515-1515-1515-151515151515"), "NotYet", "FaceId", null, null, new DateTimeOffset(new DateTime(2025, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("27272727-2727-2727-2727-272727272727"), null, null, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("16161616-1616-1616-1616-161616161616"), "NotYet", "QR", null, null, new DateTimeOffset(new DateTime(2025, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("28282828-2828-2828-2828-282828282828"), null, null, new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("17171717-1717-1717-1717-171717171717"), "NotYet", "Manual", null, null, new DateTimeOffset(new DateTime(2025, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("29292929-2929-2929-2929-292929292929"), null, null, new Guid("11111111-1111-1111-1111-111111111111") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: new Guid("15151515-1515-1515-1515-151515151515"));

            migrationBuilder.DeleteData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: new Guid("16161616-1616-1616-1616-161616161616"));

            migrationBuilder.DeleteData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: new Guid("17171717-1717-1717-1717-171717171717"));

            migrationBuilder.DeleteData(
                table: "BookingGroups",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "BookingGroups",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "BookingGroups",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                table: "BookingRequests",
                keyColumn: "Id",
                keyValue: new Guid("12121212-1212-1212-1212-121212121212"));

            migrationBuilder.DeleteData(
                table: "BookingRequests",
                keyColumn: "Id",
                keyValue: new Guid("13131313-1313-1313-1313-131313131313"));

            migrationBuilder.DeleteData(
                table: "BookingRequests",
                keyColumn: "Id",
                keyValue: new Guid("14141414-1414-1414-1414-141414141414"));

            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Campuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "GroupMembers",
                keyColumn: "Id",
                keyValue: new Guid("36363636-3636-3636-3636-363636363636"));

            migrationBuilder.DeleteData(
                table: "GroupMembers",
                keyColumn: "Id",
                keyValue: new Guid("37373737-3737-3737-3737-373737373737"));

            migrationBuilder.DeleteData(
                table: "GroupMembers",
                keyColumn: "Id",
                keyValue: new Guid("38383838-3838-3838-3838-383838383838"));

            migrationBuilder.DeleteData(
                table: "LabImages",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333330"));

            migrationBuilder.DeleteData(
                table: "LabImages",
                keyColumn: "Id",
                keyValue: new Guid("34343434-3434-3434-3434-343434343434"));

            migrationBuilder.DeleteData(
                table: "LabImages",
                keyColumn: "Id",
                keyValue: new Guid("35353535-3535-3535-3535-353535353535"));

            migrationBuilder.DeleteData(
                table: "LabOwners",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "LabOwners",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "LabOwners",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "PurposeTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PurposeTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PurposeTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SlotFrames",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SlotFrames",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SlotFrames",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 3, new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("27272727-2727-2727-2727-272727272727"));

            migrationBuilder.DeleteData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("28282828-2828-2828-2828-282828282828"));

            migrationBuilder.DeleteData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("29292929-2929-2929-2929-292929292929"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("18181818-1818-1818-1818-181818181818"));

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("19191919-1919-1919-1919-191919191919"));

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("20202020-2020-2020-2020-202020202020"));

            migrationBuilder.DeleteData(
                table: "LabRooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LabRooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LabRooms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SlotTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SlotTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SlotTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Campuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Campuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.InsertData(
                table: "ReportTypes",
                columns: new[] { "ReportTypeId", "ReportTypeName" },
                values: new object[,]
                {
                    { 1, "Thiết bị hư hỏng" },
                    { 2, "Thiết bị mất" },
                    { 3, "Vấn đề vệ sinh" },
                    { 4, "Điều hòa không hoạt động" },
                    { 5, "Vấn đề chiếu sáng" },
                    { 6, "Bàn ghế hư hỏng" },
                    { 7, "Mất kết nối mạng" },
                    { 8, "Khóa cửa hỏng" },
                    { 9, "Khác" }
                });

            migrationBuilder.InsertData(
                table: "Reports",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "ReportTypeId", "ScheduleId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("21212121-2121-2121-2121-212121212121"), new DateTimeOffset(new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), "Projector not working", 1, new Guid("27272727-2727-2727-2727-272727272727"), null, null },
                    { new Guid("22222222-2222-2222-2222-222222222221"), new DateTimeOffset(new DateTime(2025, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("22222222-2222-2222-2222-222222222222"), "Broken chair", 1, new Guid("28282828-2828-2828-2828-282828282828"), null, null },
                    { new Guid("23232323-2323-2323-2323-232323232323"), new DateTimeOffset(new DateTime(2025, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333333"), "AC not cooling", 1, new Guid("29292929-2929-2929-2929-292929292929"), null, null }
                });
        }
    }
}
