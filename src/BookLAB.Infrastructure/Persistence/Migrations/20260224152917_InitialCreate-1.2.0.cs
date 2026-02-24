using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate120 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ScheduleType",
                table: "Schedules",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ScheduleStatus",
                table: "Schedules",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                table: "Schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SlotTypeId",
                table: "Schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentCount",
                table: "Schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SubjectCode",
                table: "Schedules",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "LabRooms",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "SlotTypeId",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentCount",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
                name: "IX_Schedules_SlotTypeId",
                table: "Schedules",
                column: "SlotTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SlotTypeId",
                table: "Bookings",
                column: "SlotTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_SlotTypes_SlotTypeId",
                table: "Bookings",
                column: "SlotTypeId",
                principalTable: "SlotTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Bookings_BookingId",
                table: "Schedules",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Groups_GroupId",
                table: "Schedules",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_SlotTypes_SlotTypeId",
                table: "Schedules",
                column: "SlotTypeId",
                principalTable: "SlotTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_SlotTypes_SlotTypeId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Bookings_BookingId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Groups_GroupId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_SlotTypes_SlotTypeId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_BookingId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_GroupId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_SlotTypeId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_SlotTypeId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "SlotTypeId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "StudentCount",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "SubjectCode",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "LabRooms");

            migrationBuilder.DropColumn(
                name: "SlotTypeId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "StudentCount",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "ScheduleType",
                table: "Schedules",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ScheduleStatus",
                table: "Schedules",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
