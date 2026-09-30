using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucroImpresso.Web.Models
{
    [Table("Material")]
    public class Material : IValidatableObject
    {
        [Column("id_material")]
        public int Id { get; set; }

        [Required]
        [Column("fk_usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required(ErrorMessage = "Nome do material é obrigatório.")]
        [MaxLength(100)]
        [Column("nome_material")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("cor")]
        public string Cor { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "O peso do rolo deve ser maior que zero.")]
        [Column("peso_rolo_gramas")]
        public decimal PesoRoloGramas { get; set; }

        [Required(ErrorMessage = "O valor pago é obrigatório.")]
        [Column("valor_pago")]
        public decimal ValorPago { get; set; }

        [Required(ErrorMessage = "O saldo é obrigatório.")]
        [Column("saldo_gramas")]
        public decimal SaldoGramas { get; set; }

        // Propriedade calculada — não gera coluna no banco
        [NotMapped]
        public decimal PrecoPorGrama => PesoRoloGramas > 0 ? ValorPago / PesoRoloGramas : 0;

        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SaldoGramas > PesoRoloGramas)
            {
                yield return new ValidationResult(
                    "O saldo atual não pode ser superior ao peso original do rolo.",
                    new[] { nameof(SaldoGramas) });
            }
        }
    }
}