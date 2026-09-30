using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucroImpresso.Web.Models
{
    [Table("Acabamento")]
    public class Acabamento
    {
        [Key]
        [Column("id_acabamento")]
        public int Id { get; set; }

        [Required]
        [Column("fk_usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required(ErrorMessage = "O tipo de insumo é obrigatório.")]
        [MaxLength(50)]
        [Column("tipo")]
        public string Tipo { get; set; } = "Tinta";

        [Required(ErrorMessage = "O nome do insumo é obrigatório.")]
        [MaxLength(100)]
        [Column("nome_acabamento")]
        public string NomeAcabamento { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("cor")]
        public string Cor { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O peso/volume deve ser maior que zero.")]
        [Column("peso_litros")]
        public decimal PesoLitros { get; set; }

        [Required(ErrorMessage = "O valor pago é obrigatório.")]
        [Column("valor_pago")]
        public decimal ValorPago { get; set; }

        [Required(ErrorMessage = "O saldo é obrigatório.")]
        [Column("saldo_mililitros")]
        public decimal SaldoMililitros { get; set; }

        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public decimal PrecoPorMl => PesoLitros > 0 ? ValorPago / (PesoLitros * 1000m) : 0;
    }
}
