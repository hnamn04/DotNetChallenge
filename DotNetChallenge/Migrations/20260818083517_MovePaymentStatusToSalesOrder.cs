using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetChallenge.Migrations
{
    /// <inheritdoc />
    public partial class MovePaymentStatusToSalesOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_payments_status",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "status",
                table: "payments");

            migrationBuilder.AddColumn<string>(
                name: "payment_status",
                table: "sales_orders",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Unpaid");

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_payment_status",
                table: "sales_orders",
                column: "payment_status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sales_orders_payment_status",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "payment_status",
                table: "sales_orders");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "payments",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_payments_status",
                table: "payments",
                column: "status");
        }
    }
}
