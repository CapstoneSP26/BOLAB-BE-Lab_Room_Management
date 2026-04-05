using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCampusCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CampusCode",
                table: "Campuses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 1,
                column: "BuildingImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 2,
                column: "BuildingImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 3,
                column: "BuildingImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "Campuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "CampusCode",
                value: null);

            migrationBuilder.UpdateData(
                table: "Campuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "CampusCode",
                value: null);

            migrationBuilder.UpdateData(
                table: "Campuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "CampusCode",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CampusCode",
                table: "Campuses");

            migrationBuilder.UpdateData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 1,
                column: "BuildingImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 2,
                column: "BuildingImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 3,
                column: "BuildingImageUrl",
                value: null);
        }
    }
}
