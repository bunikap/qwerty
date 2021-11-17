using Microsoft.EntityFrameworkCore.Migrations;

namespace qwerty.Migrations
{
    public partial class InitialCreate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pswd",
                table: "Owner",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pswd",
                table: "Owner");
        }
    }
}
