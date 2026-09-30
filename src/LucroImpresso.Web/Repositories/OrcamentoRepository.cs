using LucroImpresso.Web.Data;
using LucroImpresso.Web.Interfaces;
using LucroImpresso.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LucroImpresso.Web.Repositories
{
    public class OrcamentoRepository : IOrcamentoRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrcamentoRepository> _logger;

        public OrcamentoRepository(AppDbContext context, ILogger<OrcamentoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        private IQueryable<Orcamento> QueryBase(int usuarioId) =>
            _context.Orcamentos
                .Where(o => o.UsuarioId == usuarioId)
                .Include(o => o.Material)
                .Include(o => o.Maquina)
                .Include(o => o.Cliente)
                    .ThenInclude(c => c!.PessoaFisica)
                .Include(o => o.Cliente)
                    .ThenInclude(c => c!.PessoaJuridica)
                .Include(o => o.OrcamentoAcabamentos)
                    .ThenInclude(oa => oa.Acabamento)
                .Include(o => o.Peca);

        public async Task<List<Orcamento>> GetAllAsync(int usuarioId)
        {
            return await QueryBase(usuarioId).ToListAsync();
        }

        public async Task AddAsync(Orcamento orcamento)
        {
            if (orcamento.UsuarioId <= 0)
                throw new InvalidOperationException("Não é possível cadastrar orçamento sem um Usuário válido (Multi-Tenant).");

            await _context.Orcamentos.AddAsync(orcamento);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Auditoria: Orçamento {OrcamentoId} ({NomeProjeto}) cadastrado para Usuário {UsuarioId}.", orcamento.Id, orcamento.UsuarioId, orcamento.NomeProjeto);
        }

        public async Task UpdateAsync(Orcamento orcamento)
        {
            _context.ChangeTracker.Clear();
            var existente = await _context.Orcamentos.AsNoTracking().FirstOrDefaultAsync(o => o.Id == orcamento.Id);
            if (existente == null) return;

            // Segurança Multi-Tenant (Anti-IDOR): Preserva o tenant proprietário do registro
            orcamento.UsuarioId = existente.UsuarioId;
            _context.Orcamentos.Update(orcamento);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Auditoria: Orçamento {OrcamentoId} atualizado com sucesso para Usuário {UsuarioId}. Novo status: {Status}", orcamento.Id, orcamento.UsuarioId, orcamento.Status);
        }

        public async Task DeleteAsync(int id, int usuarioId)
        {
            var orcamento = await QueryBase(usuarioId)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orcamento != null)
            {
                var resumo = await _context.HistoricoFinanceiro.FirstOrDefaultAsync(r => r.UsuarioId == usuarioId);
                if (resumo == null)
                {
                    resumo = new HistoricoFinanceiro { UsuarioId = usuarioId };
                    _context.HistoricoFinanceiro.Add(resumo);
                }

                if (orcamento.Status == StatusOrcamento.Concluido)
                {
                    resumo.ReceitasAcumuladas += orcamento.CustoFinal;
                }
                if (orcamento.Status == StatusOrcamento.Concluido || orcamento.Status == StatusOrcamento.Falha)
                {
                    var consumoW = orcamento.Maquina?.ConsumoWatts ?? 0;
                    var custoKwh = orcamento.Maquina?.CustoKwh ?? 0;
                    resumo.DespesasEnergiaAcumuladas += (consumoW / 1000m) * orcamento.TempoHrsImpressao * custoKwh;
                }

                _context.Orcamentos.Remove(orcamento);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Auditoria: Orçamento {OrcamentoId} excluído pelo Usuário {UsuarioId}. Dados financeiros preservados no cofre.", id, usuarioId);
            }
        }

        public async Task<decimal> GetTotalFaturamentoAsync(int usuarioId)
        {
            var orcamentos = await QueryBase(usuarioId)
                .Where(o => o.Status == StatusOrcamento.Concluido)
                .ToListAsync();

            var somaAtivos = orcamentos.Sum(o => o.CustoFinal);
            var resumo = await _context.HistoricoFinanceiro.FirstOrDefaultAsync(r => r.UsuarioId == usuarioId);
            return somaAtivos + (resumo?.ReceitasAcumuladas ?? 0);
        }

        public async Task<int> GetTotalPecasAsync(int usuarioId)
        {
            return await _context.Orcamentos
                .Where(o => o.UsuarioId == usuarioId && o.Status == StatusOrcamento.Concluido)
                .CountAsync();
        }

        public async Task<List<Orcamento>> GetUltimosOrcamentosAsync(int quantidade, int usuarioId)
        {
            return await QueryBase(usuarioId)
                .OrderByDescending(o => o.DataCriacao)
                .Take(quantidade)
                .ToListAsync();
        }

        public async Task DeleteAntigosAsync(int dias)
        {
            var dataLimite = DateTime.UtcNow.AddDays(-dias);
            var antigos = await _context.Orcamentos
                .Include(o => o.Maquina)
                .Include(o => o.Material)
                .Where(o => o.DataCriacao < dataLimite)
                .ToListAsync();

            if (antigos.Any())
            {
                // COFRE ACUMULADOR: Salva a receita do orçamento antes de apagá-lo
                foreach (var orcamento in antigos)
                {
                    var resumo = await _context.HistoricoFinanceiro.FirstOrDefaultAsync(r => r.UsuarioId == orcamento.UsuarioId);
                    if (resumo == null)
                    {
                        resumo = new HistoricoFinanceiro { UsuarioId = orcamento.UsuarioId };
                        _context.HistoricoFinanceiro.Add(resumo);
                    }

                    if (orcamento.Status == StatusOrcamento.Concluido)
                    {
                        resumo.ReceitasAcumuladas += orcamento.CustoFinal;
                    }
                    if (orcamento.Status == StatusOrcamento.Concluido || orcamento.Status == StatusOrcamento.Falha)
                    {
                        var consumoW = orcamento.Maquina?.ConsumoWatts ?? 0;
                        var custoKwh = orcamento.Maquina?.CustoKwh ?? 0;
                        resumo.DespesasEnergiaAcumuladas += (consumoW / 1000m) * orcamento.TempoHrsImpressao * custoKwh;
                    }
                }

                _context.Orcamentos.RemoveRange(antigos);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Auditoria Rotina de Limpeza: {Quantidade} orçamentos antigos (> {Dias} dias) arquivados e removidos com segurança.", antigos.Count, dias);
            }
        }

        public async Task<decimal> GetTotalDespesasEnergiaAsync(int usuarioId)
        {
            var impressos = await QueryBase(usuarioId)
                .Where(o => o.Status == StatusOrcamento.Concluido || o.Status == StatusOrcamento.Falha)
                .ToListAsync();

            var somaAtivos = impressos.Sum(o =>
            {
                var consumoW = o.Maquina?.ConsumoWatts ?? 0;
                var custoKwh = o.Maquina?.CustoKwh ?? 0;
                return (consumoW / 1000m) * o.TempoHrsImpressao * custoKwh;
            });

            var resumo = await _context.HistoricoFinanceiro.FirstOrDefaultAsync(r => r.UsuarioId == usuarioId);
            return somaAtivos + (resumo?.DespesasEnergiaAcumuladas ?? 0);
        }
    }
}
