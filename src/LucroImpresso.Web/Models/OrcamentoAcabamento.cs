using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucroImpresso.Web.Models
{
    [Table("ORC_ACABAMENTO")]
    public class OrcamentoAcabamento
    {
        [Column("fk_orcamento")]
        public int OrcamentoId { get; set; }
        
        [ForeignKey("OrcamentoId")]
        public Orcamento? Orcamento { get; set; }

        [Column("fk_acabamento")]
        public int AcabamentoId { get; set; }
        
        [ForeignKey("AcabamentoId")]
        public Acabamento? Acabamento { get; set; }

        [Column("fk_usuario")]
        public int UsuarioId { get; set; }
        
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        [Column("quantidade_ml")]
        public decimal QuantidadeMl { get; set; }

        [NotMapped]
        public string BuscaTexto { get; set; } = string.Empty;
    }
}
