using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucroImpresso.Web.Models
{
    [Table("Peca")]
    public class Peca
    {
        [Column("id_peca")]
        public int Id { get; set; }

        [Required]
        [Column("fk_usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required(ErrorMessage = "O nome da peça é obrigatório.")]
        [MaxLength(200)]
        [Column("nome_peca")]
        public string NomePeca { get; set; } = string.Empty;

        [MaxLength(2000)]
        [Url(ErrorMessage = "O link do arquivo STL deve ser uma URL válida (ex: https://...).")]
        [Column("link_arquivo_stl")]
        public string LinkArquivoStl { get; set; } = string.Empty;

        public ICollection<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();

        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
