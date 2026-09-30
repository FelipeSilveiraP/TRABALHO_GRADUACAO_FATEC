using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LucroImpresso.Web.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarOrcamentoTempos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tempo_horas",
                table: "Orcamento",
                newName: "tempo_total");

            migrationBuilder.AddColumn<int>(
                name: "tempo_hrs_impressao",
                table: "Orcamento",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "tempo_hrs_pintura_lixamento",
                table: "Orcamento",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tempo_hrs_impressao",
                table: "Orcamento");

            migrationBuilder.DropColumn(
                name: "tempo_hrs_pintura_lixamento",
                table: "Orcamento");

            migrationBuilder.RenameColumn(
                name: "tempo_total",
                table: "Orcamento",
                newName: "tempo_horas");
        }
    }
}
