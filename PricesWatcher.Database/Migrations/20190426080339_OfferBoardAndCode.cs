using Microsoft.EntityFrameworkCore.Migrations;

namespace PricesWatcher.Migrations
{
    public partial class OfferBoardAndCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Board",
                table: "Offers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Offers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Board",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Offers");
        }
    }
}
