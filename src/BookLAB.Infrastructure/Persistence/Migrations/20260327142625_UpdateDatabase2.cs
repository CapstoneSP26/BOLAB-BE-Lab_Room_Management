using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportType",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReportType",
                table: "Reports");

            migrationBuilder.AddColumn<int>(
                name: "ReportTypeId",
                table: "Reports",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReportTypes",
                columns: table => new
                {
                    ReportTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReportTypeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTypes", x => x.ReportTypeId);
                });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "BookingType",
                value: "Standard");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "BookingType",
                value: "Standard");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "BookingType",
                value: "Standard");

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

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: new Guid("21212121-2121-2121-2121-212121212121"),
                column: "ReportTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"),
                column: "ReportTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: new Guid("23232323-2323-2323-2323-232323232323"),
                column: "ReportTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoomPolicies",
                keyColumn: "Id",
                keyValue: new Guid("30303030-3030-3030-3030-303030303030"),
                column: "PolicyKey",
                value: "CurfewTime");

            migrationBuilder.UpdateData(
                table: "RoomPolicies",
                keyColumn: "Id",
                keyValue: new Guid("31313131-3131-3131-3131-313131313131"),
                column: "PolicyKey",
                value: "CurfewTime");

            migrationBuilder.UpdateData(
                table: "RoomPolicies",
                keyColumn: "Id",
                keyValue: new Guid("32323232-3232-3232-3232-323232323232"),
                column: "PolicyKey",
                value: "CurfewTime");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportTypeId",
                table: "Reports",
                column: "ReportTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportTypes_ReportTypeId",
                table: "Reports",
                column: "ReportTypeId",
                principalTable: "ReportTypes",
                principalColumn: "ReportTypeId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportTypes_ReportTypeId",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "ReportTypes");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportTypeId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReportTypeId",
                table: "Reports");

            migrationBuilder.AddColumn<string>(
                name: "ReportType",
                table: "Reports",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "BookingType",
                value: "0");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "BookingType",
                value: "0");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "BookingType",
                value: "0");

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: new Guid("21212121-2121-2121-2121-212121212121"),
                column: "ReportType",
                value: "0");

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"),
                column: "ReportType",
                value: "0");

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: new Guid("23232323-2323-2323-2323-232323232323"),
                column: "ReportType",
                value: "0");

            migrationBuilder.UpdateData(
                table: "RoomPolicies",
                keyColumn: "Id",
                keyValue: new Guid("30303030-3030-3030-3030-303030303030"),
                column: "PolicyKey",
                value: "MaxCapacity");

            migrationBuilder.UpdateData(
                table: "RoomPolicies",
                keyColumn: "Id",
                keyValue: new Guid("31313131-3131-3131-3131-313131313131"),
                column: "PolicyKey",
                value: "Projector");

            migrationBuilder.UpdateData(
                table: "RoomPolicies",
                keyColumn: "Id",
                keyValue: new Guid("32323232-3232-3232-3232-323232323232"),
                column: "PolicyKey",
                value: "FoodAllowed");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportType",
                table: "Reports",
                column: "ReportType");
        }
    }
}
