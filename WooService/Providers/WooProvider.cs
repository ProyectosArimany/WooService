namespace WooService.Providers;

using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;
using Microsoft.Extensions.Logging;
using WooService.Utils;
using System.Timers;

/// <summary>
/// Proveedor de servicios para administrar pedidos del protal arimany.com.
/// </summary>
public class WooProvider
{
    /// <summary>
    /// API de WooCommerce (arimany.com).
    /// </summary>
    private readonly RestAPI? _restApi;

    /// <summary>
    /// Objeto de acceso a la API de WooCommerce.
    /// </summary>
    private readonly WCObject? _wooCommerceObject;

    /// <summary>
    /// Mensajes de error producidos en el servicio.
    /// </summary>
    private string MsgError;

    /// <summary>
    /// Obtiene el mensaje de error producido en el servicio.
    /// </summary>
    /// <returns>Texto con el error producido</returns>
    public string GetMsgError { get => MsgError; }

    /// <summary>
    /// Listado de estados de pedidos en WooCommerce.
    /// </summary>
    public static readonly Dictionary<string, string> EstadosWooCommerce = new()
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

    /// <summary>
    /// Obtiene el estado de un pedido en WooCommerce.
    /// </summary>
    /// <param name="status">Clave del estado</param>
    /// <returns>Estado de pedido.</returns>
    public static string getWoocommerceStatus(string status) => EstadosWooCommerce[status];

    public WooProvider(string wooCommerceApiUri, string wooCommerceApiKey, string wooCommerceApiSecret)
    {
        MsgError = "";
        try
        {
            _restApi = new RestAPI(wooCommerceApiUri, wooCommerceApiKey, wooCommerceApiSecret);
            _wooCommerceObject = new WCObject(_restApi);
        }
        catch (Exception ex)
        {
            _restApi = null;
            _wooCommerceObject = null;
            MsgError = Global.GetExceptionError(ex);
        }
    }

    /// <summary>
    /// Obtiene los pedidos del portal web según el estado especificado. Estado debe ser empacando.
    /// </summary>
    /// <param name="statusPedido"></param>
    /// <returns></returns>
    public async Task<List<Order>> ObtenerPedidos(string statusPedido)
    {
        if (_wooCommerceObject == null)
        {
            MsgError = "el objeto _wooCommerceObject no ha sido inicializado.";
            return [];
        }

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
            MsgError = "Error al obtener pedidos del portal web" + Environment.NewLine + Global.GetExceptionError(ex);
            return [];
        }
    }

    /// <summary>
    /// Actualiza el estado de un pedido en el portal web.
    /// </summary>
    /// <param name="orderId">Id del pedido en WooCommerce</param>
    /// <param name="nuevoEstado">Estado a establecer en el pedido</param>
    /// <returns>True si la operación se realizó con éxito, false de lo contrario.</returns>
    public async Task<bool> ActualizarEstadoPedido(ulong orderId, string nuevoEstado)
    {
        if (_wooCommerceObject == null)
        {
            MsgError = "el objeto _wooCommerceObject no ha sido inicializado.";
            return false;
        }
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
            return false;
        }
    }
}
