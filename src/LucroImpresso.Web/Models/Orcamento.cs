using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucroImpresso.Web.Models
{
    public enum StatusOrcamento
    {
        Pendente,
        EmImpressao,
        Concluido,
        Falha
    }

    [Table("Orcamento")]
    public class Orcamento
    {
        [Column("id_orcamento")]
        public int Id { get; set; }

        // ── Dono do registro ──────────────────────────────────────────────
        [Required]
        [Column("fk_usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        // ── Projeto ───────────────────────────────────────────────────────
        [Required(ErrorMessage = "O nome do projeto é obrigatório.")]
        [MaxLength(200)]
        [Column("nome_projeto")]
        public string NomeProjeto { get; set; } = string.Empty;

        // ── FK Cliente (obrigatório) ───────────────────────────────────────
        [Required(ErrorMessage = "Selecione um cliente.")]
        [Column("fk_cliente")]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        // ── FK Peça (obrigatório) ────────────────────────────────────────────
        [Required(ErrorMessage = "A peça é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma peça.")]
        [Column("fk_peca")]
        public int PecaId { get; set; }

        [ForeignKey("PecaId")]
        public Peca? Peca { get; set; }

        // ── FK Máquina (obrigatório — DER: MAQUINA(1,1)—Imprime—(0,n)ORCAMENTO) ──────
        [Required(ErrorMessage = "Selecione uma máquina.")]
        [Column("fk_maquina")]
        public int MaquinaId { get; set; }

        [ForeignKey("MaquinaId")]
        public Maquina? Maquina { get; set; }

        // ── FK Material (obrigatório) ─────────────────────────────────────
        [Required(ErrorMessage = "Selecione um material.")]
        [Column("fk_material")]
        public int MaterialId { get; set; }

        [ForeignKey("MaterialId")]
        public Material? Material { get; set; }

        public List<OrcamentoAcabamento> OrcamentoAcabamentos { get; set; } = new();

        // ── Dados técnicos ────────────────────────────────────────────────
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O peso deve ser maior que zero.")]
        [Column("peso_gramas")]
        public double PesoGramas { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O tempo de impressão deve ser maior que zero.")]
        [Column("tempo_hrs_impressao", TypeName = "decimal(10,2)")]
        public decimal TempoHrsImpressao { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "O tempo não pode ser negativo.")]
        [Column("tempo_hrs_pintura_lixamento", TypeName = "decimal(10,2)")]
        public decimal TempoHrsPinturaLixamento { get; set; }

        [Required]
        [Column("tempo_total", TypeName = "decimal(10,2)")]
        public decimal TempoTotal { get; set; }

        [Required]
        [Column("custo_mao_de_obra_hora", TypeName = "decimal(18,2)")]
        public decimal CustoMaoDeObraHora { get; set; }

        // ── Status e datas ────────────────────────────────────────────────
        [Column("status")]
        public StatusOrcamento Status { get; set; } = StatusOrcamento.Pendente;

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // ── Cálculo final (Soma direta dos custos) ────────────────────────
        [NotMapped]
        public decimal CustoFinal
        {
            get
            {
                decimal custoMaterial = Material != null ? (decimal)PesoGramas * Material.PrecoPorGrama : 0m;
                decimal consumoW = Maquina?.ConsumoWatts ?? 0;
                decimal custoKwh = Maquina?.CustoKwh ?? 0;
                decimal custoEnergia = (consumoW / 1000m) * TempoHrsImpressao * custoKwh;
                decimal custoMaoDeObra = TempoTotal * CustoMaoDeObraHora;
                
                decimal custoAcabamentos = 0;
                if (OrcamentoAcabamentos != null)
                {
                    foreach (var oa in OrcamentoAcabamentos)
                    {
                        if (oa.Acabamento != null)
                        {
                            custoAcabamentos += oa.Acabamento.PrecoPorMl * oa.QuantidadeMl;
                        }
                    }
                }

                return custoMaterial + custoEnergia + custoMaoDeObra + custoAcabamentos;
            }
        }

        // ── Margem de Lucro Real (Mão de Obra sobre Preço de Venda) ───────
        [NotMapped]
        public decimal MargemLucroPercentual
        {
            get
            {
                decimal custoMaterial = Material != null ? (decimal)PesoGramas * Material.PrecoPorGrama : 0m;
                decimal consumoW = Maquina?.ConsumoWatts ?? 0;
                decimal custoKwh = Maquina?.CustoKwh ?? 0;
                decimal custoEnergia = (consumoW / 1000m) * TempoHrsImpressao * custoKwh;
                
                decimal custoAcabamentos = 0;
                if (OrcamentoAcabamentos != null)
                {
                    foreach (var oa in OrcamentoAcabamentos)
                    {
                        if (oa.Acabamento != null)
                        {
                            custoAcabamentos += oa.Acabamento.PrecoPorMl * oa.QuantidadeMl;
                        }
                    }
                }

                decimal custosFisicos = custoMaterial + custoEnergia + custoAcabamentos;
                decimal lucro = TempoTotal * CustoMaoDeObraHora;
                decimal precoVenda = custosFisicos + lucro;
                if (precoVenda == 0) return 0;
                
                return (lucro / precoVenda) * 100m;
            }
        }
    }
}
