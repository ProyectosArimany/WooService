using WooService.BL;
using WooService.Contexts;
using WooService.Models;
using WooService.Providers;
using WooService.Utils;

namespace WooService.Workers;


public class WooServiceWorker(ILogger<WooServiceWorker> logger, AppSettings appsets, WooCommerceContext context, AXContext aXContext)
{
    private readonly ILogger<WooServiceWorker> _logger = logger;
    private readonly AppSettings _appSettings = appsets;
    private readonly WooCommerceContext _context = context;

    /// <summary>
    /// Procesa los pedidos de WooCommerce según el estado especificado.
    /// </summary>
    /// <param name="OrderStatus">Estado del pedido a procesar.</param>
    /// <returns>Nada</returns>

    public async Task ProcessOrders(string OrderStatus)
    {

        try
        {
            /// Cliente de WooCommerce.
            WooProvider _wooProvider = new(appsets.WooURL, appsets.WooConsumerKey, appsets.WooConsumerSecret, _logger);

            /// Parámetros de WooCommerce, para obtener código utilizado para cargos por envio.
            (ParametrosWooCommerce? ParametrosLineasPedido, string Error) = await WooServiceBL.ObtenerParametros(_context);
            if (!Global.StrIsBlank(Error))
            {
                _logger.LogError("{Message}", Error);
                return;
            }

            (ParametrosClientesNuevosWEB? ParametrosClientes, Error) = await ServiciosAXBL.ObtenerParametrosClientesNuevosWEB(aXContext);
            if (!Global.StrIsBlank(Error))
            {
                _logger.LogError("{Message}", Error);
                return;
            }

            var orders = await _wooProvider.ObtenerPedidos(OrderStatus);

            foreach (var order in orders)
            {
                (WooPedido? pedido, Error, String Warnings) = await WooServiceBL.CreateOrUpdateWooPedido(_context, aXContext, order, ParametrosClientes, ParametrosLineasPedido, _appSettings);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing orders");
            // await NotifyAdministrator(_appSettings.EMAILUser, _appSettings.EMAILPass, _appSettings.EMAILUser, "Error processing orders", ex.ToString());
        }
    }

}