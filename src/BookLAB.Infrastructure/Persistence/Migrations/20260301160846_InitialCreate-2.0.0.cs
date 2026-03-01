using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate200 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Bookings_BookingId",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "Attendances",
                newName: "ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_BookingId",
                table: "Attendances",
                newName: "IX_Attendances_ScheduleId");

            migrationBuilder.AddColumn<string>(
                name: "UserCode",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "RoomName",
                table: "LabRooms",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "RoomNo",
                table: "LabRooms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "UQ_User_Code",
                table: "Users",
                column: "UserCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Schedules_ScheduleId",
                table: "Attendances",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Schedules_ScheduleId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "UQ_User_Code",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoomNo",
                table: "LabRooms");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "Attendances",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_ScheduleId",
                table: "Attendances",
                newName: "IX_Attendances_BookingId");

            migrationBuilder.AlterColumn<string>(
                name: "RoomName",
                table: "LabRooms",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Bookings_BookingId",
                table: "Attendances",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
