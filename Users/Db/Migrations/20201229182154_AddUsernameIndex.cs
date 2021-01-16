using Microsoft.EntityFrameworkCore.Migrations;

namespace Users.Migrations
{
    public partial class AddUsernameIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_ProjectId_Username",
                table: "Users",
                columns: new[] { "ProjectId", "Username" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_ProjectId_Username",
                table: "Users");
        }
    }
}
