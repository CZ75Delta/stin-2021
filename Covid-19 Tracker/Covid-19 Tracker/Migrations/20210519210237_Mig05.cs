using Microsoft.EntityFrameworkCore.Migrations;

namespace Covid_19_Tracker.Migrations
{
    public partial class Mig05 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Infected_Countries_CountryId",
                table: "Infected");

            migrationBuilder.DropForeignKey(
                name: "FK_Vaccinated_Countries_CountryId",
                table: "Vaccinated");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Vaccinated",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Infected",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Infected_Countries_CountryId",
                table: "Infected",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vaccinated_Countries_CountryId",
                table: "Vaccinated",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Infected_Countries_CountryId",
                table: "Infected");

            migrationBuilder.DropForeignKey(
                name: "FK_Vaccinated_Countries_CountryId",
                table: "Vaccinated");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Vaccinated",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Infected",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Infected_Countries_CountryId",
                table: "Infected",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vaccinated_Countries_CountryId",
                table: "Vaccinated",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
