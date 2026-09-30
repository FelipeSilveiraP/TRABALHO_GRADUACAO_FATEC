namespace LucroImpresso.Web.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Envia e-mail de recuperação de senha contendo o link com token seguro.
        /// </summary>
        Task EnviarEmailRecuperacaoSenhaAsync(string emailDestino, string nomeUsuario, string linkRedefinicao);
    }
}
