using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> AutenticarAsync(string nomeUsuario, string senha);
        Task<Usuario?> GetByNomeAsync(string nomeUsuario);
        Task<Usuario?> GetByIdAsync(int id);
        Task<bool> ExisteUsuarioAsync();
        Task CriarUsuarioPadraoAsync();
        Task UpdateAsync(Usuario usuario);
        Task<(bool Sucesso, string? MensagemErro)> CadastrarAsync(string nomeUsuario, string email, string senha);
        Task<bool> NomeUsuarioExisteAsync(string nomeUsuario);
        Task<bool> EmailExisteAsync(string email);

        /// <summary>
        /// Validação centralizada de força de senha (OWASP).
        /// Retorna null se válida, ou mensagem de erro se inválida.
        /// </summary>
        string? ValidarForcaSenha(string senha);

        /// <summary>
        /// Gera token criptográfico seguro de recuperação de senha e salva no banco com validade de 30 minutos.
        /// </summary>
        Task<(bool Sucesso, string? Token, string? NomeUsuario)> GerarTokenRecuperacaoAsync(string email);

        /// <summary>
        /// Obtém o usuário se o token de recuperação for válido e não tiver expirado.
        /// </summary>
        Task<Usuario?> ObterPorTokenValidoAsync(string token);

        /// <summary>
        /// Redefine a senha do usuário associado ao token, valida a nova senha e invalida o token.
        /// </summary>
        Task<(bool Sucesso, string? MensagemErro)> RedefinirSenhaAsync(string token, string novaSenha);
    }
}
