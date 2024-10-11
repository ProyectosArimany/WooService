
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WooService.Models;

[Table("Woo_Pedido")]
public class WooPedido
{
    /// <summary>
    /// Correlativo de pedidos generado por este servicio. De uso interno.
    /// Representa el número interno del pedido en esta tabla.
    /// </summary>
    [Key, Column(name: "Pedido_Id"), DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long PedidoId { get; set; } = 0;

    /// <summary>
    /// El número de pedido asignado en el portal de arimany.com
    /// </summary>
    [Column("Woo_Id")]
    public int WooId { get; set; } = 0;

    /// <summary>
    /// El código de cliente generado en WooCommerce
    /// </summary>
    [Column("Woo_Cliente_Id")]
    public int WooClienteId { get; set; } = 0;

    /// <summary>
    /// Código de cliente asignado por sistema AX
    /// </summary>
    [Column("Cliente_Id")]
    public long ClienteId { get; set; } = 0;

    /// <summary>
    /// Relación con la entidad Cliente
    /// </summary>
    [ForeignKey("ClienteId")]
    public Cliente Cliente { get; set; } = null!;

    /// <summary>
    /// Identificador de pedido WooCommerce
    /// </summary>
    [Column("Woo_Key")]
    [StringLength(150)]
    public string WooKey { get; set; } = "";

    /// <summary>
    /// El estado actual del pedido en WooCommerce (arimany.com)
    /// </summary>
    [Column("Woo_Estado")]
    [StringLength(100)]
    public string WooEstado { get; set; } = "";

    /// <summary>
    /// Cantidad de productos en el pedido
    /// </summary>
    [Column("Woo_Cantidad_Producto")]
    public int WooCantidadProducto { get; set; }

    /// <summary>
    /// Total del pedido
    /// </summary>
    [Column("Woo_Total")]
    public decimal WooTotal { get; set; }

    /// <summary>
    /// Título del método de pago
    /// </summary>
    [Column("Woo_Titulo_Metodo_Pago")]
    [StringLength(150)]
    public string WooTituloMetodoPago { get; set; } = "";

    /// <summary>
    /// Método de pago utilizado
    /// </summary>
    [Column("Woo_Metodo_Pago")]
    [StringLength(150)]
    public string WooMetodoPago { get; set; } = "";

    /// <summary>
    /// JSON del pedido obtenido del portal web arimany.com
    /// </summary>
    [Column("Woo_JSON")]
    public string WooJSON { get; set; } = "";

    /// <summary>
    /// Indica si el pedido ya fue procesado para sincronización en sistema AX
    /// </summary>
    public bool Operado { get; set; }

    /// <summary>
    /// Indica si el detalle del producto ya está preparado para sincronizar en sistema AX
    /// </summary>
    [Column("Sincronizar_Producto")]
    public bool SincronizarProducto { get; set; }

    /// <summary>
    /// Indica si el encabezado del pedido está preparado para sincronizar con sistema AX
    /// </summary>
    [Column("Generar_Encabezado_JSON")]
    public bool GenerarEncabezadoJSON { get; set; }

    /// <summary>
    /// Indica si el detalle del pedido ya está listo para sincronizar con AX
    /// </summary>
    [Column("Generar_Detalle_JSON")]
    public bool GenerarDetalleJSON { get; set; }

    /// <summary>
    /// Fecha de registro del pedido en el portal WEB
    /// </summary>
    [Column("Woo_Fecha")]
    public DateTime WooFecha { get; set; }

    /// <summary>
    /// Fecha de procesamiento del pedido
    /// </summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Posición (orden) en el listado de pedidos obtenidos del portal web
    /// </summary>
    public int Correlativo { get; set; }

    /// <summary>
    /// Lista de productos asociados al pedido
    /// </summary>
    public List<WooPedidoProducto> Productos { get; set; } = null!;
}
