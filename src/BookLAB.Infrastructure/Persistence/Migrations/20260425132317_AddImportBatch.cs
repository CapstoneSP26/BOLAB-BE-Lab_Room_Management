using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddImportBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ImportBatchId",
                table: "Schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImportBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ImportBatchType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SemesterName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportBatches", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("27272727-2727-2727-2727-272727272727"),
                column: "ImportBatchId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("28282828-2828-2828-2828-282828282828"),
                column: "ImportBatchId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: new Guid("29292929-2929-2929-2929-292929292929"),
                column: "ImportBatchId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ImportBatchId",
                table: "Schedules",
                column: "ImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportBatches_CreatedAt",
                table: "ImportBatches",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ImportBatches_ImportBatchType",
                table: "ImportBatches",
                column: "ImportBatchType");

            migrationBuilder.CreateIndex(
                name: "IX_ImportBatches_Name_SemesterName",
                table: "ImportBatches",
                columns: new[] { "Name", "SemesterName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportBatches_SemesterName",
                table: "ImportBatches",
                column: "SemesterName");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_ImportBatches_ImportBatchId",
                table: "Schedules",
                column: "ImportBatchId",
                principalTable: "ImportBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_ImportBatches_ImportBatchId",
                table: "Schedules");

            migrationBuilder.DropTable(
                name: "ImportBatches");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_ImportBatchId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "ImportBatchId",
                table: "Schedules");
        }
    }
}
