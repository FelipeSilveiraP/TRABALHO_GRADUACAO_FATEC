using FluentValidation;
using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Validators
{
    public class ClienteValidator : AbstractValidator<Cliente>
    {
        public ClienteValidator()
        {
            RuleFor(c => c.CelularUnico)
                .MaximumLength(20).WithMessage("O telefone/celular não pode exceder 20 caracteres.")
                .Matches(@"^(\+?[0-9\s\-\(\)]{8,20})?$").WithMessage("Formato de celular inválido.")
                .When(c => !string.IsNullOrWhiteSpace(c.CelularUnico));

            // RF022: integridade dos documentos pelos dígitos verificadores
            RuleFor(c => c.PessoaFisica!.Cpf)
                .Must(DocumentoValidador.CpfValido).WithMessage("CPF inválido. Verifique os dígitos informados.")
                .When(c => c.PessoaFisica != null);

            RuleFor(c => c.PessoaJuridica!.Cnpj)
                .Must(DocumentoValidador.CnpjValido).WithMessage("CNPJ inválido. Verifique os dígitos informados.")
                .When(c => c.PessoaJuridica != null);

            // Regra XT: Totalidade e Exclusividade (Exatamente PF ou PJ, nunca ambos, nunca nenhum)
            RuleFor(c => c)
                .Must(c => (c.PessoaFisica != null) ^ (c.PessoaJuridica != null))
                .WithMessage("O cliente deve ser exclusivamente Pessoa Física OU Pessoa Jurídica (Regra XT).");
        }
    }
}
