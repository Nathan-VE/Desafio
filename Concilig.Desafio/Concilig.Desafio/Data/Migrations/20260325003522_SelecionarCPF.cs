using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concilig.Desafio.Data.Migrations
{
    /// <inheritdoc />
    public partial class SelecionarCPF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CPF",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Produto",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CPF",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "Produto",
                table: "Contratos");
        }
    }
}
