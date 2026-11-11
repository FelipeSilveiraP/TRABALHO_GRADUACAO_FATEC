using FluentValidation.TestHelper;
using LucroImpresso.Web.Models;
using LucroImpresso.Web.Validators;
using Xunit;

namespace LucroImpresso.Tests
{
    public class ValidadoresDominioTests
    {
        private readonly PecaValidator _pecaValidator = new();
        private readonly OrcamentoValidator _orcamentoValidator = new();
        private readonly ClienteValidator _clienteValidator = new();
        private readonly MaterialValidator _materialValidator = new();
        private readonly MaquinaValidator _maquinaValidator = new();
        private readonly AcabamentoValidator _acabamentoValidator = new();

        #region PecaValidator (V7 - Proteção XSS no Link STL)

        [Theory]
        [InlineData("javascript:alert('XSS')")]
        [InlineData("javascript:void(0)")]
        [InlineData("data:text/html,<script>alert(1)</script>")]
        [InlineData("vbscript:msgbox(1)")]
        [InlineData("ftp://arquivos.local/modelo.stl")]
        [InlineData("file:///C:/passwords.txt")]
        public void PecaValidator_LinksNaoHttpOuHttps_DevemSerRejeitados(string urlInsegura)
        {
            var peca = new Peca
            {
                NomePeca = "Peça Teste",
                LinkArquivoStl = urlInsegura,
                UsuarioId = 1
            };

            var resultado = _pecaValidator.TestValidate(peca);
            resultado.ShouldHaveValidationErrorFor(p => p.LinkArquivoStl)
                     .WithErrorMessage("O link do arquivo STL deve ser uma URL válida iniciando com http:// ou https://.");
        }

        [Theory]
        [InlineData("https://www.thingiverse.com/thing:12345")]
        [InlineData("http://cults3d.com/model.stl")]
        [InlineData("https://makerworld.com/models/123")]
        [InlineData("")]
        [InlineData(null)]
        public void PecaValidator_LinksValidosOuVazios_DevemPassar(string? urlValida)
        {
            var peca = new Peca
            {
                NomePeca = "Peça Teste",
                LinkArquivoStl = urlValida ?? string.Empty,
                UsuarioId = 1
            };

            var resultado = _pecaValidator.TestValidate(peca);
            resultado.ShouldNotHaveValidationErrorFor(p => p.LinkArquivoStl);
        }

        [Fact]
        public void PecaValidator_NomePecaVazioOuMuitoLongo_DeveFalhar()
        {
            var pecaVazia = new Peca { NomePeca = "", UsuarioId = 1 };
            _pecaValidator.TestValidate(pecaVazia).ShouldHaveValidationErrorFor(p => p.NomePeca);

            var pecaLonga = new Peca { NomePeca = new string('A', 201), UsuarioId = 1 };
            _pecaValidator.TestValidate(pecaLonga).ShouldHaveValidationErrorFor(p => p.NomePeca);
        }

        #endregion

        #region OrcamentoValidator (V9)

        [Fact]
        public void OrcamentoValidator_DadosValidos_DevePassar()
        {
            var orcamento = new Orcamento
            {
                NomeProjeto = "Luminária Artística",
                ClienteId = 1,
                PecaId = 1,
                MaquinaId = 1,
                MaterialId = 1,
                PesoGramas = 150.5m,
                TempoHrsImpressao = 6.5m,
                TempoHrsPinturaLixamento = 1.0m,
                CustoMaoDeObraHora = 35.00m,
                UsuarioId = 1
            };

            var resultado = _orcamentoValidator.TestValidate(orcamento);
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void OrcamentoValidator_PesoInvalido_DeveFalhar(int pesoInvalido)
        {
            var orcamento = new Orcamento { PesoGramas = pesoInvalido };
            _orcamentoValidator.TestValidate(orcamento).ShouldHaveValidationErrorFor(o => o.PesoGramas);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void OrcamentoValidator_TempoImpressaoInvalido_DeveFalhar(decimal tempoInvalido)
        {
            var orcamento = new Orcamento { TempoHrsImpressao = tempoInvalido };
            _orcamentoValidator.TestValidate(orcamento).ShouldHaveValidationErrorFor(o => o.TempoHrsImpressao);
        }

        [Fact]
        public void OrcamentoValidator_FksAusentes_DevemFalhar()
        {
            var orcamento = new Orcamento
            {
                ClienteId = 0,
                PecaId = 0,
                MaquinaId = 0,
                MaterialId = 0
            };

            var resultado = _orcamentoValidator.TestValidate(orcamento);
            resultado.ShouldHaveValidationErrorFor(o => o.ClienteId);
            resultado.ShouldHaveValidationErrorFor(o => o.PecaId);
            resultado.ShouldHaveValidationErrorFor(o => o.MaquinaId);
            resultado.ShouldHaveValidationErrorFor(o => o.MaterialId);
        }

        #endregion

        #region ClienteValidator (V9 - Herança XT e Telefone)

        [Fact]
        public void ClienteValidator_SomentePessoaFisica_DevePassar()
        {
            var cliente = new Cliente
            {
                UsuarioId = 1,
                CelularUnico = "(11) 98765-4321",
                PessoaFisica = new PessoaFisica { Nome = "Maria Silva", Cpf = "529.982.247-25" }
            };

            var resultado = _clienteValidator.TestValidate(cliente);
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void ClienteValidator_SomentePessoaJuridica_DevePassar()
        {
            var cliente = new Cliente
            {
                UsuarioId = 1,
                CelularUnico = "11987654321",
                PessoaJuridica = new PessoaJuridica { RazaoSocial = "Empresa ABC Ltda", Cnpj = "11.222.333/0001-81" }
            };

            var resultado = _clienteValidator.TestValidate(cliente);
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void ClienteValidator_AmbosPF_E_PJ_DeveFalharRegraXT()
        {
            var cliente = new Cliente
            {
                UsuarioId = 1,
                PessoaFisica = new PessoaFisica { Nome = "João" },
                PessoaJuridica = new PessoaJuridica { RazaoSocial = "João ME" }
            };

            var resultado = _clienteValidator.TestValidate(cliente);
            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage.Contains("Regra XT"));
        }

        [Fact]
        public void ClienteValidator_NenhumPF_Nem_PJ_DeveFalharRegraXT()
        {
            var cliente = new Cliente
            {
                UsuarioId = 1,
                PessoaFisica = null,
                PessoaJuridica = null
            };

            var resultado = _clienteValidator.TestValidate(cliente);
            Assert.False(resultado.IsValid);
            Assert.Contains(resultado.Errors, e => e.ErrorMessage.Contains("Regra XT"));
        }

        [Theory]
        [InlineData("ABC12345")]
        [InlineData("telefone-invalido!")]
        public void ClienteValidator_CelularFormatoInvalido_DeveFalhar(string celularInvalido)
        {
            var cliente = new Cliente
            {
                UsuarioId = 1,
                CelularUnico = celularInvalido,
                PessoaFisica = new PessoaFisica { Nome = "Teste" }
            };

            var resultado = _clienteValidator.TestValidate(cliente);
            resultado.ShouldHaveValidationErrorFor(c => c.CelularUnico);
        }

        [Theory]
        [InlineData("529.982.247-25", true)]   // CPF válido (com máscara)
        [InlineData("52998224725", true)]      // CPF válido (sem máscara)
        [InlineData("123.456.789-00", false)]  // Dígitos verificadores errados
        [InlineData("111.111.111-11", false)]  // Sequência repetida
        [InlineData("529.982.247", false)]     // Incompleto
        public void DocumentoValidador_Cpf_RF022(string cpf, bool esperado)
        {
            Assert.Equal(esperado, DocumentoValidador.CpfValido(cpf));
        }

        [Theory]
        [InlineData("11.222.333/0001-81", true)]   // CNPJ válido (com máscara)
        [InlineData("11222333000181", true)]       // CNPJ válido (sem máscara)
        [InlineData("12.345.678/0001-90", false)]  // Dígitos verificadores errados
        [InlineData("00.000.000/0000-00", false)]  // Sequência repetida
        public void DocumentoValidador_Cnpj_RF022(string cnpj, bool esperado)
        {
            Assert.Equal(esperado, DocumentoValidador.CnpjValido(cnpj));
        }

        [Fact]
        public void ClienteValidator_CpfInvalido_DeveFalhar()
        {
            var cliente = new Cliente
            {
                UsuarioId = 1,
                PessoaFisica = new PessoaFisica { Nome = "Maria Silva", Cpf = "123.456.789-00" }
            };

            Assert.False(_clienteValidator.TestValidate(cliente).IsValid);
        }

        #endregion

        #region Material, Maquina e Tinta Validators

        [Fact]
        public void MaterialValidator_SaldoMaiorQuePesoRolo_DeveFalhar()
        {
            var material = new Material
            {
                Nome = "PLA Premium",
                PesoRoloGramas = 1000,
                SaldoGramas = 1200, // Maior que o rolo
                ValorPago = 120.00m,
                UsuarioId = 1
            };

            var resultado = _materialValidator.TestValidate(material);
            resultado.ShouldHaveValidationErrorFor(m => m.SaldoGramas);
        }

        [Fact]
        public void MaquinaValidator_ValoresNegativos_DevemFalhar()
        {
            var maquina = new Maquina
            {
                Nome = "Ender 3 V3",
                ConsumoWatts = -50,
                CustoKwh = -0.85m,
                UsuarioId = 1
            };

            var resultado = _maquinaValidator.TestValidate(maquina);
            resultado.ShouldHaveValidationErrorFor(m => m.ConsumoWatts);
            resultado.ShouldHaveValidationErrorFor(m => m.CustoKwh);
        }

        [Fact]
        public void AcabamentoValidator_SaldoMaiorQueVolumeOriginal_DeveFalhar()
        {
            var acabamento = new Acabamento
            {
                Tipo = "Tinta",
                NomeAcabamento = "Acrílica Fosca",
                PesoLitros = 0.5m, // 500ml
                SaldoMililitros = 600m, // 600ml > 500ml
                ValorPago = 25.00m,
                UsuarioId = 1
            };

            var resultado = _acabamentoValidator.TestValidate(acabamento);
            resultado.ShouldHaveValidationErrorFor(t => t.SaldoMililitros);
        }

        [Fact]
        public void AcabamentoValidator_TipoVazio_DeveFalhar()
        {
            var acabamento = new Acabamento
            {
                Tipo = "",
                NomeAcabamento = "Primer Spray",
                PesoLitros = 0.4m,
                SaldoMililitros = 400m,
                ValorPago = 35.00m,
                UsuarioId = 1
            };

            var resultado = _acabamentoValidator.TestValidate(acabamento);
            resultado.ShouldHaveValidationErrorFor(t => t.Tipo);
        }

        [Theory]
        [InlineData("Tinta")]
        [InlineData("Primer")]
        [InlineData("Verniz")]
        [InlineData("Solvente")]
        [InlineData("Outro")]
        public void AcabamentoValidator_TiposValidos_DevePassar(string tipo)
        {
            var acabamento = new Acabamento
            {
                Tipo = tipo,
                NomeAcabamento = $"{tipo} Especial",
                PesoLitros = 1m,
                SaldoMililitros = 1000m,
                ValorPago = 50.00m,
                UsuarioId = 1
            };

            var resultado = _acabamentoValidator.TestValidate(acabamento);
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        #endregion
    }
}
