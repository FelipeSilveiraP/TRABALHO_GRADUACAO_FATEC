using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LucroImpresso.Web.Migrations
{
    /// <inheritdoc />
    public partial class AjusteClienteEnderecoTelefone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PessoaFisica_Endereco_fk_endereco",
                table: "PessoaFisica");

            migrationBuilder.DropForeignKey(
                name: "FK_PessoaJuridica_Endereco_fk_endereco",
                table: "PessoaJuridica");

            migrationBuilder.DropIndex(
                name: "IX_PessoaJuridica_fk_endereco",
                table: "PessoaJuridica");

            migrationBuilder.DropIndex(
                name: "IX_PessoaFisica_fk_endereco",
                table: "PessoaFisica");

            migrationBuilder.DropColumn(
                name: "margem_lucro_padrao",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "fk_endereco",
                table: "PessoaJuridica");

            migrationBuilder.DropColumn(
                name: "telefone_unico",
                table: "PessoaJuridica");

            migrationBuilder.DropColumn(
                name: "fk_endereco",
                table: "PessoaFisica");

            migrationBuilder.DropColumn(
                name: "telefone_unico",
                table: "PessoaFisica");

            migrationBuilder.AddColumn<int>(
                name: "id_endereco",
                table: "Cliente",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "telefone_unico",
                table: "Cliente",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_Endereco_id_endereco",
                table: "Cliente");

            migrationBuilder.DropIndex(
                name: "IX_Cliente_id_endereco",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "id_endereco",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "telefone_unico",
                table: "Cliente");

            migrationBuilder.AddColumn<decimal>(
                name: "margem_lucro_padrao",
                table: "Usuario",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "fk_endereco",
                table: "PessoaJuridica",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "telefone_unico",
                table: "PessoaJuridica",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "fk_endereco",
                table: "PessoaFisica",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "telefone_unico",
                table: "PessoaFisica",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PessoaJuridica_fk_endereco",
                table: "PessoaJuridica",
                column: "fk_endereco",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PessoaFisica_fk_endereco",
                table: "PessoaFisica",
                column: "fk_endereco",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PessoaFisica_Endereco_fk_endereco",
                table: "PessoaFisica",
                column: "fk_endereco",
                principalTable: "Endereco",
                principalColumn: "id_endereco",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PessoaJuridica_Endereco_fk_endereco",
                table: "PessoaJuridica",
                column: "fk_endereco",
                principalTable: "Endereco",
                principalColumn: "id_endereco",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
