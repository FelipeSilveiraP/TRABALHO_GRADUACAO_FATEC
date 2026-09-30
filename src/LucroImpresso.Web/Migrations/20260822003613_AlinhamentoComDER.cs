using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LucroImpresso.Web.Migrations
{
    /// <inheritdoc />
    public partial class AlinhamentoComDER : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_Endereco_id_endereco",
                table: "Cliente");

            migrationBuilder.DropForeignKey(
                name: "FK_Orcamento_Endereco_fk_endereco",
                table: "Orcamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Orcamento_Maquina_id_maquina",
                table: "Orcamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Orcamento_Peca_fk_peca",
                table: "Orcamento");

            migrationBuilder.DropIndex(
                name: "IX_Orcamento_fk_endereco",
                table: "Orcamento");

            migrationBuilder.DropIndex(
                name: "IX_Cliente_id_endereco",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "fk_endereco",
                table: "Orcamento");

            migrationBuilder.DropColumn(
                name: "id_endereco",
                table: "Cliente");

            migrationBuilder.RenameColumn(
                name: "id_maquina",
                table: "Orcamento",
                newName: "fk_maquina");

            migrationBuilder.RenameIndex(
                name: "IX_Orcamento_id_maquina",
                table: "Orcamento",
                newName: "IX_Orcamento_fk_maquina");

            migrationBuilder.AlterColumn<int>(
                name: "fk_peca",
                table: "Orcamento",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fk_cliente",
                table: "Endereco",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Endereco_fk_cliente",
                table: "Endereco",
                column: "fk_cliente",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Endereco_Cliente_fk_cliente",
                table: "Endereco",
                column: "fk_cliente",
                principalTable: "Cliente",
                principalColumn: "id_cliente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orcamento_Maquina_fk_maquina",
                table: "Orcamento",
                column: "fk_maquina",
                principalTable: "Maquina",
                principalColumn: "id_maquina",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orcamento_Peca_fk_peca",
                table: "Orcamento",
                column: "fk_peca",
                principalTable: "Peca",
                principalColumn: "id_peca",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endereco_Cliente_fk_cliente",
                table: "Endereco");

            migrationBuilder.DropForeignKey(
                name: "FK_Orcamento_Maquina_fk_maquina",
                table: "Orcamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Orcamento_Peca_fk_peca",
                table: "Orcamento");

            migrationBuilder.DropIndex(
                name: "IX_Endereco_fk_cliente",
                table: "Endereco");

            migrationBuilder.DropColumn(
                name: "fk_cliente",
                table: "Endereco");

            migrationBuilder.RenameColumn(
                name: "fk_maquina",
                table: "Orcamento",
                newName: "id_maquina");

            migrationBuilder.RenameIndex(
                name: "IX_Orcamento_fk_maquina",
                table: "Orcamento",
                newName: "IX_Orcamento_id_maquina");

            migrationBuilder.AlterColumn<int>(
                name: "fk_peca",
                table: "Orcamento",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "fk_endereco",
                table: "Orcamento",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_endereco",
                table: "Cliente",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orcamento_fk_endereco",
                table: "Orcamento",
                column: "fk_endereco");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_id_endereco",
                table: "Cliente",
                column: "id_endereco");

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_Endereco_id_endereco",
                table: "Cliente",
                column: "id_endereco",
                principalTable: "Endereco",
                principalColumn: "id_endereco",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Orcamento_Endereco_fk_endereco",
                table: "Orcamento",
                column: "fk_endereco",
                principalTable: "Endereco",
                principalColumn: "id_endereco",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Orcamento_Maquina_id_maquina",
                table: "Orcamento",
                column: "id_maquina",
                principalTable: "Maquina",
                principalColumn: "id_maquina",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orcamento_Peca_fk_peca",
                table: "Orcamento",
                column: "fk_peca",
                principalTable: "Peca",
                principalColumn: "id_peca",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
