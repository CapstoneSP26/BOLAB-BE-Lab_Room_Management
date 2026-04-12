using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectCode = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    SubjectName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: new Guid("15151515-1515-1515-1515-151515151515"),
                column: "AttendanceStatus",
                value: "Absent");

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: new Guid("16161616-1616-1616-1616-161616161616"),
                column: "AttendanceStatus",
                value: "Absent");

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: new Guid("17171717-1717-1717-1717-171717171717"),
                column: "AttendanceStatus",
                value: "Absent");

            migrationBuilder.UpdateData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("27272727-2727-2727-2727-272727272727"),
                column: "ScheduleType",
                value: "Academic");

            migrationBuilder.UpdateData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("28282828-2828-2828-2828-282828282828"),
                column: "ScheduleType",
                value: "Academic");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SubjectCode",
                table: "Subjects",
                column: "SubjectCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: new Guid("15151515-1515-1515-1515-151515151515"),
                column: "AttendanceStatus",
                value: "NotYet");

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: new Guid("16161616-1616-1616-1616-161616161616"),
                column: "AttendanceStatus",
                value: "NotYet");

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: new Guid("17171717-1717-1717-1717-171717171717"),
                column: "AttendanceStatus",
                value: "NotYet");

            migrationBuilder.UpdateData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("27272727-2727-2727-2727-272727272727"),
                column: "ScheduleType",
                value: "Booking");

            migrationBuilder.UpdateData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("28282828-2828-2828-2828-282828282828"),
                column: "ScheduleType",
                value: "Booking");
        }
    }
}
