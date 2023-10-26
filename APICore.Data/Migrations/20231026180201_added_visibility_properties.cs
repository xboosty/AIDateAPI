using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICore.Data.Migrations
{
    public partial class added_visibility_properties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGenderVisible",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSexualityVisible",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGenderVisible",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsSexualityVisible",
                table: "Users");
        }
    }
}
