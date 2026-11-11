using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LucroImpresso.Web.Migrations
{
    /// <summary>
    /// Alinha o banco ao modelo atual:
    ///  1. Tinta → Acabamento (tabelas ORC_TINT → ORC_ACABAMENTO, colunas e coluna "tipo").
    ///     Feito com SQL condicional (idempotente): funciona tanto em um banco novo quanto em um
    ///     banco onde o antigo "Safe Check" do Program.cs já tinha renomeado as tabelas.
    ///  2. peso_gramas: double → decimal(10,2) (precisão monetária no cálculo do material).
    ///  3. Tamanhos de colunas de texto conforme os [MaxLength] dos Models.
    /// Substitui a antiga migration sem Designer (AdicionarTipoInsumoAcabamento), que o EF ignorava.
    /// </summary>
    public partial class AlinhamentoAcabamentoEPesoDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── 1. Tinta → Acabamento (idempotente) ─────────────────────────────
            // MariaDB 10.5+ aceita IF EXISTS / IF NOT EXISTS em RENAME e ALTER:
            // se a renomeação já tiver sido feita, o comando apenas não faz nada.
            migrationBuilder.Sql("SET FOREIGN_KEY_CHECKS = 0;");
            migrationBuilder.Sql("RENAME TABLE IF EXISTS Tinta TO Acabamento;");
            migrationBuilder.Sql("RENAME TABLE IF EXISTS ORC_TINT TO ORC_ACABAMENTO;");
            migrationBuilder.Sql("ALTER TABLE Acabamento CHANGE COLUMN IF EXISTS id_tinta id_acabamento INT NOT NULL AUTO_INCREMENT;");
            migrationBuilder.Sql("ALTER TABLE Acabamento CHANGE COLUMN IF EXISTS nome_tinta nome_acabamento LONGTEXT NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE Acabamento ADD COLUMN IF NOT EXISTS tipo VARCHAR(50) NOT NULL DEFAULT 'Tinta';");
            migrationBuilder.Sql("ALTER TABLE ORC_ACABAMENTO CHANGE COLUMN IF EXISTS fk_tinta fk_acabamento INT NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE HistoricoFinanceiro CHANGE COLUMN IF EXISTS despesas_tinta_acumuladas despesas_acabamento_acumuladas DECIMAL(18,2) NOT NULL DEFAULT 0.00;");
            migrationBuilder.Sql("SET FOREIGN_KEY_CHECKS = 1;");

            // ── 2. Peso em decimal ──────────────────────────────────────────────
            migrationBuilder.AlterColumn<decimal>(
                name: "peso_gramas",
                table: "Orcamento",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            // ── 3. Tamanhos de texto conforme os Models ─────────────────────────
            AlterarTexto(migrationBuilder, "Orcamento", "nome_projeto", 200);
            AlterarTexto(migrationBuilder, "Material", "nome_material", 100);
            AlterarTexto(migrationBuilder, "Material", "cor", 50);
            AlterarTexto(migrationBuilder, "Maquina", "nome_maquina", 100);
            AlterarTexto(migrationBuilder, "Acabamento", "nome_acabamento", 100);
            AlterarTexto(migrationBuilder, "Acabamento", "cor", 50);
        }

        private static void AlterarTexto(MigrationBuilder mb, string tabela, string coluna, int? tamanho)
        {
            mb.AlterColumn<string>(
                name: coluna,
                table: tabela,
                type: tamanho.HasValue ? $"varchar({tamanho})" : "longtext",
                maxLength: tamanho,
                nullable: false,
                oldClrType: typeof(string),
                oldType: tamanho.HasValue ? "longtext" : "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverte tipos de coluna. A renomeação Tinta → Acabamento é mantida,
            // pois o código atual não funciona mais com os nomes antigos.
            migrationBuilder.AlterColumn<double>(
                name: "peso_gramas",
                table: "Orcamento",
                type: "double",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            AlterarTexto(migrationBuilder, "Orcamento", "nome_projeto", null);
            AlterarTexto(migrationBuilder, "Material", "nome_material", null);
            AlterarTexto(migrationBuilder, "Material", "cor", null);
            AlterarTexto(migrationBuilder, "Maquina", "nome_maquina", null);
            AlterarTexto(migrationBuilder, "Acabamento", "nome_acabamento", null);
            AlterarTexto(migrationBuilder, "Acabamento", "cor", null);
        }
    }
}
