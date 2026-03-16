using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentReporting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Environment = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StepsToReproduce = table.Column<List<string>>(type: "text[]", nullable: false),
                    ExpectedResult = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ActualResult = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AttachmentUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ReportedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidents_Users_ReportedBy",
                        column: x => x.ReportedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_CreatedAt",
                table: "Incidents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_ReportedBy",
                table: "Incidents",
                column: "ReportedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_Severity",
                table: "Incidents",
                column: "Severity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Incidents");

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
