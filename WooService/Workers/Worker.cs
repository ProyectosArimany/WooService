using WooService.Models;

namespace WooService.Workers
{
    public class Worker(ILogger<Worker> logger, AppSettings appSettings) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly AppSettings _appSettings = appSettings;



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool isLoggingEnabled = _logger.IsEnabled(LogLevel.Information);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (isLoggingEnabled)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await Task.Delay(1000, stoppingToken);

                // Agregar una pausa de 5 segundos
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
