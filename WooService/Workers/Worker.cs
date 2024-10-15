using WooService.Contexts;
using WooService.Models;
using WooService.Providers;

namespace WooService.Workers
{
    public class Worker(ILogger<Worker> logger, AppSettings appSettings, IServiceScopeFactory _serviceScopeFactory) : BackgroundService
    {



        /// <summary>
        /// Método que se ejecuta en segundo plano Tarea principal.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            /// Verificar si el sistema de registro de eventos, está habilitado.
            bool isLoggingEnabled = logger.IsEnabled(LogLevel.Information);

            /// Tiempo de espera entre ejecuciones.
            TimeSpan TiempoEsperaEnMinutos = TimeSpan.FromMinutes(appSettings.TiempoEsperaEnMinutos);

            /// Crear un nuevo alcance para obtener los contextos de base de datos.
            using var scope = _serviceScopeFactory.CreateScope();

            /// Contexto de bases de datos.
            var wooCommerceContext = scope.ServiceProvider.GetRequiredService<WooCommerceContext>();
            var axContext = scope.ServiceProvider.GetRequiredService<AXContext>();

            /// Instancia del servicio de WooCommerce.
            WooServiceWorker wooServiceWorker = new(logger, appSettings, axContext, wooCommerceContext);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (isLoggingEnabled)
                {
                    logger.LogInformation("Se inicio el proceso a las: {time}", DateTimeOffset.Now);
                }
                await wooServiceWorker.ProcessOrders(WooProvider.getWoocommerceStatus(appSettings.WooEstadoPedido), isLoggingEnabled);
                if (isLoggingEnabled)
                {
                    logger.LogInformation("Se termino el proceso a las: {time}", DateTimeOffset.Now);
                }
                // Agregar una pausa en minutos
                await Task.Delay(TiempoEsperaEnMinutos, stoppingToken);
            }
        }
    }
}
