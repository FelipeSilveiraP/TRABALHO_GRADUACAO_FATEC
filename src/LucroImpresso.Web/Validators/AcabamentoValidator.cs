using FluentValidation;
using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Validators
{
    public class AcabamentoValidator : AbstractValidator<Acabamento>
    {
        public AcabamentoValidator()
        {
            RuleFor(a => a.Tipo)
                .NotEmpty().WithMessage("O tipo de insumo é obrigatório.")
                .MaximumLength(50).WithMessage("O tipo de insumo não pode exceder 50 caracteres.");

            RuleFor(a => a.NomeAcabamento)
                .NotEmpty().WithMessage("O nome do insumo é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do insumo não pode exceder 100 caracteres.");

            RuleFor(a => a.PesoLitros)
                .GreaterThan(0).WithMessage("O volume/peso em litros deve ser maior que zero.")
                .LessThanOrEqualTo(1000).WithMessage("O volume não pode exceder 1.000 litros.");

            RuleFor(a => a.ValorPago)
                .GreaterThan(0).WithMessage("O valor pago deve ser maior que R$ 0,00.")
                .LessThanOrEqualTo(1000000).WithMessage("O valor pago não pode exceder R$ 1.000.000,00.");

            RuleFor(a => a.SaldoMililitros)
                .GreaterThanOrEqualTo(0).WithMessage("O saldo não pode ser negativo.")
                .LessThanOrEqualTo(a => a.PesoLitros * 1000m).WithMessage("O saldo atual não pode ser superior ao volume original do insumo.");
        }
    }
}
