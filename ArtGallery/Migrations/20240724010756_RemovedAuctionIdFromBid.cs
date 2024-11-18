using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtGallery.Migrations
{
    /// <inheritdoc />
    public partial class RemovedAuctionIdFromBid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuctionId",
                table: "Bids");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Bids",
                newName: "BidderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BidderId",
                table: "Bids",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "AuctionId",
                table: "Bids",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
