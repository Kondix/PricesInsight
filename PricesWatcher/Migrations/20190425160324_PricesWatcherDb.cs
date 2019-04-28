using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PricesWatcher.Migrations
{
    public partial class PricesWatcherDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HotelCode = table.Column<string>(nullable: true),
                    HotelStandard = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HotelForeignKey = table.Column<int>(nullable: true),
                    DepartureDateTime = table.Column<DateTime>(nullable: false),
                    ReturnDate = table.Column<DateTime>(nullable: false),
                    RoomName = table.Column<string>(nullable: true),
                    RoomCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offers_Hotels_HotelForeignKey",
                        column: x => x.HotelForeignKey,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TripAdvisorHotelRatings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HotelForeignKey = table.Column<int>(nullable: true),
                    TripAdvisorRating = table.Column<float>(nullable: false),
                    TripAdvisorReviewsNo = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripAdvisorHotelRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripAdvisorHotelRatings_Hotels_HotelForeignKey",
                        column: x => x.HotelForeignKey,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OfferForeignKey = table.Column<int>(nullable: true),
                    StandardPrice = table.Column<float>(nullable: false),
                    DiscountPrice = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prices_Offers_OfferForeignKey",
                        column: x => x.OfferForeignKey,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offers_HotelForeignKey",
                table: "Offers",
                column: "HotelForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_OfferForeignKey",
                table: "Prices",
                column: "OfferForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_TripAdvisorHotelRatings_HotelForeignKey",
                table: "TripAdvisorHotelRatings",
                column: "HotelForeignKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "TripAdvisorHotelRatings");

            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DropTable(
                name: "Hotels");
        }
    }
}
