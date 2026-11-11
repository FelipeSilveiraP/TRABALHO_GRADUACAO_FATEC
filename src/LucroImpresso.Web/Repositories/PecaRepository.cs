using LucroImpresso.Web.Data;
using LucroImpresso.Web.Interfaces;
using LucroImpresso.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LucroImpresso.Web.Repositories
{
    public class PecaRepository : IPecaRepository
    {
        private readonly AppDbContext _context;

        public PecaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Peca>> GetAllAsync(int usuarioId)
        {
            return await _context.Pecas
                .Where(p => p.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Peca peca)
        {
            if (peca.UsuarioId <= 0)
                throw new InvalidOperationException("Não é possível cadastrar peça sem um Usuário válido (Multi-Tenant).");

            _context.ChangeTracker.Clear();
            await _context.Pecas.AddAsync(peca);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Peca peca)
        {
            _context.ChangeTracker.Clear();
            // Segurança Multi-Tenant (Anti-IDOR): só atualiza se o registro pertencer ao usuário logado
            var pertenceAoUsuario = await _context.Pecas.AsNoTracking().AnyAsync(p => p.Id == peca.Id && p.UsuarioId == peca.UsuarioId);
            if (!pertenceAoUsuario) return;
            _context.Pecas.Update(peca);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, int usuarioId)
        {
            _context.ChangeTracker.Clear();
            var peca = await _context.Pecas
                .FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);

            if (peca != null && await _context.Orcamentos.AnyAsync(o => o.PecaId == id && o.UsuarioId == usuarioId))
                throw new InvalidOperationException("Esta peça está vinculada a orçamentos e não pode ser excluída. Exclua os orçamentos que a utilizam primeiro.");

            if (peca != null)
            {
                _context.Pecas.Remove(peca);
                await _context.SaveChangesAsync();
            }
        }
    }
}
