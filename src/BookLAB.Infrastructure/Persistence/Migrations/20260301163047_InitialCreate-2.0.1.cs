using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate201 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportImage_Report_Avatar",
                table: "ReportImages");

            migrationBuilder.DropColumn(
                name: "IsAvatar",
                table: "ReportImages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvatar",
                table: "ReportImages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ReportImage_Report_Avatar",
                table: "ReportImages",
                columns: new[] { "ReportId", "IsAvatar" });
        }
    }
}
