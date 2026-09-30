using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LucroImpresso.Web.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTinta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "despesas_tinta_acumuladas",
                table: "HistoricoFinanceiro",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "TINTA",
                columns: table => new
                {
                    id_tinta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    nome_tinta = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cor = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    peso_litros = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    valor_pago = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    saldo_mililitros = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TINTA", x => x.id_tinta);
                    table.ForeignKey(
                        name: "FK_TINTA_Usuario_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ORC_TINT",
                columns: table => new
                {
                    fk_orcamento = table.Column<int>(type: "int", nullable: false),
                    fk_tinta = table.Column<int>(type: "int", nullable: false),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    quantidade_ml = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORC_TINT", x => new { x.fk_orcamento, x.fk_tinta });
                    table.ForeignKey(
                        name: "FK_ORC_TINT_Orcamento_fk_orcamento",
                        column: x => x.fk_orcamento,
                        principalTable: "Orcamento",
                        principalColumn: "id_orcamento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ORC_TINT_TINTA_fk_tinta",
                        column: x => x.fk_tinta,
                        principalTable: "TINTA",
                        principalColumn: "id_tinta",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ORC_TINT_Usuario_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ORC_TINT_fk_tinta",
                table: "ORC_TINT",
                column: "fk_tinta");

            migrationBuilder.CreateIndex(
                name: "IX_ORC_TINT_fk_usuario",
                table: "ORC_TINT",
                column: "fk_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_TINTA_fk_usuario",
                table: "TINTA",
                column: "fk_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ORC_TINT");

            migrationBuilder.DropTable(
                name: "TINTA");

            migrationBuilder.DropColumn(
                name: "despesas_tinta_acumuladas",
                table: "HistoricoFinanceiro");
        }
    }
}
