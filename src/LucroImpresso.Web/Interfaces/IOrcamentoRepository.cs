using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Interfaces
{
    public interface IOrcamentoRepository
    {
        Task<List<Orcamento>> GetAllAsync(int usuarioId);
        Task AddAsync(Orcamento orcamento);
        Task UpdateAsync(Orcamento orcamento);
        Task DeleteAsync(int id, int usuarioId);
        Task<(bool Sucesso, string? Erro)> IniciarImpressaoAsync(int id, int usuarioId);
        Task<(bool Sucesso, string? Erro)> FinalizarProducaoAsync(int id, int usuarioId, bool sucesso);
        Task<decimal> GetTotalFaturamentoAsync(int usuarioId);
        Task<int> GetTotalPecasAsync(int usuarioId);
        Task<List<Orcamento>> GetUltimosOrcamentosAsync(int count, int usuarioId);
        Task DeleteAntigosAsync(int dias);
        Task<decimal> GetTotalDespesasEnergiaAsync(int usuarioId);
    }
}
