using LucroImpresso.Web.Data;
using LucroImpresso.Web.Interfaces;
using LucroImpresso.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LucroImpresso.Web.Repositories
{
    public class AcabamentoRepository : IAcabamentoRepository
    {
        private readonly AppDbContext _context;

        public AcabamentoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Acabamento>> GetAllAsync(int usuarioId)
        {
            return await _context.Acabamentos
                .Where(a => a.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Acabamento acabamento)
        {
            if (acabamento.UsuarioId <= 0)
                throw new InvalidOperationException("Não é possível cadastrar acabamento sem um Usuário válido (Multi-Tenant).");

            _context.ChangeTracker.Clear();
            await _context.Acabamentos.AddAsync(acabamento);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Acabamento acabamento)
        {
            _context.ChangeTracker.Clear();
            var existente = await _context.Acabamentos.AsNoTracking().FirstOrDefaultAsync(a => a.Id == acabamento.Id);
            if (existente == null) return;

            // Segurança Multi-Tenant (Anti-IDOR): Preserva o tenant proprietário do registro
            acabamento.UsuarioId = existente.UsuarioId;
            _context.Acabamentos.Update(acabamento);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, int usuarioId)
        {
            _context.ChangeTracker.Clear();

            var acabamento = await _context.Acabamentos
                .FirstOrDefaultAsync(a => a.Id == id && a.UsuarioId == usuarioId);

            if (acabamento != null)
            {
                var resumo = await _context.HistoricoFinanceiro.FirstOrDefaultAsync(r => r.UsuarioId == usuarioId);
                if (resumo == null)
                {
                    resumo = new HistoricoFinanceiro { UsuarioId = usuarioId };
                    _context.HistoricoFinanceiro.Add(resumo);
                }
                resumo.DespesasAcabamentoAcumuladas += acabamento.ValorPago;

                _context.Acabamentos.Remove(acabamento);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetTotalDespesasAcabamentosAsync(int usuarioId)
        {
            var somaAtivos = await _context.Acabamentos
                .Where(a => a.UsuarioId == usuarioId)
                .SumAsync(a => a.ValorPago);

            var resumo = await _context.HistoricoFinanceiro.FirstOrDefaultAsync(r => r.UsuarioId == usuarioId);
            return somaAtivos + (resumo?.DespesasAcabamentoAcumuladas ?? 0);
        }
    }
}
