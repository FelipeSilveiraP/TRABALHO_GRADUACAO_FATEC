using LucroImpresso.Web.Models;

namespace LucroImpresso.Web.Interfaces
{
    public interface IHistoricoFinanceiroRepository
    {
        Task<HistoricoFinanceiro> GetResumoAsync(int usuarioId);
        Task AdicionarReceita(decimal valor, int usuarioId);
        Task AdicionarDespesaMaterial(decimal valor, int usuarioId);
        Task AdicionarDespesaEnergia(decimal valor, int usuarioId);
        Task AdicionarDespesaAcabamento(decimal valor, int usuarioId);
    }
}
