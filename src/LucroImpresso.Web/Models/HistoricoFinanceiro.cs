using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucroImpresso.Web.Models
{
    [Table("HistoricoFinanceiro")]
    public class HistoricoFinanceiro
    {
        [Key]
        [Column("id_historico_financeiro")]
        public int Id { get; set; }

        [Required]
        [Column("fk_usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Column("receitas_acumuladas", TypeName = "decimal(18,2)")]
        public decimal ReceitasAcumuladas { get; set; } = 0;

        [Column("despesas_material_acumuladas", TypeName = "decimal(18,2)")]
        public decimal DespesasMaterialAcumuladas { get; set; } = 0;

        [Column("despesas_energia_acumuladas", TypeName = "decimal(18,2)")]
        public decimal DespesasEnergiaAcumuladas { get; set; } = 0;

        [Column("despesas_acabamento_acumuladas", TypeName = "decimal(18,2)")]
        public decimal DespesasAcabamentoAcumuladas { get; set; } = 0;

        // Campo 'data' conforme definido no DER
        [Column("data")]
        public DateTime Data { get; set; } = DateTime.UtcNow;
    }
}
