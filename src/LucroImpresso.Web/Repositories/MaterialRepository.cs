using LucroImpresso.Web.Data;
using LucroImpresso.Web.Interfaces;
using LucroImpresso.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LucroImpresso.Web.Repositories
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly AppDbContext _context;

        public MaterialRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Material>> GetAllAsync(int usuarioId)
        {
            return await _context.Materiais
                .Where(m => m.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Material material)
        {
            if (material.UsuarioId <= 0)
                throw new InvalidOperationException("Não é possível cadastrar material sem um Usuário válido (Multi-Tenant).");

            _context.ChangeTracker.Clear();
            await _context.Materiais.AddAsync(material);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Material material)
        {
            _context.ChangeTracker.Clear();
            // Segurança Multi-Tenant (Anti-IDOR): só atualiza se o registro pertencer ao usuário logado
            var pertenceAoUsuario = await _context.Materiais.AsNoTracking().AnyAsync(m => m.Id == material.Id && m.UsuarioId == material.UsuarioId);
            if (!pertenceAoUsuario) return;
            _context.Materiais.Update(material);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, int usuarioId)
        {
            _context.ChangeTracker.Clear();

            var material = await _context.Materiais
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (material != null && await _context.Orcamentos.AnyAsync(o => o.MaterialId == id && o.UsuarioId == usuarioId))
                throw new InvalidOperationException("Este material está vinculado a orçamentos e não pode ser excluído. Exclua os orçamentos que o utilizam primeiro.");

            if (material != null)
            {
                // COFRE ACUMULADOR: Salva a despesa do material antes de apagá-lo
                var resumo = await _context.HistoricoFinanceiro.FirstOrDefaultAsync(r => r.UsuarioId == usuarioId);
                if (resumo == null)
                {
                    resumo = new HistoricoFinanceiro { UsuarioId = usuarioId };
                    _context.HistoricoFinanceiro.Add(resumo);
                }
                resumo.DespesasMaterialAcumuladas += material.ValorPago;

                _context.Materiais.Remove(material);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetTotalDespesasMateriaisAsync(int usuarioId)
        {
            var somaAtivos = await _context.Materiais
                .Where(m => m.UsuarioId == usuarioId)
                .SumAsync(m => m.ValorPago);

            var resumo = await _context.HistoricoFinanceiro.FirstOrDefaultAsync(r => r.UsuarioId == usuarioId);
            return somaAtivos + (resumo?.DespesasMaterialAcumuladas ?? 0);
        }
    }
}