using FluentValidation;
using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Validators
{
    public class PecaValidator : AbstractValidator<Peca>
    {
        public PecaValidator()
        {
            RuleFor(p => p.NomePeca)
                .NotEmpty().WithMessage("O nome da peça é obrigatório.")
                .MaximumLength(200).WithMessage("O nome da peça não pode exceder 200 caracteres.");

            RuleFor(p => p.LinkArquivoStl)
                .Must(ValidarUrlSegura)
                .WithMessage("O link do arquivo STL deve ser uma URL válida iniciando com http:// ou https://.")
                .When(p => !string.IsNullOrWhiteSpace(p.LinkArquivoStl));
        }

        private bool ValidarUrlSegura(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
