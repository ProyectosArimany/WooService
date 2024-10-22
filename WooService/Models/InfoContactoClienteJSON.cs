using WooService.Providers.AXServices;

namespace WooService.Models;

/// <summary>
/// Información de contacto de un cliente. Para sistema AX
/// </summary>
public class InfoContactoClienteJSON
{
    /// <summary>
    /// Código de cliente asignado por sistema AX
    /// </summary>
    public string CodigoCliente { get; set; } = "";

    /// <summary>
    /// Id del registro de contacto, generado en sistema Ax.
    /// </summary>
    public Int64 RecIdDatoContacto { get; set; }

    /// <summary>
    /// Tipo de contacto
    public LogisticsElectronicAddressMethodType TipoDatoContacto { get; set; }

    public string Descripcion { get; set; } = "";

    public string Dato { get; set; } = "";

    public int EsPrimario { get; set; }
}