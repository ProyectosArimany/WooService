
namespace WooService.Models
{
    /// <summary>
    /// AppSettings class, almacena las configuraciones de la aplicación,
    /// se obtiene de appsettings.json.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// WooCommerceConnectionString, cadena de conexión a la base de datos de WooCommerce.
        /// </summary>


        public string WooCommerceConnectionString { get; set; } = "";

        /// <summary>
        /// AXConnectionString, cadena de conexión a la base de datos de Dynamics AX.
        /// </summary>

        public string AXConnectionString { get; set; } = "";

        /// <summary>
        /// SMTPServer, nombre o ip del servidor SMTP. Utilzado para el 
        /// envío de notificaciones por correo electrónico.
        /// </summary>

        public string SMTPServer { get; set; } = "";

        /// <summary>
        /// SMTPPort, puerto del servidor SMTP. Utilzado para el
        /// envío de notificaciones por correo electrónico.

        public string SMTPPort { get; set; } = "";

        /// <summary>
        /// SMTPEnableSSL, indica si la conexión al servidor SMTP es cifrada.
        /// </summary>

        public bool SMTPEnableSSL { get; set; } = false;

        /// <summary>
        /// Cuenta del buzón de correo electrónico utilizada para
        /// enviar notificaciones.
        /// </summary>

        public string EMAILUser { get; set; } = "";

        /// <summary>
        /// Contraseña de la cuenta de correo electrónico.
        /// </summary>

        public string EMAILPass { get; set; } = "";

        /// <summary>
        /// Correo de la persona que recibira las notificaciones,
        /// generadas por este servicio.
        /// </summary>

        public string EMAILAdmin { get; set; } = "";

        /// <summary>
        /// AXAOSPrincipalName, usuario de AD con el que se 
        /// ejecuta la instancia de AOS de Dynamics AX.
        /// </summary>
        public string AXAOSPrincipalName { get; set; } = "";

        /// <summary>
        /// URI del servicio de Dynamics AX. Servicios AIF, 
        /// para registro de pedidos.
        /// </summary>
        public string AXAOSNetTCPURIPedidos { get; set; } = "";

        /// <summary>
        /// URI del servicio de Dynamics AX. Servicios AIF,
        /// para registro de clientes.
        /// </summary>
        public string AXAOSNetTCPURIClientes { get; set; } = "";

        /// <summary>
        /// AXDomain, dominio de AD para la autenticación de Dynamics AX.
        /// </summary>
        public string AXDomain { get; set; } = "";

        /// <summary>
        /// AXUser, usuario de Dynamics AX. con privilegios para
        /// registrar pedidos y clientes.
        /// </summary>
        public string AXUser { get; set; } = "";

        /// <summary>
        /// Contraseña del usuario de Dynamics AX.
        /// </summary>
        public string AXPass { get; set; } = "";

        /// <summary>
        /// Empresa de Dynamics AX, para el registro de transacciones.
        /// <summary>
        public string AXCompany { get; set; } = "";

        /// <summary>
        /// AXLang, idioma de Dynamics AX. Para realizar las peticiones
        /// a los servicios AIF.
        /// </summary>
        public string AXLang { get; set; } = "";

        /// <summary>
        /// Clave de consumidor para conectar a las API de WooCommerce.
        /// </summary>
        public string WooConsumerKey { get; set; } = "";

        /// <summary>
        /// Clave secreta de consumidor para conectar a las API de WooCommerce.
        /// </summary>
        public string WooConsumerSecret { get; set; } = "";

        /// <summary>
        /// URL del servicio wooCommerce.
        /// </summary>
        public string WooURL { get; set; } = "";
        /// <summary>
        /// Utilizar este valor para obtener los pedidos del portal
        /// woocomerce. Obtener pedidos con el estado [WooEstadoPedido]
        /// </summary>
        public string WooEstadoPedido { get; set; } = "";

        /// <summary>
        /// País de residencia del cliente.  CountryRegionId en Dynamics AX.
        /// </summary>
        public string Pais { get; set; } = "";
    }
}

