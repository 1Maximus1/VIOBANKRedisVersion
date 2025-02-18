using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VIOBANK.PostgresPersistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactioNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumberFrom",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumberTo",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberFrom",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "NumberTo",
                table: "Transactions");
        }
    }
}
