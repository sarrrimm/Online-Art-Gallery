using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtGallery.Migrations
{
    /// <inheritdoc />
    public partial class ArtWorkCategoryToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ArtWork",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "ArtWork");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ArtWork",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ArtWork_CategoryId",
                table: "ArtWork",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtWork_Category_CategoryId",
                table: "ArtWork",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
