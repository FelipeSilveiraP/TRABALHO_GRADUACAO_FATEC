using LucroImpresso.Web.Repositories;
using System.Security.Cryptography;
using Xunit;

namespace LucroImpresso.Tests
{
    public class SegurancaAutenticacaoTests
    {
        [Theory]
        [InlineData("1234567")] // Menos de 8 caracteres
        [InlineData("abcdefgh")] // Sem maiúscula, número ou especial
        [InlineData("ABCDEFGH")] // Sem minúscula, número ou especial
        [InlineData("Abcdefgh")] // Sem número ou especial
        [InlineData("Abcdefg1")] // Sem caractere especial
        [InlineData("abcdef1!")] // Sem maiúscula
        [InlineData("ABCDEF1!")] // Sem minúscula
        [InlineData("")]         // Vazio
        [InlineData(null)]       // Nulo
        public void ValidarForcaSenha_SenhasFracas_DevemRetornarErro(string? senha)
        {
            // Act
            var resultado = UsuarioRepository.ValidarForcaSenhaStatic(senha);

            // Assert
            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado);
        }

        [Theory]
        [InlineData("Admin@2026!")]
        [InlineData("Senha#Forte123")]
        [InlineData("P@ssw0rdSegura")]
        [InlineData("L#cr0!mpr3ss0*")]
        public void ValidarForcaSenha_SenhasFortes_DevemPassar(string senha)
        {
            // Act
            var resultado = UsuarioRepository.ValidarForcaSenhaStatic(senha);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public void BCrypt_HashEVerificacao_DeveFuncionarComSaltSeguro()
        {
            // Arrange
            string senhaOriginal = "Senha@Segura#2026";

            // Act
            string hash1 = BCrypt.Net.BCrypt.HashPassword(senhaOriginal);
            string hash2 = BCrypt.Net.BCrypt.HashPassword(senhaOriginal);

            // Assert
            // Salts automáticos geram hashes diferentes para a mesma senha
            Assert.NotEqual(hash1, hash2);

            // Ambos devem validar com sucesso
            Assert.True(BCrypt.Net.BCrypt.Verify(senhaOriginal, hash1));
            Assert.True(BCrypt.Net.BCrypt.Verify(senhaOriginal, hash2));

            // Senha incorreta deve falhar
            Assert.False(BCrypt.Net.BCrypt.Verify("SenhaErrada#123", hash1));
        }

        [Fact]
        public void HashToken_DeveSerDeterministicoEDiferenteDoTokenOriginal()
        {
            string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLower();

            string hash1 = UsuarioRepository.HashToken(token);
            string hash2 = UsuarioRepository.HashToken(token);

            // Mesmo token → mesmo hash (permite a busca no banco)
            Assert.Equal(hash1, hash2);
            // O banco nunca guarda o token puro
            Assert.NotEqual(token, hash1);
            // SHA-256 em hexadecimal = 64 caracteres
            Assert.Equal(64, hash1.Length);
            // Tokens diferentes → hashes diferentes
            Assert.NotEqual(hash1, UsuarioRepository.HashToken(token + "x"));
        }

        [Fact]
        public void GeracaoTokenCriptografico_DeveTerAltaEntropiaEFormatoValido()
        {
            // Arrange & Act
            var tokens = new HashSet<string>();
            for (int i = 0; i < 100; i++)
            {
                byte[] bytes = RandomNumberGenerator.GetBytes(32);
                string token = Convert.ToBase64String(bytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .TrimEnd('=');

                tokens.Add(token);

                // Assert individual
                Assert.True(token.Length >= 40, "Token deve ter pelo menos 40 caracteres base64url.");
                Assert.DoesNotContain("+", token);
                Assert.DoesNotContain("/", token);
                Assert.DoesNotContain("=", token);
            }

            // Assert conjunto: Todos os 100 tokens devem ser absolutamente únicos
            Assert.Equal(100, tokens.Count);
        }
    }
}
