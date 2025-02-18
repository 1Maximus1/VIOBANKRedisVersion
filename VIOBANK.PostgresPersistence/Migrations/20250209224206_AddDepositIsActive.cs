using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VIOBANK.PostgresPersistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDepositIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Deposits",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Deposits");
        }
    }
}
