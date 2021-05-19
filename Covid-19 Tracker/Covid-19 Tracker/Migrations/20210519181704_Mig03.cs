using Microsoft.EntityFrameworkCore.Migrations;

namespace Covid_19_Tracker.Migrations
{
    public partial class Mig03 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewVaccinations",
                table: "Vaccinated");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NewVaccinations",
                table: "Vaccinated",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
