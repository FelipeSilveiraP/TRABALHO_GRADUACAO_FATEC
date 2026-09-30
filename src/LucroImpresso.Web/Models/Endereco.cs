using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucroImpresso.Web.Models
{
    [Table("Endereco")]
    public class Endereco
    {
        [Column("id_endereco")]
        public int Id { get; set; }

        // ── Multi-Tenant ──────────────────────────────────────────────────
        [Required]
        [Column("fk_usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        // ── Relacionamento com Cliente ────────────────────────────────────
        [Required(ErrorMessage = "O cliente é obrigatório.")]
        [Column("fk_cliente")]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        // ── Atributos do endereço ─────────────────────────────────────────
        [MaxLength(300)]
        [Column("logradouro")]
        public string? Logradouro { get; set; }

        /// <summary>Número do logradouro. Nullable para permitir "S/N".</summary>
        [Column("numero")]
        public int? Numero { get; set; }

        [MaxLength(200)]
        [Column("bairro")]
        public string? Bairro { get; set; }

        [MaxLength(200)]
        [Column("cidade")]
        public string? Cidade { get; set; }

        [MaxLength(9)]
        [Column("cep")]
        public string? Cep { get; set; }

        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
