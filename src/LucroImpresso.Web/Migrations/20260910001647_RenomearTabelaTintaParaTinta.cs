using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LucroImpresso.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenomearTabelaTintaParaTinta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ORC_TINT_TINTA_fk_tinta",
                table: "ORC_TINT");

            migrationBuilder.DropForeignKey(
                name: "FK_TINTA_Usuario_fk_usuario",
                table: "TINTA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TINTA",
                table: "TINTA");

            migrationBuilder.RenameTable(
                name: "TINTA",
                newName: "Tinta");

            migrationBuilder.RenameIndex(
                name: "IX_TINTA_fk_usuario",
                table: "Tinta",
                newName: "IX_Tinta_fk_usuario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tinta",
                table: "Tinta",
                column: "id_tinta");

            migrationBuilder.AddForeignKey(
                name: "FK_ORC_TINT_Tinta_fk_tinta",
                table: "ORC_TINT",
                column: "fk_tinta",
                principalTable: "Tinta",
                principalColumn: "id_tinta",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tinta_Usuario_fk_usuario",
                table: "Tinta",
                column: "fk_usuario",
                principalTable: "Usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ORC_TINT_Tinta_fk_tinta",
                table: "ORC_TINT");

            migrationBuilder.DropForeignKey(
                name: "FK_Tinta_Usuario_fk_usuario",
                table: "Tinta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tinta",
                table: "Tinta");

            migrationBuilder.RenameTable(
                name: "Tinta",
                newName: "TINTA");

            migrationBuilder.RenameIndex(
                name: "IX_Tinta_fk_usuario",
                table: "TINTA",
                newName: "IX_TINTA_fk_usuario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TINTA",
                table: "TINTA",
                column: "id_tinta");

            migrationBuilder.AddForeignKey(
                name: "FK_ORC_TINT_TINTA_fk_tinta",
                table: "ORC_TINT",
                column: "fk_tinta",
                principalTable: "TINTA",
                principalColumn: "id_tinta",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TINTA_Usuario_fk_usuario",
                table: "TINTA",
                column: "fk_usuario",
                principalTable: "Usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
