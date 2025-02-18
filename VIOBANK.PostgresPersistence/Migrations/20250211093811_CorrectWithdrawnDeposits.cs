using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VIOBANK.PostgresPersistence.Migrations
{
    /// <inheritdoc />
    public partial class CorrectWithdrawnDeposits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WithdrawnDeposits_UserId",
                table: "WithdrawnDeposits",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawnDeposits_Users_UserId",
                table: "WithdrawnDeposits",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawnDeposits_Users_UserId",
                table: "WithdrawnDeposits");

            migrationBuilder.DropIndex(
                name: "IX_WithdrawnDeposits_UserId",
                table: "WithdrawnDeposits");
        }
    }
}
