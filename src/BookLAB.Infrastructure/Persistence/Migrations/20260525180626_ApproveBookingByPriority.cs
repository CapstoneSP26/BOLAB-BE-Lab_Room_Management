using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApproveBookingByPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AutoCancelledByBookingId",
                table: "Schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SchedulePriority",
                table: "Schedules",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriorityLevel",
                table: "PurposeTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AutoRejectedByBookingId",
                table: "Bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "AutoRejectedByBookingId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "AutoRejectedByBookingId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "AutoRejectedByBookingId",
                value: null);

            migrationBuilder.UpdateData(
                table: "PurposeTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "PriorityLevel",
                value: 0);

            migrationBuilder.UpdateData(
                table: "PurposeTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "PriorityLevel",
                value: 0);

            migrationBuilder.UpdateData(
                table: "PurposeTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "PriorityLevel",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("27272727-2727-2727-2727-272727272727"),
                columns: new[] { "AutoCancelledByBookingId", "SchedulePriority" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("28282828-2828-2828-2828-282828282828"),
                columns: new[] { "AutoCancelledByBookingId", "SchedulePriority" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("29292929-2929-2929-2929-292929292929"),
                columns: new[] { "AutoCancelledByBookingId", "SchedulePriority" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_AutoCancelledByBookingId",
                table: "Schedules",
                column: "AutoCancelledByBookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AutoRejectedByBookingId",
                table: "Bookings",
                column: "AutoRejectedByBookingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Bookings_AutoRejectedByBookingId",
                table: "Bookings",
                column: "AutoRejectedByBookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Bookings_AutoCancelledByBookingId",
                table: "Schedules",
                column: "AutoCancelledByBookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Bookings_AutoRejectedByBookingId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Bookings_AutoCancelledByBookingId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_AutoCancelledByBookingId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_AutoRejectedByBookingId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AutoCancelledByBookingId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "SchedulePriority",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "PriorityLevel",
                table: "PurposeTypes");

            migrationBuilder.DropColumn(
                name: "AutoRejectedByBookingId",
                table: "Bookings");
        }
    }
}
