using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucroImpresso.Web.Models
{
    [Table("PessoaFisica")]
    public class PessoaFisica
    {
        // PK compartilhada com Cliente (herança 1:1, lado filho)
        // Serve ao mesmo tempo como PK e FK → nome segue o padrão FK da entidade pai
        [Key]
        [Column("fk_cliente")]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        // ── Multi-Tenant ──────────────────────────────────────────────────
        [Required]
        [Column("fk_usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        // ── Dados pessoais ────────────────────────────────────────────────
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [MaxLength(14)]
        [Column("cpf")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MaxLength(200)]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;
    }
}
