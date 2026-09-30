using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Interfaces
{
    public interface IMaterialRepository
    {
        Task<List<Material>> GetAllAsync(int usuarioId);
        Task AddAsync(Material material);
        Task UpdateAsync(Material material);
        Task DeleteAsync(int id, int usuarioId);
        Task<decimal> GetTotalDespesasMateriaisAsync(int usuarioId);
    }
}