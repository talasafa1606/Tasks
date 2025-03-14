using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task1Bank.Migrations
{
    /// <inheritdoc />
    public partial class Task6Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ToAccountId",
                table: "AccountTransactions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "FromAccountId",
                table: "AccountTransactions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionDe",
                table: "AccountTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionFr",
                table: "AccountTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                table: "Accounts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionDe",
                table: "AccountTransactions");

            migrationBuilder.DropColumn(
                name: "DescriptionFr",
                table: "AccountTransactions");

            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                table: "Accounts");

            migrationBuilder.AlterColumn<int>(
                name: "ToAccountId",
                table: "AccountTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FromAccountId",
                table: "AccountTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
