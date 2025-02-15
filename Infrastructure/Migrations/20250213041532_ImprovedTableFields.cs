using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transactionsApi.Migrations
{
    /// <inheritdoc />
    public partial class ImprovedTableFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                schema: "CHALLENGE",
                table: "TransactionSummaries");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "CHALLENGE",
                table: "TransactionSummaries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "CHALLENGE",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransferTypeId",
                schema: "CHALLENGE",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "CHALLENGE",
                table: "TransactionSummaries");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "CHALLENGE",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransferTypeId",
                schema: "CHALLENGE",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                schema: "CHALLENGE",
                table: "TransactionSummaries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
