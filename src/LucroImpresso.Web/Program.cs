using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using LucroImpresso.Web.Components;
using LucroImpresso.Web.Data;
using LucroImpresso.Web.Interfaces;
using LucroImpresso.Web.Repositories;
using LucroImpresso.Web.Services;
using LucroImpresso.Web.Validators;
using FluentValidation;
using NetEscapades.AspNetCore.SecurityHeaders;
using Serilog;
using Ganss.Xss;

// ============================================================
// SERILOG — Logging Estruturado (Bootstrap antes do Host)
// ============================================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        Path.Combine(AppContext.BaseDirectory, "Logs", "lucroimpresso-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{

var builder = WebApplication.CreateBuilder(args);

// Substitui o logging padrão pelo Serilog
builder.Host.UseSerilog();

// 0. AppDbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MariaDbServerVersion(new Version(10, 11, 0)); // Versão do MariaDB utilizado no servidor Debian

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));
// 1. Injeção de Dependência
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IOrcamentoRepository, OrcamentoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IMaquinaRepository, MaquinaRepository>();
builder.Services.AddScoped<IHistoricoFinanceiroRepository, HistoricoFinanceiroRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IPecaRepository, PecaRepository>();
builder.Services.AddScoped<IAcabamentoRepository, AcabamentoRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<LucroImpresso.Web.Services.UsuarioContextService>();
builder.Services.AddScoped<OrcamentoStateMachine>();
builder.Services.AddHostedService<DatabaseCleanupService>();

// 1.1. Validação de Domínio Centralizada (FluentValidation)
builder.Services.AddValidatorsFromAssemblyContaining<MaterialValidator>();

// 1.2-B. HtmlSanitizer: Sanitização XSS em inputs de texto livre (OWASP)
builder.Services.AddSingleton<HtmlSanitizer>(sp =>
{
    var sanitizer = new HtmlSanitizer();
    sanitizer.AllowedTags.Clear(); // Remove TODAS as tags HTML — só texto puro
    return sanitizer;
});

// 1.2. DataProtection: Persiste chaves de criptografia em disco
//      Sem isso, a cada restart do app todos os cookies são invalidados.
var keysDir = Path.Combine(AppContext.BaseDirectory, "DataProtection-Keys");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysDir))
    .SetApplicationName("LucroImpresso");

// 2. Autenticação via Cookie com Endurecimento (Cookie Hardening)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(4);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = "LucroImpresso.Auth";

        // CORREÇÃO CRÍTICA: Impede que o cookie auth tente redirecionar (302)
        // requests do hub SignalR (/_blazor). WebSocket não suporta 302 e
        // isso causava travamento do circuit e loop de reconexão.
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/_blazor"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

// 3. Proteção contra Força Bruta (Rate Limiting OWASP)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("AuthLimiter", opt =>
    {
        opt.PermitLimit = 10; // Máximo 10 requisições por minuto por IP
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Hardening do Cookie Antiforgery (CSRF Defense-in-Depth)
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.Name = "LucroImpresso.Antiforgery";
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}

// ============================================================
// CABEÇALHOS DE SEGURANÇA HTTP (OWASP DEFENSE-IN-DEPTH VIA NETESCAPADES)
// ============================================================
var securityHeadersPolicy = new HeaderPolicyCollection()
    .AddDefaultSecurityHeaders()
    .AddContentSecurityPolicy(cspBuilder =>
    {
        cspBuilder.AddDefaultSrc().Self();
        cspBuilder.AddScriptSrc().Self().UnsafeInline();
        cspBuilder.AddStyleSrc().Self().UnsafeInline()
            .From("https://fonts.googleapis.com");
        cspBuilder.AddFontSrc().Self()
            .From("https://fonts.gstatic.com");
        cspBuilder.AddImgSrc().Self().From("data:");
        cspBuilder.AddConnectSrc().Self();
        cspBuilder.AddFrameAncestors().None();
        cspBuilder.AddBaseUri().Self();
        cspBuilder.AddFormAction().Self();
    })
    .AddPermissionsPolicy(builder =>
    {
        builder.AddCamera().None();
        builder.AddMicrophone().None();
        builder.AddGeolocation().None();
    })
    .AddCustomHeader("Referrer-Policy", "strict-origin-when-cross-origin");

app.UseSecurityHeaders(securityHeadersPolicy);

// Serilog: Request Logging estruturado (registra cada request HTTP)
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "[HTTP] {RequestMethod} {RequestPath} respondeu {StatusCode} em {Elapsed:0.0000} ms";
});

// Middleware de Rate Limiting
app.UseRateLimiter();

// ============================================================
// CORREÇÃO CRÍTICA DE SEGURANÇA:
// UseAuthentication e UseAuthorization DEVEM vir antes do
// MapRazorComponents. Sem eles, o [Authorize] é ignorado
// e qualquer pessoa acessa as páginas sem estar logada.
// ============================================================
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// 4. Minimal API: Processa o Login (com Rate Limiting + Antiforgery CSRF + Sanitização XSS)
app.MapPost("/auth/login", async (HttpContext ctx, [FromForm] string usuario, [FromForm] string senha, IUsuarioRepository repo, HtmlSanitizer sanitizer) =>
{
    // Sanitização XSS: remove qualquer tag HTML maliciosa do input
    var usuarioLimpo = sanitizer.Sanitize(usuario?.Trim() ?? string.Empty);

    if (string.IsNullOrWhiteSpace(usuarioLimpo) || string.IsNullOrWhiteSpace(senha))
    {
        return Results.Redirect("/login?error=campos_vazios");
    }

    var u = await repo.AutenticarAsync(usuarioLimpo, senha);
    if (u != null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, u.NomeUsuario),
            new Claim(ClaimTypes.NameIdentifier, u.Id.ToString()),
            new Claim(ClaimTypes.Email, u.Email)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        Log.Information("[AUDITORIA] Login bem-sucedido: {Usuario}", u.NomeUsuario);
        return Results.Redirect("/dashboard");
    }
    Log.Warning("[AUDITORIA] Tentativa de login falhou para usuário: {Usuario}", usuarioLimpo);
    return Results.Redirect("/login?error=credenciais_invalidas");
}).RequireRateLimiting("AuthLimiter");

// 5. Minimal API: Processa o Cadastro de Novo Usuário (com Rate Limiting, BCrypt, Email e Sanitização XSS)
app.MapPost("/auth/cadastro", async (HttpContext ctx, [FromForm] string usuario, [FromForm] string email, [FromForm] string senha, [FromForm] string confirmacaoSenha, IUsuarioRepository repo, HtmlSanitizer sanitizer) =>
{
    // Sanitização XSS: remove qualquer tag HTML maliciosa dos inputs
    var usuarioLimpo = sanitizer.Sanitize(usuario?.Trim() ?? string.Empty);
    var emailLimpo = sanitizer.Sanitize(email?.Trim() ?? string.Empty);

    if (string.IsNullOrWhiteSpace(usuarioLimpo) || string.IsNullOrWhiteSpace(emailLimpo) || string.IsNullOrWhiteSpace(senha))
    {
        return Results.Redirect("/cadastro?error=" + System.Net.WebUtility.UrlEncode("Preencha todos os campos obrigatórios."));
    }

    // Validação de formato de email (OWASP)
    if (!System.Text.RegularExpressions.Regex.IsMatch(emailLimpo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
    {
        return Results.Redirect("/cadastro?error=" + System.Net.WebUtility.UrlEncode("Formato de e-mail inválido."));
    }

    if (senha != confirmacaoSenha)
    {
        return Results.Redirect("/cadastro?error=" + System.Net.WebUtility.UrlEncode("A confirmação de senha não confere com a senha digitada."));
    }

    var (sucesso, mensagemErro) = await repo.CadastrarAsync(usuarioLimpo, emailLimpo, senha);
    if (!sucesso)
    {
        return Results.Redirect("/cadastro?error=" + System.Net.WebUtility.UrlEncode(mensagemErro ?? "Erro ao realizar cadastro."));
    }

    Log.Information("[AUDITORIA] Novo usuário cadastrado: {Usuario}", usuarioLimpo);
    return Results.Redirect("/login?cadastrado=true");
}).RequireRateLimiting("AuthLimiter");

// 6. Minimal API: Processa Solicitação de Recuperação de Senha (OWASP Anti-Enumeração + Rate Limiting + Sanitização)
app.MapPost("/auth/esqueci-senha", async (HttpContext ctx, [FromForm] string email, IUsuarioRepository repo, IEmailService emailService, HtmlSanitizer sanitizer) =>
{
    var emailLimpo = sanitizer.Sanitize(email?.Trim() ?? string.Empty);

    if (!string.IsNullOrWhiteSpace(emailLimpo))
    {
        var (sucesso, token, nomeUsuario) = await repo.GerarTokenRecuperacaoAsync(emailLimpo);
        if (sucesso && !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(nomeUsuario))
        {
            // Anti Host-Header Injection: usa a URL pública configurada (AppSettings:BaseUrl) quando existir;
            // sem ela (uso local), cai para o endereço da própria requisição.
            var baseUrl = app.Configuration["AppSettings:BaseUrl"]?.TrimEnd('/')
                ?? $"{ctx.Request.Scheme}://{ctx.Request.Host.Value}";
            var linkRedefinicao = $"{baseUrl}/redefinir-senha?token={token}";
            await emailService.EnviarEmailRecuperacaoSenhaAsync(emailLimpo, nomeUsuario, linkRedefinicao);
        }
    }

    // OWASP: Responde sempre com sucesso genérico para prevenir enumeração de contas
    return Results.Redirect("/esqueci-senha?enviado=true");
}).RequireRateLimiting("AuthLimiter");

// 7. Minimal API: Processa a Redefinição Efetiva de Senha (com Validação OWASP + Invalidação de Token)
app.MapPost("/auth/redefinir-senha", async (HttpContext ctx, [FromForm] string token, [FromForm] string novaSenha, [FromForm] string confirmacaoSenha, IUsuarioRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(token))
    {
        return Results.Redirect("/esqueci-senha?error=" + System.Net.WebUtility.UrlEncode("Link inválido. Por favor, solicite uma nova recuperação."));
    }

    if (string.IsNullOrWhiteSpace(novaSenha) || string.IsNullOrWhiteSpace(confirmacaoSenha))
    {
        return Results.Redirect($"/redefinir-senha?token={Uri.EscapeDataString(token)}&error=" + System.Net.WebUtility.UrlEncode("Preencha todos os campos obrigatórios."));
    }

    if (novaSenha != confirmacaoSenha)
    {
        return Results.Redirect($"/redefinir-senha?token={Uri.EscapeDataString(token)}&error=" + System.Net.WebUtility.UrlEncode("A confirmação não confere com a nova senha digitada."));
    }

    var (sucesso, erro) = await repo.RedefinirSenhaAsync(token, novaSenha);
    if (!sucesso)
    {
        return Results.Redirect($"/redefinir-senha?token={Uri.EscapeDataString(token)}&error=" + System.Net.WebUtility.UrlEncode(erro ?? "Erro ao redefinir a senha."));
    }

    return Results.Redirect("/login?senhaRedefinida=true");
}).RequireRateLimiting("AuthLimiter");

// 6. Minimal API: Processa o Logout (POST para prevenir Logout CSRF)
app.MapPost("/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

// SEGURANÇA V1: Rota GET de logout REMOVIDA.
// Logout deve ser feito EXCLUSIVAMENTE via POST para prevenir CSRF.
// Um atacante poderia forçar logout de qualquer usuário via <img src="/auth/logout">.

// 7. Seeding & Schema Migration Safe Check: Garante que as colunas essenciais e o admin existam
using (var scope = app.Services.CreateScope())
{
    try 
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // ── Migrations do EF Core: aplica as pendentes (ex.: Tinta -> Acabamento, peso decimal) ──
        // Substitui o antigo SQL manual executado a cada inicialização. As migrations são
        // idempotentes nesse ponto, então funcionam também em bancos já ajustados à mão.
        try
        {
            var pendentes = (await db.Database.GetPendingMigrationsAsync()).ToList();
            if (pendentes.Count > 0)
            {
                Log.Information("Aplicando {Quantidade} migration(s) pendente(s): {Migrations}", pendentes.Count, string.Join(", ", pendentes));
                await db.Database.MigrateAsync();
            }
        }
        catch (Exception exMig)
        {
            Log.Warning(exMig, "Aviso ao aplicar migrations do banco de dados. Rode 'dotnet ef database update' manualmente.");
        }

        var userRepo = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
        await userRepo.CriarUsuarioPadraoAsync();
    }
    catch (Exception ex)
    {
        // Se o banco estiver fora ou sem as tabelas, loga o erro mas não derruba a aplicação.
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Falha ao executar o Seeding do banco de dados.");
    }
}

app.Run(); // Esta deve ser a ÚLTIMA linha do seu Program.cs

}
catch (Exception ex) when (ex is not HostAbortedException)
{
    // HostAbortedException é lançada de propósito pelas ferramentas do EF (dotnet ef migrations)
    // ao inspecionar o host — não é um erro da aplicação.
    Log.Fatal(ex, "Aplicação encerrada de forma inesperada.");
}
finally
{
    Log.CloseAndFlush();
}
