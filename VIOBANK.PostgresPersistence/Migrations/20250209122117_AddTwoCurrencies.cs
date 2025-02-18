using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VIOBANK.PostgresPersistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTwoCurrencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "Transactions",
                newName: "CurrencyTo");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyFrom",
                table: "Transactions",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyFrom",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "CurrencyTo",
                table: "Transactions",
                newName: "Currency");
        }
    }
}
