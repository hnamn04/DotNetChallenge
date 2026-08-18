using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetChallenge.Migrations
{
    /// <inheritdoc />
    public partial class RenameProductCodeToSKUAndAddUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "code",
                table: "products",
                newName: "sku");

            migrationBuilder.RenameIndex(
                name: "ix_products_code",
                table: "products",
                newName: "ix_products_sku");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "sku",
                table: "products",
                newName: "code");

            migrationBuilder.RenameIndex(
                name: "ix_products_sku",
                table: "products",
                newName: "ix_products_code");
        }
    }
}
