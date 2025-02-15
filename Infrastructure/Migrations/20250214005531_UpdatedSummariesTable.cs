using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transactionsApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSummariesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "CHALLENGE",
                table: "TransactionSummaries");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                schema: "CHALLENGE",
                table: "TransactionSummaries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
