using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCampusIdToGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "Groups",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("18181818-1818-1818-1818-181818181818"),
                column: "CampusId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("19191919-1919-1919-1919-191919191919"),
                column: "CampusId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("20202020-2020-2020-2020-202020202020"),
                column: "CampusId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CampusId",
                table: "Groups",
                column: "CampusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Campuses_CampusId",
                table: "Groups",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Campuses_CampusId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_CampusId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Groups");
        }
    }
}
