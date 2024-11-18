using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtGallery.Migrations
{
    /// <inheritdoc />
    public partial class AddedArtWorksInCat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ArtWork",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArtWork_CategoryId",
                table: "ArtWork",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtWork_Category_CategoryId",
                table: "ArtWork",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtWork_Category_CategoryId",
                table: "ArtWork");

            migrationBuilder.DropIndex(
                name: "IX_ArtWork_CategoryId",
                table: "ArtWork");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ArtWork");
        }
    }
}
