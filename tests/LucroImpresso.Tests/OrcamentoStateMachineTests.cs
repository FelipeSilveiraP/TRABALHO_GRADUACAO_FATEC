using LucroImpresso.Web.Models;
using LucroImpresso.Web.Services;
using Xunit;

namespace LucroImpresso.Tests
{
    public class OrcamentoStateMachineTests
    {
        private readonly OrcamentoStateMachine _stateMachine = new();

        [Theory]
        [InlineData(StatusOrcamento.Pendente, StatusOrcamento.EmImpressao)]
        [InlineData(StatusOrcamento.EmImpressao, StatusOrcamento.Concluido)]
        [InlineData(StatusOrcamento.EmImpressao, StatusOrcamento.Falha)]
        [InlineData(StatusOrcamento.Falha, StatusOrcamento.EmImpressao)]
        [InlineData(StatusOrcamento.Pendente, StatusOrcamento.Pendente)]
        public void PodeTransitar_TransicoesValidas_DevemRetornarTrue(StatusOrcamento origem, StatusOrcamento destino)
        {
            Assert.True(_stateMachine.PodeTransitar(origem, destino));
        }

        [Theory]
        [InlineData(StatusOrcamento.Pendente, StatusOrcamento.Concluido)] // Pular impressão não permitido
        [InlineData(StatusOrcamento.Pendente, StatusOrcamento.Falha)]     // Falhar sem imprimir não permitido
        [InlineData(StatusOrcamento.Concluido, StatusOrcamento.EmImpressao)] // Concluído é final
        [InlineData(StatusOrcamento.Concluido, StatusOrcamento.Pendente)]
        [InlineData(StatusOrcamento.Concluido, StatusOrcamento.Falha)]
        [InlineData(StatusOrcamento.Falha, StatusOrcamento.Concluido)]
        public void PodeTransitar_TransicoesInvalidas_DevemRetornarFalse(StatusOrcamento origem, StatusOrcamento destino)
        {
            Assert.False(_stateMachine.PodeTransitar(origem, destino));
        }

        [Fact]
        public void TransitarParaImpressao_DePendente_DeveAlterarStatusComSucesso()
        {
            var orcamento = new Orcamento { Status = StatusOrcamento.Pendente };

            _stateMachine.TransitarParaImpressao(orcamento);

            Assert.Equal(StatusOrcamento.EmImpressao, orcamento.Status);
        }

        [Fact]
        public void TransitarParaImpressao_DeConcluido_DeveLancarExcecao()
        {
            var orcamento = new Orcamento { Status = StatusOrcamento.Concluido };

            Assert.Throws<InvalidOperationException>(() => _stateMachine.TransitarParaImpressao(orcamento));
        }

        [Fact]
        public void TransitarParaConcluido_DeveDeduzirEstoqueFisicoENaoPermitirSaldoNegativo()
        {
            // Arrange (RN003, RN005)
            var orcamento = new Orcamento
            {
                Status = StatusOrcamento.EmImpressao,
                PesoGramas = 150
            };

            var material = new Material
            {
                SaldoGramas = 200 // 200 - 150 = 50
            };

            var acabamento = new Acabamento
            {
                SaldoMililitros = 20
            };

            var acabamentos = new List<OrcamentoAcabamento>
            {
                new() { Acabamento = acabamento, QuantidadeMl = 30 } // 20 - 30 = -10 -> trava em 0
            };

            // Act
            _stateMachine.TransitarParaConcluido(orcamento, material, acabamentos);

            // Assert
            Assert.Equal(StatusOrcamento.Concluido, orcamento.Status);
            Assert.Equal(50, material.SaldoGramas);
            Assert.Equal(0, acabamento.SaldoMililitros); // Garantia de não-negatividade
        }

        [Fact]
        public void TransitarParaFalha_DeveDeduzirMaterialConsumido()
        {
            // Arrange (RN005)
            var orcamento = new Orcamento
            {
                Status = StatusOrcamento.EmImpressao,
                PesoGramas = 80
            };

            var material = new Material
            {
                SaldoGramas = 100
            };

            // Act
            _stateMachine.TransitarParaFalha(orcamento, material, null);

            // Assert
            Assert.Equal(StatusOrcamento.Falha, orcamento.Status);
            Assert.Equal(20, material.SaldoGramas);
        }

        [Theory]
        [InlineData(StatusOrcamento.Concluido, true)]
        [InlineData(StatusOrcamento.Falha, true)]
        [InlineData(StatusOrcamento.Pendente, false)]
        [InlineData(StatusOrcamento.EmImpressao, false)]
        public void ExigeConfirmacaoExplicita_StatusCriticos_DeveValidarRN006(StatusOrcamento status, bool esperado)
        {
            Assert.Equal(esperado, _stateMachine.ExigeConfirmacaoExplicita(status));
            Assert.Equal(esperado, _stateMachine.DisparaBaixaEstoque(status));
        }

        [Fact]
        public void PodeSerExpurgado_AbaixoDe180Dias_DeveRetornarFalse()
        {
            var orcamento = new Orcamento
            {
                DataCriacao = DateTime.UtcNow.AddDays(-60)
            };

            Assert.False(_stateMachine.PodeSerExpurgado(orcamento, 180));
        }

        [Fact]
        public void PodeSerExpurgado_AcimaDe180Dias_DeveRetornarTrue()
        {
            var orcamento = new Orcamento
            {
                DataCriacao = DateTime.UtcNow.AddDays(-185)
            };

            Assert.True(_stateMachine.PodeSerExpurgado(orcamento, 180));
        }
    }
}
