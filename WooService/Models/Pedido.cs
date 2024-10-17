using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WooService.Models;
/// <summary>
/// Datos de pedido de venta obtenido del portal web arimany.com
/// </summary>
[Table("Pedido")]
public class Pedido
{
    /// <summary>
    /// Identificador único del pedido en el sistema interno.
    /// </summary>
    [Key, Column(name: "Pedido_Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Pedido_Id { get; set; }

    /// <summary>
    /// Identificador del pedido en WooCommerce.
    /// </summary>
    [Column("Woo_Pedido_Id")]
    public long Woo_Pedido_Id { get; set; }

    /// <summary>
    /// Relación con la entidad WooPedido.
    /// </summary>
    [ForeignKey("Woo_Pedido_Id")]
    public WooPedido WooPedido { get; set; } = null!;

    /// <summary>
    /// Identificador del pedido en el sistema AX.
    /// </summary>
    [Column("Pedido_AX_Id")]
    [StringLength(150)]
    public string PedidoAXId { get; set; } = "";

    /// <summary>
    /// Identificador de la empresa.
    /// </summary>
    [Column("Empresa_Id")]
    [StringLength(4)]
    public string EmpresaId { get; set; } = "";

    /// <summary>
    /// Identificador del cliente en sistema AX.
    /// </summary>
    [Column("Cliente_Id")]
    [StringLength(40)]
    public string ClienteId { get; set; } = "";

    /// <summary>
    /// Identificador de la dirección.
    /// </summary>
    [Column("Direccion_Id")]
    public long DireccionId { get; set; }

    /// <summary>
    /// JSON con la información del encabezado del pedido.
    /// </summary>
    [Column("Encabezado_JSON")]
    public string EncabezadoJSON { get; set; } = "";

    /// <summary>
    /// JSON con los detalles del pedido.
    /// </summary>
    [Column("Detalle_JSON")]
    public string DetalleJSON { get; set; } = "";

    /// <summary>
    /// Identificador del mensaje en AX.
    /// </summary>
    [Column("Mensaje_AX_Id")]
    public int MensajeAXId { get; set; }

    /// <summary>
    /// Mensaje del sistema AX.
    /// </summary>
    [Column("Mensaje_AX")]
    [StringLength(300)]
    public string MensajeAX { get; set; } = "";

    /// <summary>
    /// Número de factura asociada al pedido.
    /// </summary>
    [StringLength(300)]
    public string Factura { get; set; } = "";

    /// <summary>
    /// Indica si el pedido ha sido despachado.
    /// </summary>
    public bool Despachado { get; set; }

    /// <summary>
    /// Indica si el pedido debe ser operado.
    /// </summary>
    [Column("Operar")]
    public bool Operar { get; set; }

    /// <summary>
    /// Indica si el pedido ha sido operado en AX.
    /// </summary>
    [Column("Operado_AX")]
    public bool OperadoAX { get; set; }

    /// <summary>
    /// Fecha y hora en la que se generó, 
    /// el encabezado del pedido, para enviar a AX en formato json.
    /// </summary>
    [Column("Fecha_Hora_Generar_Encabezado")]
    public DateTime? FechaHoraGenerarEncabezado { get; set; }

    /// <summary>
    /// Fecha y hora en la que se generó el detalle del pedido,
    /// para enviar a AX en formato json.
    /// </summary>

    [Column("Fecha_Hora_Generar_Detalle")]
    public DateTime? FechaHoraGenerarDetalle { get; set; }

    /// <summary>
    /// Fecha y hora en la que se envió el pedido a AX.
    /// </summary>
    [Column("Fecha_Hora_Sincronizado_AX")]
    public DateTime? FechaHoraSincronizadoAX { get; set; }

    /// <summary>
    /// Fecha y hora en la que se registro el pedido en base de datos local.
    /// </summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Correlativo del pedido, diario, comenzando en 1.
    /// </summary>
    public int Correlativo { get; set; }

}


