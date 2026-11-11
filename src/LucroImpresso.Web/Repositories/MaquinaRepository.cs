using LucroImpresso.Web.Data;
using LucroImpresso.Web.Interfaces;
using LucroImpresso.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LucroImpresso.Web.Repositories
{
    public class MaquinaRepository : IMaquinaRepository
    {
        private readonly AppDbContext _context;

        public MaquinaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Maquina>> GetAllAsync(int usuarioId)
        {
            return await _context.Maquinas
                .Where(m => m.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Maquina maquina)
        {
            if (maquina.UsuarioId <= 0)
                throw new InvalidOperationException("Não é possível cadastrar máquina sem um Usuário válido (Multi-Tenant).");

            _context.ChangeTracker.Clear();
            await _context.Maquinas.AddAsync(maquina);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Maquina maquina)
        {
            _context.ChangeTracker.Clear();
            // Segurança Multi-Tenant (Anti-IDOR): só atualiza se o registro pertencer ao usuário logado
            var pertenceAoUsuario = await _context.Maquinas.AsNoTracking().AnyAsync(m => m.Id == maquina.Id && m.UsuarioId == maquina.UsuarioId);
            if (!pertenceAoUsuario) return;
            _context.Maquinas.Update(maquina);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, int usuarioId)
        {
            _context.ChangeTracker.Clear();
            var maquina = await _context.Maquinas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (maquina != null && await _context.Orcamentos.AnyAsync(o => o.MaquinaId == id && o.UsuarioId == usuarioId))
                throw new InvalidOperationException("Esta máquina está vinculada a orçamentos e não pode ser excluída. Exclua os orçamentos que a utilizam primeiro.");

            if (maquina != null)
            {
                _context.Maquinas.Remove(maquina);
                await _context.SaveChangesAsync();
            }
        }
    }
}
