using LucroImpresso.Web.Data;
using LucroImpresso.Web.Interfaces;
using LucroImpresso.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LucroImpresso.Web.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClienteRepository> _logger;

        public ClienteRepository(AppDbContext context, ILogger<ClienteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Cliente>> GetAllAsync(int usuarioId)
        {
            return await _context.Clientes
                .Where(c => c.UsuarioId == usuarioId)
                .Include(c => c.Endereco)
                .Include(c => c.PessoaFisica)
                .Include(c => c.PessoaJuridica)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Cliente?> GetByIdAsync(int id, int usuarioId)
        {
            return await _context.Clientes
                .Where(c => c.Id == id && c.UsuarioId == usuarioId)
                .Include(c => c.Endereco)
                .Include(c => c.PessoaFisica)
                .Include(c => c.PessoaJuridica)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Cliente cliente)
        {
            if (cliente.UsuarioId <= 0)
                throw new InvalidOperationException("Não é possível cadastrar cliente sem um Usuário válido (Multi-Tenant).");

            _context.ChangeTracker.Clear();
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Auditoria: Cliente {ClienteId} cadastrado para Usuário {UsuarioId} ({Nome}).", cliente.Id, cliente.UsuarioId, cliente.NomeExibicao);
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _context.ChangeTracker.Clear();

            var existing = await _context.Clientes
                .Include(c => c.Endereco)
                .Include(c => c.PessoaFisica)
                .Include(c => c.PessoaJuridica)
                .FirstOrDefaultAsync(c => c.Id == cliente.Id && c.UsuarioId == cliente.UsuarioId);

            if (existing == null) return;

            // ── Atualiza dados comuns de Cliente ─────────────────────────────
            existing.CelularUnico = cliente.CelularUnico;

            // ── Atualiza Endereço ────────────────────────────────────────────
            if (cliente.Endereco != null)
            {
                if (existing.Endereco != null)
                {
                    existing.Endereco.Logradouro = cliente.Endereco.Logradouro;
                    existing.Endereco.Numero     = cliente.Endereco.Numero;
                    existing.Endereco.Bairro     = cliente.Endereco.Bairro;
                    existing.Endereco.Cidade     = cliente.Endereco.Cidade;
                    existing.Endereco.Cep        = cliente.Endereco.Cep;
                }
                else
                {
                    existing.Endereco = new Endereco
                    {
                        UsuarioId  = existing.UsuarioId,
                        ClienteId  = existing.Id,
                        Logradouro = cliente.Endereco.Logradouro,
                        Numero     = cliente.Endereco.Numero,
                        Bairro     = cliente.Endereco.Bairro,
                        Cidade     = cliente.Endereco.Cidade,
                        Cep        = cliente.Endereco.Cep
                    };
                }
            }

            // ── Atualiza PessoaFisica ────────────────────────────────────────
            if (cliente.PessoaFisica != null)
            {
                if (existing.PessoaFisica != null)
                {
                    existing.PessoaFisica.Nome = cliente.PessoaFisica.Nome;
                    existing.PessoaFisica.Cpf = cliente.PessoaFisica.Cpf;
                }
                else
                {
                    existing.PessoaFisica = new PessoaFisica
                    {
                        ClienteId = existing.Id,
                        UsuarioId = existing.UsuarioId,
                        Nome      = cliente.PessoaFisica.Nome,
                        Cpf       = cliente.PessoaFisica.Cpf
                    };
                }

                // Remove PJ se havia (troca de tipo PJ → PF)
                if (existing.PessoaJuridica != null)
                {
                    _context.PessoasJuridicas.Remove(existing.PessoaJuridica);
                }
            }
            // ── Atualiza PessoaJuridica ──────────────────────────────────────
            else if (cliente.PessoaJuridica != null)
            {
                if (existing.PessoaJuridica != null)
                {
                    existing.PessoaJuridica.RazaoSocial = cliente.PessoaJuridica.RazaoSocial;
                    existing.PessoaJuridica.Cnpj        = cliente.PessoaJuridica.Cnpj;
                }
                else
                {
                    existing.PessoaJuridica = new PessoaJuridica
                    {
                        ClienteId   = existing.Id,
                        UsuarioId   = existing.UsuarioId,
                        RazaoSocial = cliente.PessoaJuridica.RazaoSocial,
                        Cnpj        = cliente.PessoaJuridica.Cnpj
                    };
                }

                // Remove PF se havia (troca de tipo PF → PJ)
                if (existing.PessoaFisica != null)
                {
                    _context.PessoasFisicas.Remove(existing.PessoaFisica);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Auditoria: Cliente {ClienteId} atualizado com sucesso pelo Usuário {UsuarioId}.", cliente.Id, cliente.UsuarioId);
        }

        public async Task DeleteAsync(int id, int usuarioId)
        {
            _context.ChangeTracker.Clear();

            var cliente = await _context.Clientes
                .Where(c => c.Id == id && c.UsuarioId == usuarioId)
                .FirstOrDefaultAsync();

            if (cliente == null) return;

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Auditoria: Cliente {ClienteId} excluído com sucesso pelo Usuário {UsuarioId}.", id, usuarioId);
        }
    }
}
