namespace WooService.Models
{
    /// <summary>
    /// Ficha de clientes en sistema AX
    /// </summary>
    public class ClienteAX
    {

        /// <summary>
        /// Código de cliente en sistema AX.
        /// </summary>
        public string Cliente { get; set; } = "";

        /// <summary>
        /// Nombre de cliente en sistema AX.
        /// </summary>

        public string RazonSocial { get; set; } = "";

        /// <summary>
        /// NIT de cliente en sistema AX.
        /// </summary>
        public string NIT { get; set; } = "";
    }
}