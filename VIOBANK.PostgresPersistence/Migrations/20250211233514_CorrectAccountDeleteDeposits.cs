using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VIOBANK.PostgresPersistence.Migrations
{
    /// <inheritdoc />
    public partial class CorrectAccountDeleteDeposits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Accounts_AccountId",
                table: "Deposits");

            migrationBuilder.DropIndex(
                name: "IX_Deposits_AccountId",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Deposits");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Deposits",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_AccountId",
                table: "Deposits",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Accounts_AccountId",
                table: "Deposits",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId");
        }
    }
}
