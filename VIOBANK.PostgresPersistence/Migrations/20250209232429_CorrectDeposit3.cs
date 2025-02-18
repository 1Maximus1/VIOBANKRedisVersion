using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VIOBANK.PostgresPersistence.Migrations
{
    /// <inheritdoc />
    public partial class CorrectDeposit3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Cards_CardId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Cards_CardId",
                table: "Deposits",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "CardId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Cards_CardId",
                table: "Deposits");

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
                name: "FK_Deposits_Cards_CardId",
                table: "Deposits",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "CardId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Cards_CardId1",
                table: "Deposits",
                column: "CardId1",
                principalTable: "Cards",
                principalColumn: "CardId");
        }
    }
}
