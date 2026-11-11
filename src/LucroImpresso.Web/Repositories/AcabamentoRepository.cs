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
            // Segurança Multi-Tenant (Anti-IDOR): só atualiza se o registro pertencer ao usuário logado
            var pertenceAoUsuario = await _context.Acabamentos.AsNoTracking().AnyAsync(a => a.Id == acabamento.Id && a.UsuarioId == acabamento.UsuarioId);
            if (!pertenceAoUsuario) return;
            _context.Acabamentos.Update(acabamento);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, int usuarioId)
        {
            _context.ChangeTracker.Clear();

            var acabamento = await _context.Acabamentos
                .FirstOrDefaultAsync(a => a.Id == id && a.UsuarioId == usuarioId);

            if (acabamento != null && await _context.OrcamentoAcabamentos.AnyAsync(oa => oa.AcabamentoId == id && oa.UsuarioId == usuarioId))
                throw new InvalidOperationException("Este insumo está vinculado a orçamentos e não pode ser excluído. Remova-o dos orçamentos primeiro.");

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
