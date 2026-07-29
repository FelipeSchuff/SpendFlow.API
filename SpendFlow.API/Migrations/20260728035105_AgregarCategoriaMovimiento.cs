using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpendFlow.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCategoriaMovimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Movimientos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Movimientos");
        }
    }
}
