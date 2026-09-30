using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace LucroImpresso.Web.Services
{
    /// <summary>
    /// Serviço auxiliar que extrai o ID do usuário logado a partir dos claims do cookie de autenticação.
    /// Deve ser injetado como Scoped nas páginas Blazor Server.
    /// </summary>
    public class UsuarioContextService
    {
        private readonly AuthenticationStateProvider _authStateProvider;

        public UsuarioContextService(AuthenticationStateProvider authStateProvider)
        {
            _authStateProvider = authStateProvider;
        }

        /// <summary>
        /// Retorna o ID do usuário logado (0 se não autenticado ou claim ausente).
        /// </summary>
        public async Task<int> GetUsuarioIdAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int id))
                return id;

            return 0;
        }

        /// <summary>
        /// Retorna o nome de usuário logado.
        /// </summary>
        public async Task<string> GetNomeUsuarioAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.Identity?.Name ?? string.Empty;
        }
    }
}
