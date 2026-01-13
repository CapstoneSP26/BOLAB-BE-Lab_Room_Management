using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatev12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_booking_groups_bookings_BookingId",
                table: "booking_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_booking_groups_bookings_BookingId1",
                table: "booking_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_booking_groups_student_groups_GroupId",
                table: "booking_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_booking_requests_bookings_BookingId",
                table: "booking_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_booking_requests_users_ApprovedByUserId",
                table: "booking_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_booking_requests_users_RequestedByUserId",
                table: "booking_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_bookings_lab_rooms_RoomId",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_bookings_semesters_SemesterId",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_bookings_slot_types_SlotTypeId",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_bookings_users_BookedByUserId",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_buildings_campuses_CampusId",
                table: "buildings");

            migrationBuilder.DropForeignKey(
                name: "FK_group_members_student_groups_GroupId",
                table: "group_members");

            migrationBuilder.DropForeignKey(
                name: "FK_group_members_users_UserId",
                table: "group_members");

            migrationBuilder.DropForeignKey(
                name: "FK_lab_rooms_buildings_BuildingId",
                table: "lab_rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_lab_rooms_users_ManagerUserId",
                table: "lab_rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_room_policies_lab_rooms_LabRoomId",
                table: "room_policies");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_RoleId",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_UserId",
                table: "user_roles");

            migrationBuilder.DropTable(
                name: "attendance_details");

            migrationBuilder.DropTable(
                name: "attendance_summaries");

            migrationBuilder.DropTable(
                name: "booking_users");

            migrationBuilder.DropTable(
                name: "equipment_maintenances");

            migrationBuilder.DropTable(
                name: "feedbacks");

            migrationBuilder.DropTable(
                name: "required_equipments");

            migrationBuilder.DropTable(
                name: "semesters");

            migrationBuilder.DropTable(
                name: "student_groups");

            migrationBuilder.DropTable(
                name: "equipment_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_StudentId",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_campuses",
                table: "campuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_buildings",
                table: "buildings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_bookings",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "IX_bookings_BookedByUserId",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "IX_bookings_RoomId_StartTime_EndTime",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "IX_bookings_SemesterId",
                table: "bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_slot_types",
                table: "slot_types");

            migrationBuilder.DropIndex(
                name: "IX_slot_types_Code",
                table: "slot_types");

            migrationBuilder.DropPrimaryKey(
                name: "PK_room_policies",
                table: "room_policies");

            migrationBuilder.DropIndex(
                name: "IX_room_policies_LabRoomId",
                table: "room_policies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lab_rooms",
                table: "lab_rooms");

            migrationBuilder.DropIndex(
                name: "IX_lab_rooms_BuildingId_RoomName",
                table: "lab_rooms");

            migrationBuilder.DropIndex(
                name: "IX_lab_rooms_ManagerUserId",
                table: "lab_rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_group_members",
                table: "group_members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_booking_requests",
                table: "booking_requests");

            migrationBuilder.DropIndex(
                name: "IX_booking_requests_ApprovalStatus",
                table: "booking_requests");

            migrationBuilder.DropIndex(
                name: "IX_booking_requests_ApprovedByUserId",
                table: "booking_requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_booking_groups",
                table: "booking_groups");

            migrationBuilder.DropIndex(
                name: "IX_booking_groups_BookingId",
                table: "booking_groups");

            migrationBuilder.DropIndex(
                name: "IX_booking_groups_BookingId1",
                table: "booking_groups");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "NumberOfFloors",
                table: "buildings");

            migrationBuilder.DropColumn(
                name: "BookedByUserId",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "ExpectedParticipantCount",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "IsCourseSchedule",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "ParticipantMode",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "AllowsOverCapacity",
                table: "slot_types");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "slot_types");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "slot_types");

            migrationBuilder.DropColumn(
                name: "IsFixedDuration",
                table: "slot_types");

            migrationBuilder.DropColumn(
                name: "RequiresApproval",
                table: "slot_types");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "lab_rooms");

            migrationBuilder.DropColumn(
                name: "ApprovalNotes",
                table: "booking_requests");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "booking_requests");

            migrationBuilder.DropColumn(
                name: "BookingId1",
                table: "booking_groups");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "campuses",
                newName: "Campuses");

            migrationBuilder.RenameTable(
                name: "buildings",
                newName: "Buildings");

            migrationBuilder.RenameTable(
                name: "bookings",
                newName: "Bookings");

            migrationBuilder.RenameTable(
                name: "user_roles",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "slot_types",
                newName: "SlotTypes");

            migrationBuilder.RenameTable(
                name: "room_policies",
                newName: "RoomPolicies");

            migrationBuilder.RenameTable(
                name: "lab_rooms",
                newName: "LabRooms");

            migrationBuilder.RenameTable(
                name: "group_members",
                newName: "GroupMembers");

            migrationBuilder.RenameTable(
                name: "booking_requests",
                newName: "BookingRequests");

            migrationBuilder.RenameTable(
                name: "booking_groups",
                newName: "BookingGroups");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "Users",
                newName: "UQ_User_Email");

            migrationBuilder.RenameIndex(
                name: "IX_campuses_CampusName",
                table: "Campuses",
                newName: "UQ_Campus_Name");

            migrationBuilder.RenameIndex(
                name: "IX_buildings_CampusId_BuildingName",
                table: "Buildings",
                newName: "UQ_Building_Campus_Name");

            migrationBuilder.RenameColumn(
                name: "SlotTypeId",
                table: "Bookings",
                newName: "PurposeTypeId");

            migrationBuilder.RenameColumn(
                name: "SemesterId",
                table: "Bookings",
                newName: "LabRoomId");

            migrationBuilder.RenameIndex(
                name: "IX_bookings_SlotTypeId",
                table: "Bookings",
                newName: "IX_Bookings_PurposeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_UserId_RoleId",
                table: "UserRoles",
                newName: "UQ_User_Role");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_RoleId",
                table: "UserRoles",
                newName: "IX_UserRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_room_policies_LabRoomId_PolicyKey",
                table: "RoomPolicies",
                newName: "UQ_Room_PolicyKey");

            migrationBuilder.RenameColumn(
                name: "ManagerUserId",
                table: "LabRooms",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "IsSpecialized",
                table: "LabRooms",
                newName: "HasEquipment");

            migrationBuilder.RenameIndex(
                name: "IX_lab_rooms_BuildingId",
                table: "LabRooms",
                newName: "IX_LabRooms_BuildingId");

            migrationBuilder.RenameIndex(
                name: "IX_group_members_UserId",
                table: "GroupMembers",
                newName: "IX_GroupMember_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_group_members_GroupId_UserId",
                table: "GroupMembers",
                newName: "UQ_Group_User_Member");

            migrationBuilder.RenameIndex(
                name: "IX_group_members_GroupId",
                table: "GroupMembers",
                newName: "IX_GroupMember_GroupId");

            migrationBuilder.RenameColumn(
                name: "ApprovedByUserId",
                table: "BookingRequests",
                newName: "ResponsedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_requests_RequestedByUserId",
                table: "BookingRequests",
                newName: "IX_BookingRequests_RequestedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_requests_BookingId",
                table: "BookingRequests",
                newName: "IX_BookingRequests_BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_groups_GroupId",
                table: "BookingGroups",
                newName: "IX_BookingGroup_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_groups_BookingId_GroupId",
                table: "BookingGroups",
                newName: "UQ_Booking_Group");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<Guid>(
                name: "CampusId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserImageUrl",
                table: "Users",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CampusName",
                table: "Campuses",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Campuses",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CampusImageUrl",
                table: "Campuses",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuildingImageUrl",
                table: "Buildings",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Buildings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookingStatus",
                table: "Bookings",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BookingType",
                table: "Bookings",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Bookings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SlotTypes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "CampusId",
                table: "SlotTypes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RoomPolicies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "RoomPolicies",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RoomPolicies",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RoomPolicies",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "RoomPolicies",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RoomName",
                table: "LabRooms",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "LabRooms",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "LabRooms",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LabRooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "OverrideNumber",
                table: "LabRooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LabRooms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "LabRooms",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "BookingRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookingRequestStatus",
                table: "BookingRequests",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResponseContext",
                table: "BookingRequests",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Campuses",
                table: "Campuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Buildings",
                table: "Buildings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SlotTypes",
                table: "SlotTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomPolicies",
                table: "RoomPolicies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabRooms",
                table: "LabRooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMembers",
                table: "GroupMembers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingRequests",
                table: "BookingRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingGroups",
                table: "BookingGroups",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckInMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AttendanceStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendances_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabOwners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabRoomId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabOwners_LabRooms_LabRoomId",
                        column: x => x.LabRoomId,
                        principalTable: "LabRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabOwners_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurposeTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurposeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurposeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LecturerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduleType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ScheduleStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_LabRooms_LabRoomId",
                        column: x => x.LabRoomId,
                        principalTable: "LabRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_Users_LecturerId",
                        column: x => x.LecturerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SlotFrames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTimeSlot = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTimeSlot = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotFrames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlotFrames_SlotTypes_SlotTypeId",
                        column: x => x.SlotTypeId,
                        principalTable: "SlotTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CampusId",
                table: "Users",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FullName",
                table: "Users",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Campuses_IsActive",
                table: "Campuses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_CampusId",
                table: "Buildings",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Room_Time",
                table: "Bookings",
                columns: new[] { "LabRoomId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingStatus",
                table: "Bookings",
                column: "BookingStatus");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotTypes_CampusId",
                table: "SlotTypes",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "UQ_SlotType_Campus_Code",
                table: "SlotTypes",
                columns: new[] { "CampusId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomPolicies_IsActive",
                table: "RoomPolicies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RoomPolicies_PolicyKey",
                table: "RoomPolicies",
                column: "PolicyKey");

            migrationBuilder.CreateIndex(
                name: "IX_LabRooms_IsActive",
                table: "LabRooms",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UQ_LabRoom_Building_Name",
                table: "LabRooms",
                columns: new[] { "BuildingId", "RoomName", "IsDeleted" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_BookingRequestStatus",
                table: "BookingRequests",
                column: "BookingRequestStatus");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_ResponsedByUserId",
                table: "BookingRequests",
                column: "ResponsedByUserId",
                filter: "\"ResponsedByUserId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_AttendanceStatus",
                table: "Attendances",
                column: "AttendanceStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_BookingId",
                table: "Attendances",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_UserId",
                table: "Attendances",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_Attendance_Booking_User",
                table: "Attendances",
                columns: new[] { "BookingId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_GroupName",
                table: "Groups",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_OwnerId",
                table: "Groups",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "UQ_Group_Owner_Name_Active",
                table: "Groups",
                columns: new[] { "OwnerId", "GroupName", "IsDeleted" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_LabOwner_LabRoomId",
                table: "LabOwners",
                column: "LabRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOwner_UserId",
                table: "LabOwners",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_LabOwner_User_Room",
                table: "LabOwners",
                columns: new[] { "UserId", "LabRoomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_PurposeType_Name",
                table: "PurposeTypes",
                column: "PurposeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Report_Unresolved",
                table: "Reports",
                column: "IsResolved",
                filter: "\"IsResolved\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CreatedBy",
                table: "Reports",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportType",
                table: "Reports",
                column: "ReportType");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ScheduleId",
                table: "Reports",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_LabRoomId",
                table: "Schedules",
                column: "LabRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_LecturerId",
                table: "Schedules",
                column: "LecturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ScheduleStatus",
                table: "Schedules",
                column: "ScheduleStatus");

            migrationBuilder.CreateIndex(
                name: "IX_SlotFrames_SlotTypeId",
                table: "SlotFrames",
                column: "SlotTypeId");

            migrationBuilder.CreateIndex(
                name: "UQ_SlotFrame_Type_Order",
                table: "SlotFrames",
                columns: new[] { "SlotTypeId", "OrderIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_SlotFrame_Type_StartTime",
                table: "SlotFrames",
                columns: new[] { "SlotTypeId", "StartTimeSlot" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingGroups_Bookings_BookingId",
                table: "BookingGroups",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingGroups_Groups_GroupId",
                table: "BookingGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_Bookings_BookingId",
                table: "BookingRequests",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_Users_RequestedByUserId",
                table: "BookingRequests",
                column: "RequestedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_Users_ResponsedByUserId",
                table: "BookingRequests",
                column: "ResponsedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_LabRooms_LabRoomId",
                table: "Bookings",
                column: "LabRoomId",
                principalTable: "LabRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_PurposeTypes_PurposeTypeId",
                table: "Bookings",
                column: "PurposeTypeId",
                principalTable: "PurposeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Campuses_CampusId",
                table: "Buildings",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_Groups_GroupId",
                table: "GroupMembers",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_Users_UserId",
                table: "GroupMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabRooms_Buildings_BuildingId",
                table: "LabRooms",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomPolicies_LabRooms_LabRoomId",
                table: "RoomPolicies",
                column: "LabRoomId",
                principalTable: "LabRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SlotTypes_Campuses_CampusId",
                table: "SlotTypes",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Campuses_CampusId",
                table: "Users",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingGroups_Bookings_BookingId",
                table: "BookingGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingGroups_Groups_GroupId",
                table: "BookingGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingRequests_Bookings_BookingId",
                table: "BookingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingRequests_Users_RequestedByUserId",
                table: "BookingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingRequests_Users_ResponsedByUserId",
                table: "BookingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_LabRooms_LabRoomId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_PurposeTypes_PurposeTypeId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Campuses_CampusId",
                table: "Buildings");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_Groups_GroupId",
                table: "GroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_Users_UserId",
                table: "GroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_LabRooms_Buildings_BuildingId",
                table: "LabRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomPolicies_LabRooms_LabRoomId",
                table: "RoomPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_SlotTypes_Campuses_CampusId",
                table: "SlotTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Campuses_CampusId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "LabOwners");

            migrationBuilder.DropTable(
                name: "PurposeTypes");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "SlotFrames");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CampusId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_FullName",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Campuses",
                table: "Campuses");

            migrationBuilder.DropIndex(
                name: "IX_Campuses_IsActive",
                table: "Campuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Buildings",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_CampusId",
                table: "Buildings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Booking_Room_Time",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_BookingStatus",
                table: "Bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRole_UserId",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SlotTypes",
                table: "SlotTypes");

            migrationBuilder.DropIndex(
                name: "IX_SlotTypes_CampusId",
                table: "SlotTypes");

            migrationBuilder.DropIndex(
                name: "UQ_SlotType_Campus_Code",
                table: "SlotTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomPolicies",
                table: "RoomPolicies");

            migrationBuilder.DropIndex(
                name: "IX_RoomPolicies_IsActive",
                table: "RoomPolicies");

            migrationBuilder.DropIndex(
                name: "IX_RoomPolicies_PolicyKey",
                table: "RoomPolicies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabRooms",
                table: "LabRooms");

            migrationBuilder.DropIndex(
                name: "IX_LabRooms_IsActive",
                table: "LabRooms");

            migrationBuilder.DropIndex(
                name: "UQ_LabRoom_Building_Name",
                table: "LabRooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMembers",
                table: "GroupMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingRequests",
                table: "BookingRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_BookingRequestStatus",
                table: "BookingRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_ResponsedByUserId",
                table: "BookingRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingGroups",
                table: "BookingGroups");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserImageUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CampusImageUrl",
                table: "Campuses");

            migrationBuilder.DropColumn(
                name: "BuildingImageUrl",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "BookingStatus",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BookingType",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "SlotTypes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RoomPolicies");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RoomPolicies");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RoomPolicies");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RoomPolicies");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RoomPolicies");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LabRooms");

            migrationBuilder.DropColumn(
                name: "OverrideNumber",
                table: "LabRooms");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LabRooms");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "LabRooms");

            migrationBuilder.DropColumn(
                name: "BookingRequestStatus",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "ResponseContext",
                table: "BookingRequests");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Campuses",
                newName: "campuses");

            migrationBuilder.RenameTable(
                name: "Buildings",
                newName: "buildings");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "bookings");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "user_roles");

            migrationBuilder.RenameTable(
                name: "SlotTypes",
                newName: "slot_types");

            migrationBuilder.RenameTable(
                name: "RoomPolicies",
                newName: "room_policies");

            migrationBuilder.RenameTable(
                name: "LabRooms",
                newName: "lab_rooms");

            migrationBuilder.RenameTable(
                name: "GroupMembers",
                newName: "group_members");

            migrationBuilder.RenameTable(
                name: "BookingRequests",
                newName: "booking_requests");

            migrationBuilder.RenameTable(
                name: "BookingGroups",
                newName: "booking_groups");

            migrationBuilder.RenameIndex(
                name: "UQ_User_Email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameIndex(
                name: "UQ_Campus_Name",
                table: "campuses",
                newName: "IX_campuses_CampusName");

            migrationBuilder.RenameIndex(
                name: "UQ_Building_Campus_Name",
                table: "buildings",
                newName: "IX_buildings_CampusId_BuildingName");

            migrationBuilder.RenameColumn(
                name: "PurposeTypeId",
                table: "bookings",
                newName: "SlotTypeId");

            migrationBuilder.RenameColumn(
                name: "LabRoomId",
                table: "bookings",
                newName: "SemesterId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_PurposeTypeId",
                table: "bookings",
                newName: "IX_bookings_SlotTypeId");

            migrationBuilder.RenameIndex(
                name: "UQ_User_Role",
                table: "user_roles",
                newName: "IX_user_roles_UserId_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_RoleId",
                table: "user_roles",
                newName: "IX_user_roles_RoleId");

            migrationBuilder.RenameIndex(
                name: "UQ_Room_PolicyKey",
                table: "room_policies",
                newName: "IX_room_policies_LabRoomId_PolicyKey");

            migrationBuilder.RenameColumn(
                name: "HasEquipment",
                table: "lab_rooms",
                newName: "IsSpecialized");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "lab_rooms",
                newName: "ManagerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_LabRooms_BuildingId",
                table: "lab_rooms",
                newName: "IX_lab_rooms_BuildingId");

            migrationBuilder.RenameIndex(
                name: "UQ_Group_User_Member",
                table: "group_members",
                newName: "IX_group_members_GroupId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMember_UserId",
                table: "group_members",
                newName: "IX_group_members_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMember_GroupId",
                table: "group_members",
                newName: "IX_group_members_GroupId");

            migrationBuilder.RenameColumn(
                name: "ResponsedByUserId",
                table: "booking_requests",
                newName: "ApprovedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingRequests_RequestedByUserId",
                table: "booking_requests",
                newName: "IX_booking_requests_RequestedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingRequests_BookingId",
                table: "booking_requests",
                newName: "IX_booking_requests_BookingId");

            migrationBuilder.RenameIndex(
                name: "UQ_Booking_Group",
                table: "booking_groups",
                newName: "IX_booking_groups_BookingId_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingGroup_GroupId",
                table: "booking_groups",
                newName: "IX_booking_groups_GroupId");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(
                name: "StudentId",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CampusName",
                table: "campuses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "campuses",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfFloors",
                table: "buildings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "bookings",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "BookedByUserId",
                table: "bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ExpectedParticipantCount",
                table: "bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCourseSchedule",
                table: "bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantMode",
                table: "bookings",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "bookings",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "bookings",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "slot_types",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<bool>(
                name: "AllowsOverCapacity",
                table: "slot_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "slot_types",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "slot_types",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFixedDuration",
                table: "slot_types",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresApproval",
                table: "slot_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "RoomName",
                table: "lab_rooms",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "lab_rooms",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "lab_rooms",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "lab_rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "booking_requests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalNotes",
                table: "booking_requests",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "booking_requests",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId1",
                table: "booking_groups",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_campuses",
                table: "campuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_buildings",
                table: "buildings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_bookings",
                table: "bookings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_slot_types",
                table: "slot_types",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_room_policies",
                table: "room_policies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lab_rooms",
                table: "lab_rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_group_members",
                table: "group_members",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_booking_requests",
                table: "booking_requests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_booking_groups",
                table: "booking_groups",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "attendance_details",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckInMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CheckOutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attendance_details_bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attendance_details_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "attendance_summaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttendanceMode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecordedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalGroups = table.Column<int>(type: "integer", nullable: true),
                    TotalParticipants = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_summaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attendance_summaries_bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attendance_summaries_users_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "booking_users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_booking_users_bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_booking_users_bookings_BookingId1",
                        column: x => x.BookingId1,
                        principalTable: "bookings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_booking_users_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "equipment_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Condition = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: true),
                    SerialNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_equipment_items_lab_rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "lab_rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "feedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    FeedbackType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_feedbacks_bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_feedbacks_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "semesters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SemesterCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_semesters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "student_groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    GroupName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "equipment_maintenances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    EquipmentItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment_maintenances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_equipment_maintenances_equipment_items_EquipmentItemId",
                        column: x => x.EquipmentItemId,
                        principalTable: "equipment_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_equipment_maintenances_users_ReportedByUserId",
                        column: x => x.ReportedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "required_equipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    EquipmentName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    QuantityRequired = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_required_equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_required_equipments_bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_required_equipments_equipment_items_EquipmentItemId",
                        column: x => x.EquipmentItemId,
                        principalTable: "equipment_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_StudentId",
                table: "users",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bookings_BookedByUserId",
                table: "bookings",
                column: "BookedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_RoomId_StartTime_EndTime",
                table: "bookings",
                columns: new[] { "RoomId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_bookings_SemesterId",
                table: "bookings",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_slot_types_Code",
                table: "slot_types",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_policies_LabRoomId",
                table: "room_policies",
                column: "LabRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_lab_rooms_BuildingId_RoomName",
                table: "lab_rooms",
                columns: new[] { "BuildingId", "RoomName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lab_rooms_ManagerUserId",
                table: "lab_rooms",
                column: "ManagerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_booking_requests_ApprovalStatus",
                table: "booking_requests",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_booking_requests_ApprovedByUserId",
                table: "booking_requests",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_booking_groups_BookingId",
                table: "booking_groups",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_booking_groups_BookingId1",
                table: "booking_groups",
                column: "BookingId1");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_details_BookingId",
                table: "attendance_details",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_details_BookingId_UserId",
                table: "attendance_details",
                columns: new[] { "BookingId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_attendance_details_UserId",
                table: "attendance_details",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_summaries_AttendanceMode",
                table: "attendance_summaries",
                column: "AttendanceMode");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_summaries_BookingId",
                table: "attendance_summaries",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_attendance_summaries_RecordedByUserId",
                table: "attendance_summaries",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_booking_users_BookingId",
                table: "booking_users",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_booking_users_BookingId_UserId",
                table: "booking_users",
                columns: new[] { "BookingId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_booking_users_BookingId1",
                table: "booking_users",
                column: "BookingId1");

            migrationBuilder.CreateIndex(
                name: "IX_booking_users_UserId",
                table: "booking_users",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_items_Condition",
                table: "equipment_items",
                column: "Condition");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_items_IsDeleted",
                table: "equipment_items",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_items_RoomId",
                table: "equipment_items",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_items_SerialNumber",
                table: "equipment_items",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_equipment_maintenances_EquipmentItemId",
                table: "equipment_maintenances",
                column: "EquipmentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_maintenances_ReportedByUserId",
                table: "equipment_maintenances",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_maintenances_Status",
                table: "equipment_maintenances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_BookingId",
                table: "feedbacks",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_FeedbackType",
                table: "feedbacks",
                column: "FeedbackType");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_IsResolved",
                table: "feedbacks",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_UserId",
                table: "feedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_required_equipments_BookingId",
                table: "required_equipments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_required_equipments_EquipmentItemId",
                table: "required_equipments",
                column: "EquipmentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_semesters_SemesterCode",
                table: "semesters",
                column: "SemesterCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_groups_GroupName",
                table: "student_groups",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_student_groups_IsDeleted",
                table: "student_groups",
                column: "IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_booking_groups_bookings_BookingId",
                table: "booking_groups",
                column: "BookingId",
                principalTable: "bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_booking_groups_bookings_BookingId1",
                table: "booking_groups",
                column: "BookingId1",
                principalTable: "bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_booking_groups_student_groups_GroupId",
                table: "booking_groups",
                column: "GroupId",
                principalTable: "student_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_booking_requests_bookings_BookingId",
                table: "booking_requests",
                column: "BookingId",
                principalTable: "bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_booking_requests_users_ApprovedByUserId",
                table: "booking_requests",
                column: "ApprovedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_booking_requests_users_RequestedByUserId",
                table: "booking_requests",
                column: "RequestedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_lab_rooms_RoomId",
                table: "bookings",
                column: "RoomId",
                principalTable: "lab_rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_semesters_SemesterId",
                table: "bookings",
                column: "SemesterId",
                principalTable: "semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_slot_types_SlotTypeId",
                table: "bookings",
                column: "SlotTypeId",
                principalTable: "slot_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_users_BookedByUserId",
                table: "bookings",
                column: "BookedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_buildings_campuses_CampusId",
                table: "buildings",
                column: "CampusId",
                principalTable: "campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_group_members_student_groups_GroupId",
                table: "group_members",
                column: "GroupId",
                principalTable: "student_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_group_members_users_UserId",
                table: "group_members",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_lab_rooms_buildings_BuildingId",
                table: "lab_rooms",
                column: "BuildingId",
                principalTable: "buildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_lab_rooms_users_ManagerUserId",
                table: "lab_rooms",
                column: "ManagerUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_room_policies_lab_rooms_LabRoomId",
                table: "room_policies",
                column: "LabRoomId",
                principalTable: "lab_rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_RoleId",
                table: "user_roles",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_UserId",
                table: "user_roles",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
