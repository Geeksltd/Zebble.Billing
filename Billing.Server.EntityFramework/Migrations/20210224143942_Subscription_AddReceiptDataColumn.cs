using Microsoft.EntityFrameworkCore.Migrations;

namespace Zebble.Migrations
{
    public partial class Subscription_AddReceiptDataColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalTransactionId",
                schema: "Subscription",
                table: "Subscriptions",
                newName: "TransactionId");

            migrationBuilder.AddColumn<string>(
                name: "ReceiptData",
                schema: "Subscription",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptData",
                schema: "Subscription",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                schema: "Subscription",
                table: "Subscriptions",
                newName: "OriginalTransactionId");
        }
    }
}
