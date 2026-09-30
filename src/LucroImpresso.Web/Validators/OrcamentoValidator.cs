using FluentValidation;
using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Validators
{
    public class OrcamentoValidator : AbstractValidator<Orcamento>
    {
        public OrcamentoValidator()
        {
            RuleFor(o => o.NomeProjeto)
                .NotEmpty().WithMessage("O nome do projeto é obrigatório.")
                .MaximumLength(200).WithMessage("O nome do projeto não pode exceder 200 caracteres.");

            RuleFor(o => o.ClienteId)
                .GreaterThan(0).WithMessage("Selecione um cliente válido.");

            RuleFor(o => o.PecaId)
                .GreaterThan(0).WithMessage("Selecione uma peça válida.");

            RuleFor(o => o.MaquinaId)
                .GreaterThan(0).WithMessage("Selecione uma máquina válida.");

            RuleFor(o => o.MaterialId)
                .GreaterThan(0).WithMessage("Selecione um material válido.");

            RuleFor(o => o.PesoGramas)
                .GreaterThan(0).WithMessage("O peso deve ser maior que zero (ex: 50g).")
                .LessThanOrEqualTo(100000).WithMessage("O peso não pode exceder 100.000g.");

            RuleFor(o => o.TempoHrsImpressao)
                .GreaterThan(0).WithMessage("O tempo de impressão deve ser maior que zero.")
                .LessThanOrEqualTo(1000).WithMessage("O tempo de impressão não pode exceder 1.000 horas.");

            RuleFor(o => o.TempoHrsPinturaLixamento)
                .GreaterThanOrEqualTo(0).WithMessage("O tempo de pós-processamento não pode ser negativo.")
                .LessThanOrEqualTo(500).WithMessage("O tempo de pós-processamento não pode exceder 500 horas.");

            RuleFor(o => o.CustoMaoDeObraHora)
                .GreaterThanOrEqualTo(0).WithMessage("O custo da mão de obra por hora não pode ser negativo.")
                .LessThanOrEqualTo(10000).WithMessage("O custo da mão de obra não pode exceder R$ 10.000,00/hora.");
        }
    }
}
