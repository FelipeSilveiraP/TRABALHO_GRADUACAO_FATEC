using LucroImpresso.Web.Data;
using LucroImpresso.Web.Interfaces;
using LucroImpresso.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LucroImpresso.Web.Repositories
{
    public class HistoricoFinanceiroRepository : IHistoricoFinanceiroRepository
    {
        private readonly AppDbContext _context;

        public HistoricoFinanceiroRepository(AppDbContext context)
        {
            _context = context;
        }

        private async Task<HistoricoFinanceiro> EnsureExistsAsync(int usuarioId)
        {
            var resumo = await _context.HistoricoFinanceiro.FirstOrDefaultAsync(r => r.UsuarioId == usuarioId);
            if (resumo == null)
            {
                resumo = new HistoricoFinanceiro 
                { 
                    UsuarioId = usuarioId, 
                    ReceitasAcumuladas = 0, 
                    DespesasMaterialAcumuladas = 0, 
                    DespesasEnergiaAcumuladas = 0,
                    DespesasAcabamentoAcumuladas = 0
                };
                _context.HistoricoFinanceiro.Add(resumo);
                await _context.SaveChangesAsync();
            }
            return resumo;
        }

        public async Task<HistoricoFinanceiro> GetResumoAsync(int usuarioId)
        {
            return await EnsureExistsAsync(usuarioId);
        }

        public async Task AdicionarReceita(decimal valor, int usuarioId)
        {
            var resumo = await EnsureExistsAsync(usuarioId);
            resumo.ReceitasAcumuladas += valor;
            _context.HistoricoFinanceiro.Update(resumo);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarDespesaMaterial(decimal valor, int usuarioId)
        {
            var resumo = await EnsureExistsAsync(usuarioId);
            resumo.DespesasMaterialAcumuladas += valor;
            _context.HistoricoFinanceiro.Update(resumo);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarDespesaEnergia(decimal valor, int usuarioId)
        {
            var resumo = await EnsureExistsAsync(usuarioId);
            resumo.DespesasEnergiaAcumuladas += valor;
            _context.HistoricoFinanceiro.Update(resumo);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarDespesaAcabamento(decimal valor, int usuarioId)
        {
            var resumo = await EnsureExistsAsync(usuarioId);
            resumo.DespesasAcabamentoAcumuladas += valor;
            _context.HistoricoFinanceiro.Update(resumo);
            await _context.SaveChangesAsync();
        }
    }
}
