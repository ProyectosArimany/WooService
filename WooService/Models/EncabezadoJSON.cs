using System;

namespace WooService.Models;
using Newtonsoft.Json;

/// <summary>
/// Encabezado de pedido, enviado a Sistema
/// </summary>
public class EncabezadoJSON
{
    /// <summary>
    /// Código de cliente en sistema AX
    /// </summary>

    public string Cliente { get; set; } = string.Empty;

    /// <summary>
    /// Dirección de entrega del pedido
    /// </summary>

    public long Direccion { get; set; } = 0;

    /// <summary>
    /// Fecha de recepción del pedido
    /// </summary>

    public string FechaRecepcion { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de envío del pedido
    /// </summary>

    public string FechaEnvio { get; set; } = string.Empty;

    /// <summary>
    /// Condiciones de pago, esta se obtiene de la tabla de parametros para wooservice,
    /// en sistema AX.
    /// </summary>
    [JsonProperty(nameof(CondicionesPago))]
    public string CondicionesPago { get; set; } = string.Empty;

    /// <summary>
    /// Si el valor es verdadero entonces el pedido se marca como de,
    /// temporada.
    /// </summary>
    [JsonProperty(nameof(PedidoTemporada))]
    public string PedidoTemporada { get; set; } = string.Empty;

}
