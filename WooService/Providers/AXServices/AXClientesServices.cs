using WooService.Models;
using WooService.AXServices.AIFCrearClientes;
using WooService.Utils;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;

namespace WooService.Providers.AXServices;

public class AXClientesServices
{

    readonly CallContext callContext;
    readonly AppSettings appSettings;
    readonly triAIFCrearClienteServiceClient client;

    public AXClientesServices(AppSettings appsets)
    {
        this.appSettings = appsets;
        this.callContext = new()
        {
            Company = appsets.AXCompany
            //Language = "es"

        };

        this.client = CreateAXClient(appsets);

    }

    /// <summary>
    /// Registra coordenadas geograficas de un cliente en AX
    /// </summary>
    /// <param name="cliente">Código de cliente en AX</param>
    /// <param name="direccion">Id o RecId de dirección del cliente</param>
    /// <param name="latitud">Latitud</param>
    /// <param name="longitud">Longitud</param>
    /// <returns>Estado de la operación</returns>
    public async Task<ResultadoOperacionAX> ActualizarCoordenasGeograficas(String cliente, long direccion, decimal latitud, decimal longitud)
    {

        string msgError;
        if (String.IsNullOrWhiteSpace(cliente) || direccion <= 0)
        {
            msgError = "El cliente y la dirección, para registrar coordenadas, es obligatorio.";
            return new ResultadoOperacionAX(msgError, "", "", 0);
        }

        try
        {

            var result = await client.AIFActualizarCoordenasClienteAsync(this.callContext, cliente, direccion, latitud, longitud);

            if (result != null)
            {
                string resp = result.response;
                ResultadoOperacionAX? res = DesSerializarResultadoOperacionAX(resp);
                return res;

            }
            else
            {
                msgError = $"No se completó la tarea actualizar cliente en AX";
            }
        }
        catch (Exception ex)
        {
            msgError = Global.GetExceptionError(ex);
        }
        return new ResultadoOperacionAX(msgError, "", "", 0);
    }

    /// <summary>
    /// Crea un nuevo cliente en AX o actualiza uno existente.
    /// </summary>
    /// <param name="cliente">dAtos del cliente</param>
    /// <param name="ParamClientes">Parametros para clientes nuevos</param>
    /// <param name="Pais">Pais de residencia del cliente</param>
    /// <param name="Depto">Departamento de residencia del cliente</param>
    /// <returns>Estado de la operación</returns>
    public async Task<ResultadoOperacionAX> CrearClienteWEB(Cliente cliente, ParametrosClientesNuevosWEB ParamClientes, String Pais)
    {

        String Depto, msgError;
        if (cliente is null || ParamClientes is null || cliente.Direcciones is null || cliente.Direcciones.Count == 0)
        {
            msgError = "Datos del cliente o parametros para crear cliente son obligatorios.";
            return new ResultadoOperacionAX(msgError, "", "", 0);
        }

        if (String.IsNullOrWhiteSpace(Pais)) return new ResultadoOperacionAX("El país de residencia del cliente es obligatorio.", "", "", 0);

        Depto = cliente.Direcciones[0].DepartamentoAXId;

        FichaClienteJSON fichaCliente = new()
        {
            CodigoCliente = cliente.ClienteAXId,
            CondicionEntrega = ParamClientes.TerminoEntrega,
            ModoEntrega = ParamClientes.ModoDeEntrega,
            FormaPago = ParamClientes.FormaPago,
            NIT = cliente.Nit,
            TipoCliente = 0,  // 0 = Individual
            Nombre = cliente.Nombres + " " + cliente.Apellidos,
            DoctoIdentificacion = "",
            Pais = Pais,
            Depto = Depto
        };


        try
        {
            string JSONFichaCliente = JsonSerializer.Serialize(fichaCliente);

            var response = await client.AIFCrearClienteAsync(this.callContext, JSONFichaCliente);

            if (response != null)
            {
                string resp = response.response;
                ResultadoOperacionAX res = DesSerializarResultadoOperacionAX(resp);
                return res;
            }
            else
            {
                msgError = $"No se completó la tarea actualizar cliente en AX";
            }
        }
        catch (Exception ex)
        {
            msgError = Global.GetExceptionError(ex);

        }
        return new ResultadoOperacionAX(msgError, "", "", 0);
    }

    /// <summary>
    /// Crea una nueva dirección de cliente en AX o actualiza una existente.
    /// </summary>
    /// <param name="JSONDireccionCliente">JSON con datos de la dirección</param>
    /// <returns>Estado de la operación</returns>   
    public async Task<ResultadoOperacionAX> CrearDireccionCliente(ClienteDireccion direccion, String Pais)
    {

        if (Global.StrIsBlank(direccion.Cliente.ClienteAXId))


            return new ResultadoOperacionAX("El código de cliente es obligatorio para crear una dirección.", "", "", 0);
         ]
        
        DireccionClienteJSON direccionCliente = new()
        {
            CodigoCliente = direccion.Cliente.ClienteAXId,
            RecIdDeDireccion = 0,
            TipoDireccion = direccion.TipoDireccion,
            Descripcion = direccion.Direccion,
            Pais = Pais,
            Depto = direccion.DepartamentoAXId,
            Municipio = direccion.MunicipioAXId,
            Calle = direccion.Direccion + " " + direccion.DireccionComplemento,
            EsPrimaria = 1,
        };

        string msgError;
        string JSONDireccionCliente = JsonSerializer.Serialize(direccionCliente);
        try
        {
            var response = await client.AIFCrearDireccionAsync(this.callContext, JSONDireccionCliente);
            if (response != null)
            {
                ResultadoOperacionAX? res = DesSerializarResultadoOperacionAX(response.response);
                return res;
            }
            else
            {
                msgError = $"No se completó la tarea actualizar dirección de cliente en AX";
            }
        }
        catch (Exception ex)
        {
            msgError = Global.GetExceptionError(ex);
        }
        return new ResultadoOperacionAX(msgError, "", "", 0);
    }

    public async Task<ResultadoOperacionAX> CrearContactoDeCliente(string JSONContactoCliente)
    {
        string msgError = "";
        try
        {
            var response = await client.AIFCrearContactoAsync(this.callContext, JSONContactoCliente);
            if (response != null)
            {
                ResultadoOperacionAX res = DesSerializarResultadoOperacionAX(response.response);
                return res;
            }
            else
            {
                msgError = $"No se completó la tarea actualizar dirección de cliente en AX";
            }
        }
        catch (Exception ex)
        {
            msgError = Global.GetExceptionError(ex);
        }
        return new ResultadoOperacionAX(msgError, "", "", 0);
    }

    /// <summary>
    /// Crea la clase proxy para conexion al servicio de facturación en AX
    /// </summary>
    /// <param name="appSettings">Contiene los valores de los parametros necesarios para crear la conexión</param>
    /// <returns>Clase con datos de conexión</returns>
    private static triAIFCrearClienteServiceClient CreateAXClient(AppSettings appSettings)
    {
        var endpointIdentity = new System.ServiceModel.UpnEndpointIdentity(appSettings.AXAOSPrincipalName);
        var EndPoint = new System.ServiceModel.EndpointAddress(new Uri(appSettings.AXAOSNetTCPURIClientes), endpointIdentity);
        triAIFCrearClienteServiceClient.EndpointConfiguration endPointConfig = triAIFCrearClienteServiceClient.EndpointConfiguration.NetTcpBinding_triAIFCrearClienteService;

        triAIFCrearClienteServiceClient axClient = new(endPointConfig, EndPoint);

        //  axClient.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(appSettings.AXUser, appSettings.AXPass, appSettings.AXDomain);
        axClient.ClientCredentials.Windows.ClientCredential.Domain = appSettings.AXDomain;
        axClient.ClientCredentials.Windows.ClientCredential.UserName = appSettings.AXUser;
        axClient.ClientCredentials.Windows.ClientCredential.Password = appSettings.AXPass;

        axClient.InnerChannel.OperationTimeout = new TimeSpan(0, 15, 0);

        return axClient;
    }

    /// <summary>
    /// Deserializa un JSON a un objeto de tipo EstadoOperacionAX,
    /// este método es usado para obtener el resultado de la operación de los servicios de AX.
    /// </summary>
    /// <param name="JSONResultadoOperacionAX">JSON con el resultado de la operación</param>
    /// <returns>Estado de la operación, devuelto como objeto de tipo EstadoOperacionAX</returns>
    public static ResultadoOperacionAX DesSerializarResultadoOperacionAX(string JSONResultadoOperacionAX)
    {
        string msgError;
        try
        {
            ResultadoOperacionAX? res = JsonSerializer.Deserialize<ResultadoOperacionAX>(JSONResultadoOperacionAX);
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
        return new ResultadoOperacionAX(msgError, "", "", 0);
    }
}

/// <summary>
/// Tipos de direcciones utilizadas en sistema AX.
/// </summary>
enum LogisticsLocationRoleType
{
    Definidoporelusuario = 0,  // Definido por el usuario
    Invoice = 1,               // Facturación
    Delivery = 2,              // Entrega
    SWIFT = 4,                 // Código SWIFT
    Payment = 5,               // Pago
    Service = 6,               // Servicio
    Home = 7,                  // Hogar
    Other = 8,                 // Otro
    Business = 9,              // Negocio
    Remitto = 10,              // Remitir a
    Thirdpartyshipping = 11,   // Envío a terceros
    Extracto = 12,             // Extracto
    Fixedasset = 15,           // Activo fijo
    Onetime = 16,              // Una vez
    Recruit = 17,              // Reclutamiento
    SMS = 18,                  // SMS
    Lading = 101,              // Carga
    Unlading = 102,            // Descarga
    Consignment = 103,         // Consignación
    Reallocation = 150         // Reasignación
}




/// <summary>
/// Clase para recibir el resultado de la operación de los servicios de AX
/// </summary>
public class ResultadoOperacionAX
{

    /// <summary>
    /// Error generado en sistema AX
    /// </summary>
    public string ErrorMsg { get; set; } = string.Empty;

    /// <summary>
    /// Codigo de cliente en AX
    /// </summary>
    public string IdCliente { get; set; } = string.Empty;

    /// <summary>
    /// Advertencias generadas en el proceso, se concluyo el proceso pero hay inconsistencias
    /// </summary>
    public string Advertencia { get; set; } = string.Empty;

    /// <summary>
    /// Numero de registro en la tabla CusTable de AX CustTable.RecId
    /// </summary>
    public long RecId { get; set; } = 0;

    public ResultadoOperacionAX() { }
    public ResultadoOperacionAX(String errorMsg, String idCliente, String advertencia, long recId)
    {
        this.ErrorMsg = errorMsg;
        this.IdCliente = idCliente;
        this.Advertencia = advertencia;
        this.RecId = recId;
    }

}
