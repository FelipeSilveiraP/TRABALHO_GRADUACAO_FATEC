using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LucroImpresso.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenameTelefoneToCelular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricoFinanceiro_Usuario_fk_usuario",
                table: "HistoricoFinanceiro");

            migrationBuilder.RenameColumn(
                name: "telefone_unico",
                table: "Cliente",
                newName: "celular_unico");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricoFinanceiro_Usuario_fk_usuario",
                table: "HistoricoFinanceiro",
                column: "fk_usuario",
                principalTable: "Usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricoFinanceiro_Usuario_fk_usuario",
                table: "HistoricoFinanceiro");

            migrationBuilder.RenameColumn(
                name: "celular_unico",
                table: "Cliente",
                newName: "telefone_unico");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricoFinanceiro_Usuario_fk_usuario",
                table: "HistoricoFinanceiro",
                column: "fk_usuario",
                principalTable: "Usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
