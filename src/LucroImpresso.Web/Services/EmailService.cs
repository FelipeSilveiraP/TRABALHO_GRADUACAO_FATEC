using LucroImpresso.Web.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LucroImpresso.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IWebHostEnvironment _env;

        // Permite inspecionar o último link gerado em ambiente de desenvolvimento/testes
        public static string? UltimoLinkGeradoDev { get; private set; }
        public static void LimparLinkDev() => UltimoLinkGeradoDev = null;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _logger = logger;
            _env = env;
        }

        public async Task EnviarEmailRecuperacaoSenhaAsync(string emailDestino, string nomeUsuario, string linkRedefinicao)
        {
            // SEGURANÇA V2: Só armazena o link de dev em ambiente de desenvolvimento.
            // Em produção, este campo NUNCA é preenchido (permanece null), impedindo exposição acidental do token.
            if (_env.IsDevelopment())
            {
                UltimoLinkGeradoDev = linkRedefinicao;
            }
            else
            {
                UltimoLinkGeradoDev = null;
            }

            var host = _configuration["SmtpSettings:Host"];
            var portStr = _configuration["SmtpSettings:Port"];
            var username = _configuration["SmtpSettings:Username"];
            var password = _configuration["SmtpSettings:Password"];
            var senderName = _configuration["SmtpSettings:SenderName"] ?? "Lucro Impresso";
            var senderEmail = _configuration["SmtpSettings:SenderEmail"] ?? "nao-responda@lucroimpresso.com";

            // Se o SMTP não estiver configurado (ambiente local/desenvolvimento de TG sem servidor de e-mail ativo),
            // registramos nos logs e permitimos testes sem falhar.
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || host == "smtp.exemplo.com")
            {
                _logger.LogInformation(
                    "========================================================================\n" +
                    "[SMTP EM MODO DESENVOLVIMENTO / SIMULAÇÃO]\n" +
                    "Destinatário: {EmailDestino} (Usuário: {NomeUsuario})\n" +
                    "Link de Redefinição: {LinkRedefinicao}\n" +
                    "Válido por: 30 minutos\n" +
                    "Para envio real via e-mail, configure as chaves 'SmtpSettings' no appsettings.json.\n" +
                    "========================================================================",
                    emailDestino, nomeUsuario, linkRedefinicao);

                return;
            }

            try
            {
                int port = int.TryParse(portStr, out var p) ? p : 587;

                var mensagem = new MimeMessage();
                mensagem.From.Add(new MailboxAddress(senderName, senderEmail));
                mensagem.To.Add(new MailboxAddress(nomeUsuario, emailDestino));
                mensagem.Subject = "Recuperação de Senha - Lucro Impresso";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                    <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 3px solid #000; padding: 24px; background-color: #fff;"">
                        <div style=""border-bottom: 3px solid #000; padding-bottom: 16px; margin-bottom: 20px; text-align: center;"">
                            <h1 style=""margin: 0; font-size: 24px; font-weight: 900; text-transform: uppercase;"">LUCRO IMPRESSO</h1>
                            <p style=""margin: 4px 0 0; font-size: 12px; color: #555; text-transform: uppercase; font-weight: bold;"">Recuperação de Credenciais de Acesso</p>
                        </div>
                        <p style=""font-size: 15px; color: #111;"">Olá, <strong>{nomeUsuario}</strong>,</p>
                        <p style=""font-size: 14px; color: #333; line-height: 1.5;"">
                            Recebemos uma solicitação para redefinir a senha da sua conta no <strong>Lucro Impresso</strong>.
                        </p>
                        <p style=""font-size: 14px; color: #333; line-height: 1.5;"">
                            Clique no botão abaixo para criar uma nova senha. Este link é seguro e expira em <strong>30 minutos</strong>:
                        </p>
                        <div style=""text-align: center; margin: 30px 0;"">
                            <a href=""{linkRedefinicao}"" style=""display: inline-block; padding: 14px 28px; background-color: #ffd600; color: #000; font-weight: 900; text-decoration: none; border: 2px solid #000; box-shadow: 4px 4px 0px #000; text-transform: uppercase; font-size: 15px;"">
                                REDEFINIR MINHA SENHA
                            </a>
                        </div>
                        <p style=""font-size: 12px; color: #666; line-height: 1.4;"">
                            Se o botão acima não funcionar, copie e cole o seguinte link no seu navegador:<br/>
                            <a href=""{linkRedefinicao}"" style=""color: #000; word-break: break-all;"">{linkRedefinicao}</a>
                        </p>
                        <hr style=""border: 0; border-top: 1px solid #ddd; margin: 24px 0;"" />
                        <p style=""font-size: 11px; color: #777; margin: 0;"">
                            Se você não solicitou a redefinição de senha, ignore este e-mail com segurança. Sua senha atual permanecerá inalterada.
                        </p>
                    </div>"
                };

                mensagem.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                // Utiliza STARTTLS para criptografia em trânsito
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    await client.AuthenticateAsync(username, password);
                }
                await client.SendAsync(mensagem);
                await client.DisconnectAsync(true);

                _logger.LogInformation("E-mail de recuperação de senha enviado com sucesso para '{EmailDestino}'.", emailDestino);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao enviar e-mail de recuperação de senha para '{EmailDestino}'.", emailDestino);
                // Não propagamos a exceção ao usuário para não vazar detalhes de infraestrutura
            }
        }
    }
}
