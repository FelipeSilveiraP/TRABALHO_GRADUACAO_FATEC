using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucroImpresso.Web.Models
{
    [Table("Maquina")]
    public class Maquina
    {
        [Column("id_maquina")]
        public int Id { get; set; }

        [Required]
        [Column("fk_usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required(ErrorMessage = "O nome da máquina é obrigatório.")]
        [MaxLength(100)]
        [Column("nome_maquina")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A potência em Watts é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A potência deve ser maior que zero.")]
        [Column("consumo_watts")]
        public decimal ConsumoWatts { get; set; }

        [Required(ErrorMessage = "A taxa de kWh é obrigatória.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O custo do kWh deve ser maior que zero.")]
        [Column("custo_kwh")]
        public decimal CustoKwh { get; set; }

        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
