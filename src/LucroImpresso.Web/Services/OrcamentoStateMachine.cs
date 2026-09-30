using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Services
{
    /// <summary>
    /// Serviço de Domínio responsável por gerenciar e governar o Ciclo de Vida e a Máquina de Estados
    /// da entidade Orcamento / Ordem de Produção no LucroImpresso, aplicando as Regras de Negócio
    /// (RN001 a RN009) para transições válidas, condições de guarda e efeitos colaterais de estoque.
    /// </summary>
    public class OrcamentoStateMachine
    {
        private static readonly Dictionary<StatusOrcamento, List<StatusOrcamento>> TransicoesPermitidas = new()
        {
            [StatusOrcamento.Pendente] = new() { StatusOrcamento.EmImpressao },
            [StatusOrcamento.EmImpressao] = new() { StatusOrcamento.Concluido, StatusOrcamento.Falha },
            [StatusOrcamento.Falha] = new() { StatusOrcamento.EmImpressao }, // Reimpressão permitida
            [StatusOrcamento.Concluido] = new() // Estado final operacional (imutável)
        };

        /// <summary>
        /// Verifica se a transição entre o status de origem e o status de destino é válida segundo a especificação UML.
        /// </summary>
        public bool PodeTransitar(StatusOrcamento origem, StatusOrcamento destino)
        {
            if (origem == destino) return true;
            return TransicoesPermitidas.TryGetValue(origem, out var destinosValidos) && destinosValidos.Contains(destino);
        }

        /// <summary>
        /// Retorna a lista de transições operacionais permitidas a partir do estado atual.
        /// </summary>
        public IReadOnlyList<StatusOrcamento> ObterProximosStatusValidos(StatusOrcamento statusAtual)
        {
            if (TransicoesPermitidas.TryGetValue(statusAtual, out var lista))
                return lista.AsReadOnly();

            return Array.Empty<StatusOrcamento>();
        }

        /// <summary>
        /// Indica se a transição de estado exige confirmação explícita em tela do usuário (RN006).
        /// </summary>
        public bool ExigeConfirmacaoExplicita(StatusOrcamento novoStatus)
        {
            return novoStatus is StatusOrcamento.Concluido or StatusOrcamento.Falha;
        }

        /// <summary>
        /// Indica se a transição para o novo status dispara o gatilho de dedução atômica no estoque físico (RN005).
        /// </summary>
        public bool DisparaBaixaEstoque(StatusOrcamento novoStatus)
        {
            return novoStatus is StatusOrcamento.Concluido or StatusOrcamento.Falha;
        }

        /// <summary>
        /// Executa a transição do orçamento para o estado EmImpressão.
        /// Trava alterações de preço e aloca a máquina na fila operacional.
        /// </summary>
        public void TransitarParaImpressao(Orcamento orcamento)
        {
            ArgumentNullException.ThrowIfNull(orcamento);

            if (!PodeTransitar(orcamento.Status, StatusOrcamento.EmImpressao))
            {
                throw new InvalidOperationException($"Transição inválida: Não é possível mover o orçamento de '{orcamento.Status}' para 'EmImpressao'.");
            }

            orcamento.Status = StatusOrcamento.EmImpressao;
        }

        /// <summary>
        /// Executa a transição do orçamento para Concluído com sucesso (RN003, RN005).
        /// Aplica a baixa física de matéria-prima (g) e de insumos de acabamento (mL).
        /// </summary>
        public void TransitarParaConcluido(Orcamento orcamento, Material? material, IEnumerable<OrcamentoAcabamento>? acabamentos)
        {
            ArgumentNullException.ThrowIfNull(orcamento);

            if (!PodeTransitar(orcamento.Status, StatusOrcamento.Concluido))
            {
                throw new InvalidOperationException($"Transição inválida: Não é possível concluir um orçamento em status '{orcamento.Status}'.");
            }

            // RN005: Gatilho de Baixa Física de Estoque de Material
            if (material != null)
            {
                material.SaldoGramas -= (decimal)orcamento.PesoGramas;
                if (material.SaldoGramas < 0) material.SaldoGramas = 0;
            }

            // RN005: Gatilho de Baixa Física de Estoque de Insumos de Acabamento (mL)
            if (acabamentos != null)
            {
                foreach (var oa in acabamentos)
                {
                    if (oa.Acabamento != null)
                    {
                        oa.Acabamento.SaldoMililitros -= oa.QuantidadeMl;
                        if (oa.Acabamento.SaldoMililitros < 0) oa.Acabamento.SaldoMililitros = 0;
                    }
                }
            }

            orcamento.Status = StatusOrcamento.Concluido;
        }

        /// <summary>
        /// Executa a transição do orçamento para Falha de Impressão ("Espaguete") (RN005).
        /// Aplica a dedução dos materiais desperdiçados no sinistro.
        /// </summary>
        public void TransitarParaFalha(Orcamento orcamento, Material? material, IEnumerable<OrcamentoAcabamento>? acabamentos)
        {
            ArgumentNullException.ThrowIfNull(orcamento);

            if (!PodeTransitar(orcamento.Status, StatusOrcamento.Falha))
            {
                throw new InvalidOperationException($"Transição inválida: Não é possível registrar falha em um orçamento com status '{orcamento.Status}'.");
            }

            // RN005: Baixa de perda física no estoque
            if (material != null)
            {
                material.SaldoGramas -= (decimal)orcamento.PesoGramas;
                if (material.SaldoGramas < 0) material.SaldoGramas = 0;
            }

            if (acabamentos != null)
            {
                foreach (var oa in acabamentos)
                {
                    if (oa.Acabamento != null)
                    {
                        oa.Acabamento.SaldoMililitros -= oa.QuantidadeMl;
                        if (oa.Acabamento.SaldoMililitros < 0) oa.Acabamento.SaldoMililitros = 0;
                    }
                }
            }

            orcamento.Status = StatusOrcamento.Falha;
        }

        /// <summary>
        /// Avalia se um orçamento está elegível para a rotina diária de expurgo por tempo de inatividade (RN007).
        /// </summary>
        public bool PodeSerExpurgado(Orcamento orcamento, int diasLimite = 180)
        {
            ArgumentNullException.ThrowIfNull(orcamento);
            return orcamento.DataCriacao < DateTime.UtcNow.AddDays(-diasLimite);
        }
    }
}
