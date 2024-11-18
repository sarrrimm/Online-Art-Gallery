using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtGallery.Migrations
{
    /// <inheritdoc />
    public partial class AddedSoldTo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuctionId",
                table: "ArtWork",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoldTo",
                table: "ArtWork",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Auction",
                columns: table => new
                {
                    AuctionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AuctionName = table.Column<string>(type: "TEXT", nullable: false),
                    AuctionLocation = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auction", x => x.AuctionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtWork_AuctionId",
                table: "ArtWork",
                column: "AuctionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtWork_Auction_AuctionId",
                table: "ArtWork",
                column: "AuctionId",
                principalTable: "Auction",
                principalColumn: "AuctionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtWork_Auction_AuctionId",
                table: "ArtWork");

            migrationBuilder.DropTable(
                name: "Auction");

            migrationBuilder.DropIndex(
                name: "IX_ArtWork_AuctionId",
                table: "ArtWork");

            migrationBuilder.DropColumn(
                name: "AuctionId",
                table: "ArtWork");

            migrationBuilder.DropColumn(
                name: "SoldTo",
                table: "ArtWork");
        }
    }
}
