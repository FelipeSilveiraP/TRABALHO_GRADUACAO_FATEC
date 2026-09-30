using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucroImpresso.Web.Models
{
    [Table("Cliente")]
    public class Cliente : IValidatableObject
    {
        [Column("id_cliente")]
        public int Id { get; set; }

        [Required]
        [Column("fk_usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [MaxLength(20)]
        [Column("celular_unico")]
        public string? CelularUnico { get; set; }

        public Endereco? Endereco { get; set; }

        // Navegação para especialização (XT — exclusiva total)
        public PessoaFisica? PessoaFisica { get; set; }
        public PessoaJuridica? PessoaJuridica { get; set; }

        public ICollection<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();

        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        // Propriedade auxiliar para exibir o nome do cliente independente do tipo
        [NotMapped]
        public string NomeExibicao =>
            PessoaFisica?.Nome
            ?? PessoaJuridica?.RazaoSocial
            ?? $"Cliente #{Id}";

        [NotMapped]
        public string Documento =>
            PessoaFisica?.Cpf
            ?? PessoaJuridica?.Cnpj
            ?? string.Empty;

        [NotMapped]
        public string TipoExibicao =>
            PessoaFisica != null ? "PF" : PessoaJuridica != null ? "PJ" : "—";

        [NotMapped]
        public string CelularExibicao => CelularUnico ?? string.Empty;
        // ── Validação da Herança XT (Exclusiva Total) ──────────────────────────
        // DER: Todo Cliente DEVE ser exatamente PF ou PJ (não ambos, não nenhum)
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            bool temPF = PessoaFisica != null;
            bool temPJ = PessoaJuridica != null;

            if (!temPF && !temPJ)
                yield return new ValidationResult(
                    "Todo cliente deve ser Pessoa Física ou Pessoa Jurídica (herança XT: totalidade).",
                    new[] { nameof(PessoaFisica), nameof(PessoaJuridica) });

            if (temPF && temPJ)
                yield return new ValidationResult(
                    "Um cliente não pode ser simultaneamente Pessoa Física e Pessoa Jurídica (herança XT: exclusividade).",
                    new[] { nameof(PessoaFisica), nameof(PessoaJuridica) });
        }
    }
}
