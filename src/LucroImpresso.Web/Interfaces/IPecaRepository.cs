using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Interfaces
{
    public interface IPecaRepository
    {
        Task<List<Peca>> GetAllAsync(int usuarioId);
        Task AddAsync(Peca peca);
        Task UpdateAsync(Peca peca);
        Task DeleteAsync(int id, int usuarioId);
    }
}
