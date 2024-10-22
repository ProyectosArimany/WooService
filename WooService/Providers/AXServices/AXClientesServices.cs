using WooService.Models;
using WooService.AXServices.AIFCrearClientes;
using WooService.Utils;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.ServiceModel;
using System.Text;

namespace WooService.Providers.AXServices;
/// <summary>
/// Proveedor de servicios para la creación de clientes en AX. 
/// Servicios AIF/WCF expuestos en Dynamics AX.
/// </summary>
public class AXClientesServices
{
    /// <summary>
    /// Contexto de llamada de servicios AIF de AX.
    /// </summary>
    readonly CallContext callContext;

    /// <summary>
    /// Configuración de la aplicación.
    /// </summary>
    readonly AppSettings appSettings;

    /// <summary>
    /// Cliente para consumir los servicios de creación de clientes en AX.
    /// </summary>
    readonly triAIFCrearClienteServiceClient client;

    /// <summary>
    /// Constructor de la clase.
    /// </summary>
    /// <param name="appsets">Configuración de la aplicación</param>
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
    public async Task<ResultadoOperacionAX> CrearDireccionCliente(ClienteDireccion direccion, string NombreCliente, String Pais)
    {

        if (Global.StrIsBlank(direccion.Cliente.ClienteAXId))
            return new ResultadoOperacionAX("El código de cliente es obligatorio para crear una dirección.", "", "", 0);

        DireccionClienteJSON direccionCliente = new()
        {
            CodigoCliente = direccion.Cliente.ClienteAXId,
            RecIdDeDireccion = 0,
            TipoDireccion = (int)direccion.TipoDireccion,
            Descripcion = direccion.TipoDireccion == LogisticsLocationRoleType.Invoice ? NombreCliente : "DIRECCION DE ENVIO",
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


    /// <summary>
    /// Crea un nuevo contacto de cliente en AX.
    /// </summary>
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
    /// Crea el cliente proxy para conectar al servicio de facturación en AX (AIF).
    /// </summary>
    /// <param name="appSettings">Contiene los valores de los parametros necesarios para crear la conexión</param>
    /// <returns>El Objeto proxy instanciado, con datos para conectar al sistema Dynamics AX</returns>
    private static triAIFCrearClienteServiceClient CreateAXClient(AppSettings appSettings)
    {
        // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        var binding = new NetTcpBinding(SecurityMode.TransportWithMessageCredential);
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
        var endpointIdentity = new UpnEndpointIdentity(appSettings.AXAOSPrincipalName);
        var EndPoint = new EndpointAddress(new Uri(appSettings.AXAOSNetTCPURIClientes), endpointIdentity);

        triAIFCrearClienteServiceClient.EndpointConfiguration endPointConfig = triAIFCrearClienteServiceClient.EndpointConfiguration.NetTcpBinding_triAIFCrearClienteService;
        triAIFCrearClienteServiceClient axClient = new(endPointConfig, EndPoint);

        axClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(appSettings.AXUser, appSettings.AXPass, appSettings.AXDomain);
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
                if (Global.StrIsBlank(res.ErrorMsg)) res.ErrorMsg = "";
                else
                {
                    byte[] base64bytes = Convert.FromBase64String(res.ErrorMsg);
                    res.ErrorMsg = Encoding.UTF8.GetString(base64bytes);
                }
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
public enum LogisticsLocationRoleType
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
/// Tipos de métodos de contacto electrónico utilizados en sistema AX.
/// </summary>
public enum LogisticsElectronicAddressMethodType
{
    /// <summary>
    /// Sin tipo de contaco
    /// </summary>
    None = 0,
    /// <summary>
    /// El contacto es un número de teléfono.
    /// </summary>
    Teléfono = 1,
    /// <summary>
    /// El contacto es una dirección de correo electrónico.
    /// </summary>
    Email = 2,
    /// <summary>
    /// El contacto es una dirección URL.
    /// </summary>
    URL = 3,
    /// <summary>
    /// El contacto es un número de telex.
    /// </summary>
    Telex = 4,

    /// <summary>
    /// El contacto es un número de fax.
    /// </summary>
    Fax = 5,
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
