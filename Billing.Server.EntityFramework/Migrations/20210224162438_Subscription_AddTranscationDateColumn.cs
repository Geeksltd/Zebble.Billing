using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Zebble.Migrations
{
    public partial class Subscription_AddTranscationDateColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                schema: "Subscription",
                table: "Subscriptions",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionDate",
                schema: "Subscription",
                table: "Subscriptions");
        }
    }
}
