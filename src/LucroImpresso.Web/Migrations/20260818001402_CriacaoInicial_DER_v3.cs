using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LucroImpresso.Web.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial_DER_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome_usuario = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    senha = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    margem_lucro_padrao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.id_usuario);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    id_cliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.id_cliente);
                    table.ForeignKey(
                        name: "FK_Cliente_Usuario_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Endereco",
                columns: table => new
                {
                    id_endereco = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    logradouro = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero = table.Column<int>(type: "int", nullable: true),
                    bairro = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cidade = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cep = table.Column<string>(type: "varchar(9)", maxLength: 9, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_cadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endereco", x => x.id_endereco);
                    table.ForeignKey(
                        name: "FK_Endereco_Usuario_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HistoricoFinanceiro",
                columns: table => new
                {
                    id_historico_financeiro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    receitas_acumuladas = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    despesas_material_acumuladas = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    despesas_energia_acumuladas = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    data = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoFinanceiro", x => x.id_historico_financeiro);
                    table.ForeignKey(
                        name: "FK_HistoricoFinanceiro_Usuario_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Maquina",
                columns: table => new
                {
                    id_maquina = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    nome_maquina = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    consumo_watts = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    custo_kwh = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maquina", x => x.id_maquina);
                    table.ForeignKey(
                        name: "FK_Maquina_Usuario_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    id_material = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    nome_material = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cor = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    peso_rolo_gramas = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    valor_pago = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    saldo_gramas = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.id_material);
                    table.ForeignKey(
                        name: "FK_Material_Usuario_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Peca",
                columns: table => new
                {
                    id_peca = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    nome_peca = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    link_arquivo_stl = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_cadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Peca", x => x.id_peca);
                    table.ForeignKey(
                        name: "FK_Peca_Usuario_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PessoaFisica",
                columns: table => new
                {
                    fk_cliente = table.Column<int>(type: "int", nullable: false),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    cpf = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone_unico = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fk_endereco = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PessoaFisica", x => x.fk_cliente);
                    table.ForeignKey(
                        name: "FK_PessoaFisica_Cliente_fk_cliente",
                        column: x => x.fk_cliente,
                        principalTable: "Cliente",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PessoaFisica_Endereco_fk_endereco",
                        column: x => x.fk_endereco,
                        principalTable: "Endereco",
                        principalColumn: "id_endereco",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PessoaFisica_Usuario_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PessoaJuridica",
                columns: table => new
                {
                    fk_cliente = table.Column<int>(type: "int", nullable: false),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    cnpj = table.Column<string>(type: "varchar(18)", maxLength: 18, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    razao_social = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone_unico = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fk_endereco = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PessoaJuridica", x => x.fk_cliente);
                    table.ForeignKey(
                        name: "FK_PessoaJuridica_Cliente_fk_cliente",
                        column: x => x.fk_cliente,
                        principalTable: "Cliente",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PessoaJuridica_Endereco_fk_endereco",
                        column: x => x.fk_endereco,
                        principalTable: "Endereco",
                        principalColumn: "id_endereco",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PessoaJuridica_Usuario_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Orcamento",
                columns: table => new
                {
                    id_orcamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    nome_projeto = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fk_cliente = table.Column<int>(type: "int", nullable: false),
                    fk_peca = table.Column<int>(type: "int", nullable: true),
                    id_maquina = table.Column<int>(type: "int", nullable: false),
                    fk_endereco = table.Column<int>(type: "int", nullable: true),
                    fk_material = table.Column<int>(type: "int", nullable: false),
                    peso_gramas = table.Column<double>(type: "double", nullable: false),
                    tempo_horas = table.Column<int>(type: "int", nullable: false),
                    custo_mao_de_obra_hora = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orcamento", x => x.id_orcamento);
                    table.ForeignKey(
                        name: "FK_Orcamento_Cliente_fk_cliente",
                        column: x => x.fk_cliente,
                        principalTable: "Cliente",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orcamento_Endereco_fk_endereco",
                        column: x => x.fk_endereco,
                        principalTable: "Endereco",
                        principalColumn: "id_endereco",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orcamento_Maquina_id_maquina",
                        column: x => x.id_maquina,
                        principalTable: "Maquina",
                        principalColumn: "id_maquina",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orcamento_Material_fk_material",
                        column: x => x.fk_material,
                        principalTable: "Material",
                        principalColumn: "id_material",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orcamento_Peca_fk_peca",
                        column: x => x.fk_peca,
                        principalTable: "Peca",
                        principalColumn: "id_peca",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orcamento_Usuario_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_fk_usuario",
                table: "Cliente",
                column: "fk_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Endereco_fk_usuario",
                table: "Endereco",
                column: "fk_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoFinanceiro_fk_usuario",
                table: "HistoricoFinanceiro",
                column: "fk_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Maquina_fk_usuario",
                table: "Maquina",
                column: "fk_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Material_fk_usuario",
                table: "Material",
                column: "fk_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Orcamento_fk_cliente",
                table: "Orcamento",
                column: "fk_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Orcamento_fk_endereco",
                table: "Orcamento",
                column: "fk_endereco");

            migrationBuilder.CreateIndex(
                name: "IX_Orcamento_fk_material",
                table: "Orcamento",
                column: "fk_material");

            migrationBuilder.CreateIndex(
                name: "IX_Orcamento_fk_peca",
                table: "Orcamento",
                column: "fk_peca");

            migrationBuilder.CreateIndex(
                name: "IX_Orcamento_fk_usuario",
                table: "Orcamento",
                column: "fk_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Orcamento_id_maquina",
                table: "Orcamento",
                column: "id_maquina");

            migrationBuilder.CreateIndex(
                name: "IX_Peca_fk_usuario",
                table: "Peca",
                column: "fk_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_PessoaFisica_fk_endereco",
                table: "PessoaFisica",
                column: "fk_endereco",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PessoaFisica_fk_usuario",
                table: "PessoaFisica",
                column: "fk_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_PessoaJuridica_fk_endereco",
                table: "PessoaJuridica",
                column: "fk_endereco",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PessoaJuridica_fk_usuario",
                table: "PessoaJuridica",
                column: "fk_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoFinanceiro");

            migrationBuilder.DropTable(
                name: "Orcamento");

            migrationBuilder.DropTable(
                name: "PessoaFisica");

            migrationBuilder.DropTable(
                name: "PessoaJuridica");

            migrationBuilder.DropTable(
                name: "Maquina");

            migrationBuilder.DropTable(
                name: "Material");

            migrationBuilder.DropTable(
                name: "Peca");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Endereco");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
