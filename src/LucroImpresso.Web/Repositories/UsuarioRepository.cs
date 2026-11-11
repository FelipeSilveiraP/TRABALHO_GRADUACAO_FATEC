using LucroImpresso.Web.Data;
using LucroImpresso.Web.Interfaces;
using LucroImpresso.Web.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace LucroImpresso.Web.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsuarioRepository> _logger;
        private readonly IConfiguration _configuration;

        public UsuarioRepository(AppDbContext context, ILogger<UsuarioRepository> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<Usuario?> AutenticarAsync(string nomeUsuario, string senha)
        {
            // 1. Busca o usuário apenas pelo nome
            var u = await _context.Usuarios.FirstOrDefaultAsync(x => x.NomeUsuario == nomeUsuario);

            // 2. Verifica se achou o usuário E se a senha digitada bate com o Hash salvo no banco
            if (u != null && BCrypt.Net.BCrypt.Verify(senha, u.Senha))
            {
                _logger.LogInformation("Login bem-sucedido para usuário '{NomeUsuario}'.", nomeUsuario);
                return u; // Autenticado com sucesso
            }

            // Log de tentativa falhada (sem revelar se o usuário existe ou não)
            _logger.LogWarning("Tentativa de login falhada para nome de usuário '{NomeUsuario}'.", nomeUsuario);
            return null; // Falha na autenticação
        }

        public async Task<Usuario?> GetByNomeAsync(string nomeUsuario)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NomeUsuario == nomeUsuario);
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExisteUsuarioAsync()
        {
            return await _context.Usuarios.AnyAsync();
        }

        public async Task CriarUsuarioPadraoAsync()
        {
            var admin = await _context.Usuarios.FirstOrDefaultAsync(u => u.NomeUsuario == "admin");
            // SEGURANÇA V3: Senha lida EXCLUSIVAMENTE da configuração (appsettings.json, que NÃO é versionado).
            // Sem fallback hardcoded: se a chave não existir, o admin padrão não é criado/alterado.
            var senhaAdmin = _configuration["AdminSettings:SenhaPadrao"];
            if (string.IsNullOrWhiteSpace(senhaAdmin))
            {
                if (admin == null)
                    _logger.LogWarning("AdminSettings:SenhaPadrao não configurada — usuário admin padrão não foi criado. Use a tela de cadastro ou configure a chave no appsettings.json.");
                return;
            }

            if (ValidarForcaSenhaStatic(senhaAdmin) is { } erroSenhaAdmin)
            {
                _logger.LogWarning("AdminSettings:SenhaPadrao ignorada por não atender à política de senha: {Erro}", erroSenhaAdmin);
                return;
            }

            string hashSenhaForte = BCrypt.Net.BCrypt.HashPassword(senhaAdmin);

            if (admin == null)
            {
                await _context.Usuarios.AddAsync(new Usuario
                {
                    NomeUsuario = "admin",
                    Email = "admin@lucroimpresso.local",
                    Senha = hashSenhaForte
                });
                await _context.SaveChangesAsync();
                _logger.LogInformation("Usuário admin padrão criado com senha forte.");
            }
            else if (admin.Senha == "senha_temporaria" || string.IsNullOrEmpty(admin.Email))
            {
                admin.Senha = hashSenhaForte;
                if (string.IsNullOrEmpty(admin.Email))
                    admin.Email = "admin@lucroimpresso.local";
                _context.Usuarios.Update(admin);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Usuário admin atualizado com senha forte e email.");
            }
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.ChangeTracker.Clear();
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> NomeUsuarioExisteAsync(string nomeUsuario)
        {
            if (string.IsNullOrWhiteSpace(nomeUsuario))
                return false;

            var nomeSanitizado = nomeUsuario.Trim().ToLower();
            return await _context.Usuarios
                .AnyAsync(u => u.NomeUsuario.ToLower() == nomeSanitizado);
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var emailSanitizado = email.Trim().ToLower();
            return await _context.Usuarios
                .AnyAsync(u => u.Email.ToLower() == emailSanitizado);
        }

        // ══════════════════════════════════════════════════════════════════
        //  VALIDAÇÃO CENTRALIZADA DE FORÇA DE SENHA (OWASP)
        //  Retorna null se válida, ou mensagem de erro se inválida.
        //  Usada tanto no Cadastro quanto nas Configurações.
        // ══════════════════════════════════════════════════════════════════
        public static string? ValidarForcaSenhaStatic(string? senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                return "A senha é obrigatória.";

            if (senha.Length < 8)
                return "A senha deve ter no mínimo 8 caracteres.";

            if (senha.Length > 64)
                return "A senha não pode ter mais de 64 caracteres.";

            if (!Regex.IsMatch(senha, @"[A-Z]"))
                return "A senha deve conter pelo menos 1 letra maiúscula.";

            if (!Regex.IsMatch(senha, @"[a-z]"))
                return "A senha deve conter pelo menos 1 letra minúscula.";

            if (!Regex.IsMatch(senha, @"[0-9]"))
                return "A senha deve conter pelo menos 1 número.";

            if (!Regex.IsMatch(senha, @"[!@#$%^&*?_\-]"))
                return "A senha deve conter pelo menos 1 caractere especial (!@#$%^&*?_-).";

            return null; // Senha forte ✅
        }

        public string? ValidarForcaSenha(string senha) => ValidarForcaSenhaStatic(senha);

        // ══════════════════════════════════════════════════════════════════
        //  VALIDAÇÃO DE NOME DE USUÁRIO
        // ══════════════════════════════════════════════════════════════════
        private string? ValidarNomeUsuario(string nomeUsuario)
        {
            if (string.IsNullOrWhiteSpace(nomeUsuario))
                return "O nome de usuário é obrigatório.";

            if (nomeUsuario.Length < 3)
                return "O nome de usuário deve ter pelo menos 3 caracteres.";

            if (nomeUsuario.Length > 30)
                return "O nome de usuário não pode ter mais de 30 caracteres.";

            // Permite apenas letras, números, underscores e pontos
            if (!Regex.IsMatch(nomeUsuario, @"^[a-zA-Z0-9_.]+$"))
                return "O nome de usuário deve conter apenas letras, números, underscores (_) e pontos (.).";

            // Não pode começar ou terminar com _ ou .
            if (nomeUsuario.StartsWith('_') || nomeUsuario.StartsWith('.') ||
                nomeUsuario.EndsWith('_') || nomeUsuario.EndsWith('.'))
                return "O nome de usuário não pode começar ou terminar com _ ou ponto.";

            // Não pode conter __ ou .. consecutivos
            if (nomeUsuario.Contains("__") || nomeUsuario.Contains(".."))
                return "O nome de usuário não pode conter __ ou .. consecutivos.";

            return null; // Nome válido ✅
        }

        // ══════════════════════════════════════════════════════════════════
        //  VALIDAÇÃO DE EMAIL
        // ══════════════════════════════════════════════════════════════════
        private string? ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "O e-mail é obrigatório.";

            var emailTrimmed = email.Trim();

            if (emailTrimmed.Length > 100)
                return "O e-mail não pode ter mais de 100 caracteres.";

            // Regex de validação de email (RFC 5322 simplificado)
            if (!Regex.IsMatch(emailTrimmed, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                return "O formato do e-mail é inválido.";

            return null; // Email válido ✅
        }

        // ══════════════════════════════════════════════════════════════════
        //  CADASTRO DE NOVO USUÁRIO (com todas as validações)
        // ══════════════════════════════════════════════════════════════════
        public async Task<(bool Sucesso, string? MensagemErro)> CadastrarAsync(string nomeUsuario, string email, string senha)
        {
            // ── Validar Nome de Usuário ──
            var nomeSanitizado = nomeUsuario?.Trim() ?? string.Empty;
            var erroNome = ValidarNomeUsuario(nomeSanitizado);
            if (erroNome != null)
                return (false, erroNome);

            // ── Validar Email ──
            var emailSanitizado = email?.Trim().ToLower() ?? string.Empty;
            var erroEmail = ValidarEmail(emailSanitizado);
            if (erroEmail != null)
                return (false, erroEmail);

            // ── Validar Força da Senha ──
            var erroSenha = ValidarForcaSenha(senha);
            if (erroSenha != null)
                return (false, erroSenha);

            // ── Verificar duplicatas ──
            if (await NomeUsuarioExisteAsync(nomeSanitizado))
                return (false, "Este nome de usuário já está em uso. Escolha outro.");

            if (await EmailExisteAsync(emailSanitizado))
                return (false, "Este e-mail já está cadastrado. Use outro e-mail ou faça login.");

            try
            {
                // Hash seguro com BCrypt (Salt gerado automaticamente)
                string hashSenha = BCrypt.Net.BCrypt.HashPassword(senha);

                var novoUsuario = new Usuario
                {
                    NomeUsuario = nomeSanitizado,
                    Email = emailSanitizado,
                    Senha = hashSenha,
                    DataCadastro = DateTime.UtcNow
                };

                await _context.Usuarios.AddAsync(novoUsuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Novo usuário cadastrado: '{NomeUsuario}' ({Email}).", nomeSanitizado, emailSanitizado);
                return (true, null);
            }
            catch (Exception ex)
            {
                // SEGURANÇA: Não expor ex.Message ao usuário — logar apenas no servidor
                _logger.LogError(ex, "Erro ao cadastrar usuário '{NomeUsuario}'.", nomeSanitizado);
                return (false, "Ocorreu um erro inesperado ao criar sua conta. Tente novamente.");
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  RECUPERAÇÃO DE SENHA (OWASP ANTI-ENUMERAÇÃO + TOKENS CRIPTOGRÁFICOS)
        // ══════════════════════════════════════════════════════════════════
        /// <summary>
        /// Hash SHA-256 (hex minúsculo) do token de recuperação. Se o banco vazar,
        /// os tokens armazenados não podem ser usados para redefinir senhas.
        /// </summary>
        public static string HashToken(string token) =>
            Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token))).ToLower();

        public async Task<(bool Sucesso, string? Token, string? NomeUsuario)> GerarTokenRecuperacaoAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, null, null);

            var emailSanitizado = email.Trim().ToLower();
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email.ToLower() == emailSanitizado);

            if (usuario == null)
            {
                _logger.LogWarning("Tentativa de recuperação de senha para e-mail não cadastrado: {Email}", emailSanitizado);
                return (false, null, null);
            }

            // Geração de token criptograficamente seguro (32 bytes / 256 bits de entropia)
            byte[] bytesAleatorios = new byte[32];
            RandomNumberGenerator.Fill(bytesAleatorios);
            string token = Convert.ToHexString(bytesAleatorios).ToLower();

            // O banco guarda apenas o hash SHA-256 do token; o token puro só existe no link enviado por e-mail.
            usuario.TokenRedefinicaoSenha = HashToken(token);
            usuario.TokenRedefinicaoExpiracao = DateTime.UtcNow.AddMinutes(30);

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Token de recuperação gerado para usuário '{NomeUsuario}' ({Email}).", usuario.NomeUsuario, emailSanitizado);
            return (true, token, usuario.NomeUsuario);
        }

        public async Task<Usuario?> ObterPorTokenValidoAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenHash = HashToken(token.Trim().ToLower());
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.TokenRedefinicaoSenha == tokenHash);

            if (usuario == null)
                return null;

            if (usuario.TokenRedefinicaoExpiracao == null || usuario.TokenRedefinicaoExpiracao < DateTime.UtcNow)
            {
                _logger.LogWarning("Tentativa de uso de token expirado para usuário '{NomeUsuario}'.", usuario.NomeUsuario);
                return null;
            }

            return usuario;
        }

        public async Task<(bool Sucesso, string? MensagemErro)> RedefinirSenhaAsync(string token, string novaSenha)
        {
            if (string.IsNullOrWhiteSpace(token))
                return (false, "Link de redefinição inválido.");

            var erroSenha = ValidarForcaSenha(novaSenha);
            if (erroSenha != null)
                return (false, erroSenha);

            var usuario = await ObterPorTokenValidoAsync(token);
            if (usuario == null)
            {
                return (false, "O link de redefinição é inválido ou já expirou. Por favor, solicite um novo link.");
            }

            try
            {
                // Hashing seguro com BCrypt
                usuario.Senha = BCrypt.Net.BCrypt.HashPassword(novaSenha);

                // Anti-Replay: Invalida imediatamente o token
                usuario.TokenRedefinicaoSenha = null;
                usuario.TokenRedefinicaoExpiracao = null;

                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Senha redefinida com sucesso para o usuário '{NomeUsuario}'.", usuario.NomeUsuario);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao redefinir senha para usuário '{NomeUsuario}'.", usuario.NomeUsuario);
                return (false, "Ocorreu um erro ao redefinir sua senha. Tente novamente.");
            }
        }
    }
}