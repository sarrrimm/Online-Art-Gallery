using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtGallery.Migrations
{
    /// <inheritdoc />
    public partial class AddedDiscountPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountedPrice",
                table: "ArtWork",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleId",
                table: "ArtWork",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sale",
                columns: table => new
                {
                    SaleId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SaleName = table.Column<string>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sale", x => x.SaleId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtWork_SaleId",
                table: "ArtWork",
                column: "SaleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtWork_Sale_SaleId",
                table: "ArtWork",
                column: "SaleId",
                principalTable: "Sale",
                principalColumn: "SaleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtWork_Sale_SaleId",
                table: "ArtWork");

            migrationBuilder.DropTable(
                name: "Sale");

            migrationBuilder.DropIndex(
                name: "IX_ArtWork_SaleId",
                table: "ArtWork");

            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "ArtWork");

            migrationBuilder.DropColumn(
                name: "SaleId",
                table: "ArtWork");
        }
    }
}
