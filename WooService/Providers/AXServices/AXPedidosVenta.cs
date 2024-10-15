using System.Text.Json;
using WooService.AXServices.AIFCrearPedidos;
using WooService.Models;
using WooService.Utils;

namespace WooService.Providers.AXServices;
/// <summary>
/// Proveedor de servicios para la creación de pedidos en AX.
/// </summary>
/// <param name="appsets">Configuración de la aplicación.</param>
/// <param name="axctx">Contexto de la conexión a base de datos de AX.</param>
/// <returns>Estado de la operación. o nulo si ocurrió un error.</returns>
public class AXPedidosService(AppSettings appsets)
{
    /// <summary>
    /// Configuración de la aplicación.
    /// </summary>
    readonly AppSettings appSettings = appsets;
    /// <summary>
    /// Contexto de AX para ejecutar las operaciones.
    /// </summary>
    readonly CallContext callContext = new()
    {
        Company = appsets.AXCompany
        //Language = "es"

    };

    readonly triAIFCreateSalesQuotationOrderServiceClient client = CreateAXConnection(appsets);

    /// <summary>
    /// Se intentara generar un archivo de impresion en formato PDF,
    /// en el sistema AX de una factura especifica.
    /// </summary>
    /// <param name="AXContext">Conexión al sistema Dynamics AX</param>
    /// <param name="appSettings">Archivo de configuración del sistema, con datos para conectar al servicio de facturarcion de AX.</param>
    /// <param name="Empresa">Empresa donde se registro la factura.</param>
    /// <param name="Factura">Numero de factura que se utilizara para generar el formulario de impresión</param>
    /// <returns>Arreglo de bytes con del archivo PDF generado en AX</returns>
    public async Task<EstadoOperacionPedidoAX> CrearPedido(string EncabezadoPedidoJSON, string DetallePedidoJSON)
    {
        string msgError;
        try
        {
            var resultado = await client.AIFCreateSalesOrderWEBAsync(callContext, EncabezadoPedidoJSON, DetallePedidoJSON, false, true);
            if (resultado is not null)
            {
                EstadoOperacionPedidoAX estado = DesSerializarResultadoOperacionPedidoAX(resultado.response);
                return estado;
            }
            msgError = "Se produjo un error al crear el pedido en AX";
        }
        catch (Exception ex)
        {
            msgError = Global.GetExceptionError(ex);
        }
        return new EstadoOperacionPedidoAX()
        {
            Error = 1,
            MsgError = msgError,
        };
    }


    /// <summary>
    /// Deserializa un JSON a un objeto de tipo EstadoOperacionAX,
    /// este método es usado para obtener el resultado de la operación de los servicios de AX.
    /// </summary>
    /// <param name="JSONResultadoOperacionAX">JSON con el resultado de la operación</param>
    /// <returns>Estado de la operación, devuelto como objeto de tipo EstadoOperacionAX</returns>
    public static EstadoOperacionPedidoAX DesSerializarResultadoOperacionPedidoAX(string JSONResultadoOperacionClienteAX)
    {
        string msgError;
        try
        {
            EstadoOperacionPedidoAX? res = JsonSerializer.Deserialize<EstadoOperacionPedidoAX>(JSONResultadoOperacionClienteAX);
            if (res is not null)
            {
                return res;
            }
            msgError = "Error al obtene el resultado de la operación en AX";
        }
        catch (Exception ex)
        {
            msgError = Global.GetExceptionError(ex);
        }
        return new EstadoOperacionPedidoAX()
        {
            Error = 1,
            MsgError = msgError
        };

    }
    /// <summary>
    /// Crea la clase proxy para conectar al servicio de crear pedidos en AX
    /// </summary>
    /// <param name="appSettings">Contiene los valores de los parametros necesarios para crear la conexión</param>
    /// <returns>Clase con datos de conexión</returns>
    private static triAIFCreateSalesQuotationOrderServiceClient CreateAXConnection(AppSettings appSettings)
    {
        var endpointIdentity = new System.ServiceModel.UpnEndpointIdentity(appSettings.AXAOSPrincipalName);
        var EndPoint = new System.ServiceModel.EndpointAddress(new Uri(appSettings.AXAOSNetTCPURIPedidos), endpointIdentity);
        triAIFCreateSalesQuotationOrderServiceClient.EndpointConfiguration endPointConfig = triAIFCreateSalesQuotationOrderServiceClient.EndpointConfiguration.NetTcpBinding_triAIFCreateSalesQuotationOrderService;
        triAIFCreateSalesQuotationOrderServiceClient axClient = new(endPointConfig, EndPoint);

        //  axClient.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(appSettings.AXUser, appSettings.AXPass, appSettings.AXDomain);
        axClient.ClientCredentials.Windows.ClientCredential.Domain = appSettings.AXDomain;
        axClient.ClientCredentials.Windows.ClientCredential.UserName = appSettings.AXUser;
        axClient.ClientCredentials.Windows.ClientCredential.Password = appSettings.AXPass;

        axClient.InnerChannel.OperationTimeout = new TimeSpan(0, 15, 0);

        return axClient;
    }
}

/// <summary>
/// Datos devueltos por los servicios AIF, que se utilizan
/// para creación de pedidos y cotizaciones de venta, realizadas,
/// desde el portal web y app de ventas.
/// </summary>

public class EstadoOperacionPedidoAX
{
    /// <summary>
    /// No. de pedido generado por sistema.
    /// </summary>
    public string SalesId { get; set; } = "";
    /// <summary>
    /// Numero de error generado por el servicio de creación de pedidos.
    /// </summary>
    public int Error { get; set; }

    /// <summary>
    /// Texto con mensaje de error, si fallo el servicio AIF.
    /// </summary>
    public string MsgError { get; set; } = "";

    /// <summary>
    /// Numero de factura interno, generado en Sistema AX.
    /// </summary>
    public string InvoiceId { get; set; } = "";
}
