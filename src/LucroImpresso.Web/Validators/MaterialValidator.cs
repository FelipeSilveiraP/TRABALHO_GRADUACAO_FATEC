using FluentValidation;
using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Validators
{
    public class MaterialValidator : AbstractValidator<Material>
    {
        public MaterialValidator()
        {
            RuleFor(m => m.Nome)
                .NotEmpty().WithMessage("O nome do material é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do material não pode exceder 100 caracteres.");

            RuleFor(m => m.PesoRoloGramas)
                .GreaterThan(0).WithMessage("O peso do rolo deve ser maior que zero (ex: 1000g).")
                .LessThanOrEqualTo(100000).WithMessage("O peso do rolo não pode exceder 100.000g (100kg).");

            RuleFor(m => m.ValorPago)
                .GreaterThan(0).WithMessage("O valor pago deve ser maior que R$ 0,00.")
                .LessThanOrEqualTo(1000000).WithMessage("O valor pago não pode exceder R$ 1.000.000,00.");

            RuleFor(m => m.SaldoGramas)
                .GreaterThanOrEqualTo(0).WithMessage("O saldo não pode ser negativo.")
                .LessThanOrEqualTo(m => m.PesoRoloGramas).WithMessage("O saldo atual não pode ser superior ao peso original do rolo.");
        }
    }
}
