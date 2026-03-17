using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlterSeedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampusName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CampusImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurposeTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PurposeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurposeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    RoleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampusId = table.Column<int>(type: "integer", nullable: false),
                    BuildingName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BuildingImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Buildings_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlotTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CampusId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlotTypes_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProviderId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CampusId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BuildingId = table.Column<int>(type: "integer", nullable: false),
                    RoomName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RoomNo = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OverrideNumber = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    HasEquipment = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabRooms_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SlotFrames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SlotTypeId = table.Column<int>(type: "integer", nullable: false),
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
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
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
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LabRoomId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    FileType = table.Column<int>(type: "integer", nullable: false),
                    IsAvatar = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabImages_LabRooms_LabRoomId",
                        column: x => x.LabRoomId,
                        principalTable: "LabRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabOwners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabRoomId = table.Column<int>(type: "integer", nullable: false)
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
                name: "RoomPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LabRoomId = table.Column<int>(type: "integer", nullable: false),
                    PolicyKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PolicyValue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomPolicies_LabRooms_LabRoomId",
                        column: x => x.LabRoomId,
                        principalTable: "LabRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckInTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CheckOutTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CheckInMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AttendanceStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponsedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    BookingRequestStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ResponseContext = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingRequests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingRequests_Users_ResponsedByUserId",
                        column: x => x.ResponsedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LabRoomId = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SlotTypeId = table.Column<int>(type: "integer", nullable: false),
                    BookingStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BookingType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StudentCount = table.Column<int>(type: "integer", nullable: false),
                    Recur = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PurposeTypeId = table.Column<int>(type: "integer", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_LabRooms_LabRoomId",
                        column: x => x.LabRoomId,
                        principalTable: "LabRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_PurposeTypes_PurposeTypeId",
                        column: x => x.PurposeTypeId,
                        principalTable: "PurposeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_SlotTypes_SlotTypeId",
                        column: x => x.SlotTypeId,
                        principalTable: "SlotTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LecturerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabRoomId = table.Column<int>(type: "integer", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: true),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    SlotTypeId = table.Column<int>(type: "integer", nullable: false),
                    CalendarEventId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ScheduleType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ScheduleStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StudentCount = table.Column<int>(type: "integer", nullable: false),
                    SubjectCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_LabRooms_LabRoomId",
                        column: x => x.LabRoomId,
                        principalTable: "LabRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_SlotTypes_SlotTypeId",
                        column: x => x.SlotTypeId,
                        principalTable: "SlotTypes",
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
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "ReportImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    FileType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportImages_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                table: "Bookings",
                columns: new[] { "Id", "BookingStatus", "BookingType", "CreatedAt", "CreatedBy", "EndTime", "LabRoomId", "PurposeTypeId", "Reason", "ScheduleId", "SlotTypeId", "StartTime", "StudentCount", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), "PendingApproval", "0", new DateTimeOffset(new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), new DateTimeOffset(new DateTime(2025, 2, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 1, "Intro lecture", null, 1, new DateTimeOffset(new DateTime(2025, 2, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 10, null, null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "PendingApproval", "0", new DateTimeOffset(new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("22222222-2222-2222-2222-222222222222"), new DateTimeOffset(new DateTime(2025, 2, 2, 15, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 2, "Practical session", null, 2, new DateTimeOffset(new DateTime(2025, 2, 2, 13, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 20, null, null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Approved", "0", new DateTimeOffset(new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333333"), new DateTimeOffset(new DateTime(2025, 2, 3, 10, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 3, "Workshop", null, 3, new DateTimeOffset(new DateTime(2025, 2, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 10, null, null }
                });

            migrationBuilder.InsertData(
                table: "GroupMembers",
                columns: new[] { "Id", "GroupId", "UserId" },
                values: new object[,]
                {
                    { new Guid("36363636-3636-3636-3636-363636363636"), new Guid("18181818-1818-1818-1818-181818181818"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("37373737-3737-3737-3737-373737373737"), new Guid("19191919-1919-1919-1919-191919191919"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("38383838-3838-3838-3838-383838383838"), new Guid("20202020-2020-2020-2020-202020202020"), new Guid("11111111-1111-1111-1111-111111111111") }
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
                table: "RoomPolicies",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "LabRoomId", "PolicyKey", "PolicyValue", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("30303030-3030-3030-3030-303030303030"), new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), true, 1, "MaxCapacity", "30", null, null },
                    { new Guid("31313131-3131-3131-3131-313131313131"), new DateTimeOffset(new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), true, 2, "Projector", "Required", null, null },
                    { new Guid("32323232-3232-3232-3232-323232323232"), new DateTimeOffset(new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), true, 3, "FoodAllowed", "No", null, null }
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
                table: "BookingRequests",
                columns: new[] { "Id", "BookingId", "BookingRequestStatus", "CreatedAt", "CreatedBy", "RequestedByUserId", "ResponseContext", "ResponsedByUserId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("12121212-1212-1212-1212-121212121212"), new Guid("44444444-4444-4444-4444-444444444444"), "Pending", new DateTimeOffset(new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), null, null, null, null },
                    { new Guid("13131313-1313-1313-1313-131313131313"), new Guid("55555555-5555-5555-5555-555555555555"), "Pending", new DateTimeOffset(new DateTime(2025, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("22222222-2222-2222-2222-222222222222"), null, null, null, null },
                    { new Guid("14141414-1414-1414-1414-141414141414"), new Guid("66666666-6666-6666-6666-666666666666"), "Pending", new DateTimeOffset(new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("33333333-3333-3333-3333-333333333333"), null, null, null, null }
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

            migrationBuilder.InsertData(
                table: "Reports",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "ReportType", "ScheduleId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("21212121-2121-2121-2121-212121212121"), new DateTimeOffset(new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), "Projector not working", "0", new Guid("27272727-2727-2727-2727-272727272727"), null, null },
                    { new Guid("22222222-2222-2222-2222-222222222221"), new DateTimeOffset(new DateTime(2025, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("22222222-2222-2222-2222-222222222222"), "Broken chair", "0", new Guid("28282828-2828-2828-2828-282828282828"), null, null },
                    { new Guid("23232323-2323-2323-2323-232323232323"), new DateTimeOffset(new DateTime(2025, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333333"), "AC not cooling", "0", new Guid("29292929-2929-2929-2929-292929292929"), null, null }
                });

            migrationBuilder.InsertData(
                table: "ReportImages",
                columns: new[] { "Id", "FileType", "ImageUrl", "ReportId", "Size" },
                values: new object[,]
                {
                    { new Guid("24242424-2424-2424-2424-242424242424"), 0, "https://cdn.example/report1.jpg", new Guid("21212121-2121-2121-2121-212121212121"), 1200 },
                    { new Guid("25252525-2525-2525-2525-252525252525"), 0, "https://cdn.example/report2.jpg", new Guid("22222222-2222-2222-2222-222222222221"), 800 },
                    { new Guid("26262626-2626-2626-2626-262626262626"), 0, "https://cdn.example/report3.jpg", new Guid("23232323-2323-2323-2323-232323232323"), 600 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_AttendanceStatus",
                table: "Attendances",
                column: "AttendanceStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ScheduleId",
                table: "Attendances",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_UserId",
                table: "Attendances",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_Attendance_Booking_User",
                table: "Attendances",
                columns: new[] { "ScheduleId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingGroup_GroupId",
                table: "BookingGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "UQ_Booking_Group",
                table: "BookingGroups",
                columns: new[] { "BookingId", "GroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_BookingId",
                table: "BookingRequests",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_BookingRequestStatus",
                table: "BookingRequests",
                column: "BookingRequestStatus");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_RequestedByUserId",
                table: "BookingRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_ResponsedByUserId",
                table: "BookingRequests",
                column: "ResponsedByUserId",
                filter: "\"ResponsedByUserId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Room_Time",
                table: "Bookings",
                columns: new[] { "LabRoomId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingStatus",
                table: "Bookings",
                column: "BookingStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PurposeTypeId",
                table: "Bookings",
                column: "PurposeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ScheduleId",
                table: "Bookings",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SlotTypeId",
                table: "Bookings",
                column: "SlotTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_CampusId",
                table: "Buildings",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "UQ_Building_Campus_Name",
                table: "Buildings",
                columns: new[] { "CampusId", "BuildingName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campuses_IsActive",
                table: "Campuses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UQ_Campus_Name",
                table: "Campuses",
                column: "CampusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupMember_GroupId",
                table: "GroupMembers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMember_UserId",
                table: "GroupMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_Group_User_Member",
                table: "GroupMembers",
                columns: new[] { "GroupId", "UserId" },
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
                name: "IX_LabImage_LabRoomId",
                table: "LabImages",
                column: "LabRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_LabImage_Room_Avatar",
                table: "LabImages",
                columns: new[] { "LabRoomId", "IsAvatar" });

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
                name: "IX_LabRooms_BuildingId",
                table: "LabRooms",
                column: "BuildingId");

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
                name: "UQ_PurposeType_Name",
                table: "PurposeTypes",
                column: "PurposeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportImage_ReportId",
                table: "ReportImages",
                column: "ReportId");

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
                name: "IX_roles_RoleName",
                table: "roles",
                column: "RoleName",
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
                name: "UQ_Room_PolicyKey",
                table: "RoomPolicies",
                columns: new[] { "LabRoomId", "PolicyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_BookingId",
                table: "Schedules",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_GroupId",
                table: "Schedules",
                column: "GroupId");

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
                name: "IX_Schedules_SlotTypeId",
                table: "Schedules",
                column: "SlotTypeId");

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
                name: "IX_UserRole_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CampusId",
                table: "Users",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FullName",
                table: "Users",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "UQ_User_Code",
                table: "Users",
                column: "UserCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_User_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Schedules_ScheduleId",
                table: "Attendances",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingGroups_Bookings_BookingId",
                table: "BookingGroups",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_Bookings_BookingId",
                table: "BookingRequests",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Schedules_ScheduleId",
                table: "Bookings",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Schedules_ScheduleId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "BookingGroups");

            migrationBuilder.DropTable(
                name: "BookingRequests");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "LabImages");

            migrationBuilder.DropTable(
                name: "LabOwners");

            migrationBuilder.DropTable(
                name: "ReportImages");

            migrationBuilder.DropTable(
                name: "RoomPolicies");

            migrationBuilder.DropTable(
                name: "SlotFrames");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "LabRooms");

            migrationBuilder.DropTable(
                name: "PurposeTypes");

            migrationBuilder.DropTable(
                name: "SlotTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "Campuses");
        }
    }
}
