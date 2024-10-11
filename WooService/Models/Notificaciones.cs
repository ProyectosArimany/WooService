using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WooService.Models;

/// <summary>
/// Notificaciones emitidas por el servicio WooService.
/// </summary>
/// 
[Table("Notificaciones")]
public class Notificaciones
{
    /// <summary>
    /// Identificador único de la notificación.
    /// </summary>
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long NotificacionId { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    /// <summary>
    /// Identificador del pedido en WooCommerce.
    /// </summary>
    public long WooPedidoId { get; set; }

    /// <summary>
    /// Tipo notifiación. (Error, Warning)
    public string TipoNotificacion { get; set; } = "";

    /// <summary>
    /// Mensaje de la notificación.
    /// </summary>
    public string Notificacion { get; set; } = "";
}