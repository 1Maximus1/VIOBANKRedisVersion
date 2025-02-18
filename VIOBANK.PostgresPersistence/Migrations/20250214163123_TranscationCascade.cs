using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VIOBANK.PostgresPersistence.Migrations
{
    /// <inheritdoc />
    public partial class TranscationCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Cards_FromCardId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Cards_ToCardId",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Cards_FromCardId",
                table: "Transactions",
                column: "FromCardId",
                principalTable: "Cards",
                principalColumn: "CardId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Cards_ToCardId",
                table: "Transactions",
                column: "ToCardId",
                principalTable: "Cards",
                principalColumn: "CardId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION update_account_balance()
                RETURNS TRIGGER AS $$
                BEGIN
                    -- Обновляем баланс аккаунта на сумму всех его карт
                    UPDATE ""Accounts""
                    SET ""Balance"" = (
                        SELECT COALESCE(SUM(""Balance""), 0) 
                        FROM ""Cards""
                        WHERE ""AccountId"" = NEW.""AccountId""
                    )
                    WHERE ""AccountId"" = NEW.""AccountId"";

                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
            ");

             migrationBuilder.Sql(@"
                CREATE TRIGGER trigger_update_account_balance
                AFTER INSERT OR UPDATE ON ""Cards""
                FOR EACH ROW
                EXECUTE FUNCTION update_account_balance();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Cards_FromCardId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Cards_ToCardId",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Cards_FromCardId",
                table: "Transactions",
                column: "FromCardId",
                principalTable: "Cards",
                principalColumn: "CardId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Cards_ToCardId",
                table: "Transactions",
                column: "ToCardId",
                principalTable: "Cards",
                principalColumn: "CardId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trigger_update_account_balance ON ""Cards"";");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS update_account_balance;");
        }
    }
}
