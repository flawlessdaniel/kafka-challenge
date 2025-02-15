using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transactionsApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSummariesTableKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionSummaries",
                schema: "CHALLENGE",
                table: "TransactionSummaries",
                columns: new[] { "AccountId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionSummaries",
                schema: "CHALLENGE",
                table: "TransactionSummaries");
        }
    }
}
