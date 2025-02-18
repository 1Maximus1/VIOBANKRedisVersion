using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VIOBANK.PostgresPersistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmploymentToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Employment_Income",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Employment_Type",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Employment_Income",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Employment_Type",
                table: "Users");
        }
    }
}
