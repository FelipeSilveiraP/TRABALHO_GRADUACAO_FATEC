using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucroImpresso.Web.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Column("id_usuario")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O usuário é obrigatório.")]
        [MaxLength(50)]
        [Column("nome_usuario")]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MaxLength(100)]
        [Column("senha")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [MaxLength(100)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        [Column("token_redefinicao_senha")]
        public string? TokenRedefinicaoSenha { get; set; }

        [Column("token_redefinicao_expiracao")]
        public DateTime? TokenRedefinicaoExpiracao { get; set; }
    }
}
