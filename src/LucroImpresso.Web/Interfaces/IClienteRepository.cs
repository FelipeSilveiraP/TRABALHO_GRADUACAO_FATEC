using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Interfaces
{
    public interface IClienteRepository
    {
        Task<List<Cliente>> GetAllAsync(int usuarioId);
        Task<Cliente?> GetByIdAsync(int id, int usuarioId);
        Task AddAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(int id, int usuarioId);
    }
}
