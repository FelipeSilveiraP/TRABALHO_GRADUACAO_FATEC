using LucroImpresso.Web.Models;
using LucroImpresso.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace LucroImpresso.Tests
{
    public class MultiTenantEInfraTests
    {
        private class MockEnvironment : IWebHostEnvironment
        {
            public string EnvironmentName { get; set; } = "Production";
            public string ApplicationName { get; set; } = "LucroImpresso";
            public string WebRootPath { get; set; } = string.Empty;
            public Microsoft.Extensions.FileProviders.IFileProvider WebRootFileProvider { get; set; } = null!;
            public string ContentRootPath { get; set; } = string.Empty;
            public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
        }

        [Fact]
        public void Models_NovasInstancias_DevemInicializarComDataUtc()
        {
            var antes = DateTime.UtcNow.AddSeconds(-1);

            var orcamento = new Orcamento();
            var material = new Material();
            var maquina = new Maquina();
            var acabamento = new Acabamento();
            var peca = new Peca();
            var cliente = new Cliente();
            var usuario = new Usuario();
            var endereco = new Endereco();
            var historico = new HistoricoFinanceiro();

            var depois = DateTime.UtcNow.AddSeconds(1);

            Assert.InRange(orcamento.DataCriacao, antes, depois);
            Assert.InRange(material.DataCadastro, antes, depois);
            Assert.InRange(maquina.DataCadastro, antes, depois);
            Assert.InRange(acabamento.DataCadastro, antes, depois);
            Assert.InRange(peca.DataCadastro, antes, depois);
            Assert.InRange(cliente.DataCadastro, antes, depois);
            Assert.InRange(usuario.DataCadastro, antes, depois);
            Assert.InRange(endereco.DataCadastro, antes, depois);
            Assert.InRange(historico.Data, antes, depois);

            Assert.Equal(DateTimeKind.Utc, orcamento.DataCriacao.Kind);
            Assert.Equal(DateTimeKind.Utc, usuario.DataCadastro.Kind);
        }

        [Fact]
        public async Task EmailService_EmProducao_NaoDeveExporUltimoLinkDev()
        {
            // Reset de estado
            EmailService.LimparLinkDev();

            // Arrange: Ambiente configurado como "Production"
            var mockEnv = new MockEnvironment { EnvironmentName = "Production" };
            var config = new ConfigurationBuilder().Build();
            var logger = NullLogger<EmailService>.Instance;
            var emailService = new EmailService(config, logger, mockEnv);

            // Act
            await emailService.EnviarEmailRecuperacaoSenhaAsync("teste@cliente.com", "teste", "https://app.com/redefinir?token=xyz");

            // Assert: Em produção, o link NUNCA deve ser armazenado em memória
            Assert.Null(EmailService.UltimoLinkGeradoDev);
        }

        [Fact]
        public async Task EmailService_EmDesenvolvimento_DevePermitirLinkDev()
        {
            // Reset de estado
            EmailService.LimparLinkDev();

            // Arrange: Ambiente configurado como "Development"
            var mockEnv = new MockEnvironment { EnvironmentName = "Development" };
            var config = new ConfigurationBuilder().Build();
            var logger = NullLogger<EmailService>.Instance;
            var emailService = new EmailService(config, logger, mockEnv);

            string linkEsperado = "https://localhost:5067/redefinir?token=token-teste-dev";

            // Act
            await emailService.EnviarEmailRecuperacaoSenhaAsync("dev@teste.com", "devuser", linkEsperado);

            // Assert: Em dev, o link deve estar disponível para testes locais
            Assert.Equal(linkEsperado, EmailService.UltimoLinkGeradoDev);
        }

        [Theory]
        [InlineData(typeof(Orcamento), nameof(Orcamento.NomeProjeto), 200)]
        [InlineData(typeof(Material), nameof(Material.Nome), 100)]
        [InlineData(typeof(Material), nameof(Material.Cor), 50)]
        [InlineData(typeof(Maquina), nameof(Maquina.Nome), 100)]
        [InlineData(typeof(Acabamento), nameof(Acabamento.NomeAcabamento), 100)]
        [InlineData(typeof(Acabamento), nameof(Acabamento.Cor), 50)]
        [InlineData(typeof(Peca), nameof(Peca.NomePeca), 200)]
        [InlineData(typeof(Peca), nameof(Peca.LinkArquivoStl), 2000)]
        [InlineData(typeof(Usuario), nameof(Usuario.NomeUsuario), 50)]
        [InlineData(typeof(Usuario), nameof(Usuario.Email), 100)]
        public void Models_CamposTextoCriticos_DevemPossuirAtributoMaxLength(Type modelType, string propertyName, int maxEsperado)
        {
            var prop = modelType.GetProperty(propertyName);
            Assert.NotNull(prop);

            var attr = prop.GetCustomAttributes(typeof(MaxLengthAttribute), false).FirstOrDefault() as MaxLengthAttribute;
            Assert.NotNull(attr);
            Assert.Equal(maxEsperado, attr.Length);
        }
    }
}
