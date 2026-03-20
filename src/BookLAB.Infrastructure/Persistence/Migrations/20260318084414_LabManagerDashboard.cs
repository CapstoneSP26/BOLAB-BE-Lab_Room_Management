using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LabManagerDashboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GroupMembers",
                keyColumn: "Id",
                keyValue: new Guid("36363636-3636-3636-3636-363636363636"),
                column: "SubjectCode",
                value: "");

            migrationBuilder.UpdateData(
                table: "GroupMembers",
                keyColumn: "Id",
                keyValue: new Guid("37373737-3737-3737-3737-373737373737"),
                column: "SubjectCode",
                value: "");

            migrationBuilder.UpdateData(
                table: "GroupMembers",
                keyColumn: "Id",
                keyValue: new Guid("38383838-3838-3838-3838-383838383838"),
                column: "SubjectCode",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GroupMembers",
                keyColumn: "Id",
                keyValue: new Guid("36363636-3636-3636-3636-363636363636"),
                column: "SubjectCode",
                value: null);

            migrationBuilder.UpdateData(
                table: "GroupMembers",
                keyColumn: "Id",
                keyValue: new Guid("37373737-3737-3737-3737-373737373737"),
                column: "SubjectCode",
                value: null);

            migrationBuilder.UpdateData(
                table: "GroupMembers",
                keyColumn: "Id",
                keyValue: new Guid("38383838-3838-3838-3838-383838383838"),
                column: "SubjectCode",
                value: null);
        }
    }
}
