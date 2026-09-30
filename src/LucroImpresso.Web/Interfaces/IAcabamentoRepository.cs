using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Interfaces
{
    public interface IAcabamentoRepository
    {
        Task<List<Acabamento>> GetAllAsync(int usuarioId);
        Task AddAsync(Acabamento acabamento);
        Task UpdateAsync(Acabamento acabamento);
        Task DeleteAsync(int id, int usuarioId);
        Task<decimal> GetTotalDespesasAcabamentosAsync(int usuarioId);
    }
}
