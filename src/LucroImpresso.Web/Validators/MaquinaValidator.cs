using FluentValidation;
using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Validators
{
    public class MaquinaValidator : AbstractValidator<Maquina>
    {
        public MaquinaValidator()
        {
            RuleFor(m => m.Nome)
                .NotEmpty().WithMessage("O nome da máquina é obrigatório.")
                .MaximumLength(100).WithMessage("O nome da máquina não pode exceder 100 caracteres.");

            RuleFor(m => m.ConsumoWatts)
                .GreaterThan(0).WithMessage("A potência em Watts deve ser maior que zero.")
                .LessThanOrEqualTo(50000).WithMessage("A potência não pode exceder 50.000W.");

            RuleFor(m => m.CustoKwh)
                .GreaterThan(0).WithMessage("O custo do kWh deve ser maior que R$ 0,00.")
                .LessThanOrEqualTo(100).WithMessage("O custo do kWh não pode exceder R$ 100,00.");
        }
    }
}
