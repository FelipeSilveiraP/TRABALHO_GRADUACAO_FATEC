using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucroImpresso.Web.Models
{
    [Table("PessoaJuridica")]
    public class PessoaJuridica
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

        // ── Dados empresariais ────────────────────────────────────────────
        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [MaxLength(18)]
        [Column("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [Required(ErrorMessage = "A razão social é obrigatória.")]
        [MaxLength(300)]
        [Column("razao_social")]
        public string RazaoSocial { get; set; } = string.Empty;
    }
}
