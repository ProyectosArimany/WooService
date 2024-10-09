namespace WooService.Providers;

using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;
using Microsoft.Extensions.Logging;
using WooService.Utils;

public class WooProvider
{
    private readonly RestAPI _restApi;
    private readonly WCObject _wooCommerceObject;
    private readonly ILogger<WooProvider> _logger;
    private string MsgError = "";
    public readonly Dictionary<string, string> EstadosWooCommerce = new()
    {
        {"pending", "pending"},
        {"processing", "processing"},
        { "on-hold", "on-hold"},
        { "completed", "completed"},
        { "cancelled", "cancelled"},
        { "refunded", "refunded"},
        { "failed", "failed"},
        { "empacando", "empacando"}
    };

    public WooProvider(string wooCommerceApiUri, string wooCommerceApiKey, string wooCommerceApiSecret, ILogger<WooProvider> logger)
    {
        _logger = logger;
        MsgError = "";
        try
        {
            _restApi = new RestAPI(wooCommerceApiUri, wooCommerceApiKey, wooCommerceApiSecret);
            _wooCommerceObject = new WCObject(_restApi);
        }
        catch (Exception ex)
        {
            MsgError = Global.GetExceptionError(ex);
            _logger.LogError(ex, "Error al inicializar WooProvider");
            throw;
        }
    }

    public async Task<List<Order>> ObtenerPedidos(string statusPedido)
    {
        MsgError = "";
        try
        {
            var pedidos = await _wooCommerceObject.Order.GetAll(new Dictionary<string, string>
                {
                    {"status", statusPedido}
                });
            return [.. pedidos];
        }
        catch (Exception ex)
        {
            MsgError = Global.GetExceptionError(ex);
            _logger.LogError(ex, "Error al obtener pedidos");
            return [];
        }
    }

    public async Task<bool> ActualizarEstadoPedido(ulong orderId, string nuevoEstado)
    {
        MsgError = "";
        try
        {
            var pedido = await _wooCommerceObject.Order.Get(orderId);
            if (pedido != null)
            {
                pedido.status = nuevoEstado;
                await _wooCommerceObject.Order.Update(orderId, pedido);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            string msgError = $"Error al actualizar el estado del pedido {orderId}";
            MsgError = msgError + Environment.NewLine + Global.GetExceptionError(ex);
            _logger.LogError(ex, "{Message}", msgError);
            return false;
        }
    }
}
