using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixNotificationUserId1ShadowColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM information_schema.table_constraints
        WHERE constraint_name = 'FK_Notifications_Users_UserId1'
          AND table_name = 'Notifications'
    ) THEN
        ALTER TABLE ""Notifications"" DROP CONSTRAINT ""FK_Notifications_Users_UserId1"";
    END IF;
END $$;");

            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM pg_indexes
        WHERE schemaname = 'public'
          AND tablename = 'Notifications'
          AND indexname = 'IX_Notifications_UserId1'
    ) THEN
        DROP INDEX ""IX_Notifications_UserId1"";
    END IF;
END $$;");

            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_name = 'Notifications'
          AND column_name = 'UserId1'
    ) THEN
        ALTER TABLE ""Notifications"" DROP COLUMN ""UserId1"";
    END IF;
END $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Notifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId1",
                table: "Notifications",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId1",
                table: "Notifications",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
