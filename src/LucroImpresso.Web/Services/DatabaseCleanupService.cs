using LucroImpresso.Web.Interfaces;

namespace LucroImpresso.Web.Services
{
    public class DatabaseCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseCleanupService> _logger;

        public DatabaseCleanupService(IServiceProvider serviceProvider, ILogger<DatabaseCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de Limpeza Automática iniciado.");

            // PeriodicTimer para rodar uma vez por dia
            using PeriodicTimer timer = new(TimeSpan.FromDays(1));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await DoCleanup();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Serviço de Limpeza Automática parando.");
            }
        }

        private async Task DoCleanup()
        {
            _logger.LogInformation("Executando limpeza de orçamentos antigos (> 180 dias)...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IOrcamentoRepository>();
                await repo.DeleteAntigosAsync(180);
            }

            _logger.LogInformation("Limpeza concluída com sucesso.");
        }
    }
}
