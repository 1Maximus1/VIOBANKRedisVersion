using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VIOBANK.PostgresPersistence.Migrations
{
    /// <inheritdoc />
    public partial class CorrectDeposit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Accounts_AccountId",
                table: "Deposits");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Deposits",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "CardId1",
                table: "Deposits",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_CardId1",
                table: "Deposits",
                column: "CardId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Accounts_AccountId",
                table: "Deposits",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Cards_CardId1",
                table: "Deposits",
                column: "CardId1",
                principalTable: "Cards",
                principalColumn: "CardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Accounts_AccountId",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Cards_CardId1",
                table: "Deposits");

            migrationBuilder.DropIndex(
                name: "IX_Deposits_CardId1",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "CardId1",
                table: "Deposits");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Deposits",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Accounts_AccountId",
                table: "Deposits",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
