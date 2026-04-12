using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLAB.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueFieldGroupMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_Group_User_Member",
                table: "GroupMembers");

            migrationBuilder.CreateIndex(
                name: "UQ_Group_User_Member_Subject",
                table: "GroupMembers",
                columns: new[] { "GroupId", "UserId", "SubjectCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_Group_User_Member_Subject",
                table: "GroupMembers");

            migrationBuilder.CreateIndex(
                name: "UQ_Group_User_Member",
                table: "GroupMembers",
                columns: new[] { "GroupId", "UserId" },
                unique: true);
        }
    }
}
