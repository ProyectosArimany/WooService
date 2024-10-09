using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WooService.Models;

/// <summary>
/// Representa un producto en un pedido de WooCommerce.
/// </summary>
[Table("WooPedido_Producto")]
public class WooPedidoProducto
{
    /// <summary>
    /// Identificador único del detalle del pedido.
    /// </summary>
    [Column(name: "Detalle_Id", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int DetalleId { get; set; }

    /// <summary>
    /// Identificador del pedido al que pertenece este producto.
    /// </summary>
    [Column(name: "Pedido_Id", Order = 1)]
    public long PedidoId { get; set; }

    /// <summary>
    /// Referencia al pedido asociado.
    /// </summary>
    [ForeignKey("PedidoId")]
    public WooPedido Pedido { get; set; } = null!;

    /// <summary>
    /// Código del producto base en el sistema AX.
    /// </summary>
    [Column("Item_Id")]
    [StringLength(100)]
    public string ItemId { get; set; } = "";

    /// <summary>
    /// Número de variante si el producto es un producto maestro en el sistema AX.
    /// </summary>
    [Column("Retail_Variant_Id")]
    [StringLength(100)]
    public string RetailVariantId { get; set; } = "";

    /// <summary>
    /// Código único del producto o variante de producto en el sistema AX.
    /// </summary>
    [Column("Woo_SKU")]
    [StringLength(150)]
    public string WooSKU { get; set; } = "";

    /// <summary>
    /// Descripción única del producto o variante de producto en el sistema AX.
    /// </summary>
    [Column("Woo_Nombre")]
    public string WooNombre { get; set; } = "";

    /// <summary>
    /// Número de unidades del producto solicitado.
    /// </summary>
    [Column("Woo_Cantidad")]
    public decimal WooCantidad { get; set; } = 0;

    /// <summary>
    /// Precio actual del producto para clientes web (Lista de webifica).
    /// </summary>
    [Column("Woo_Precio")]
    public decimal WooPrecio { get; set; } = 0;

    /// <summary>
    /// Indica si la línea fue preparada para ser registrada en el sistema AX.
    /// Valor uno si está preparada, o cero si no se ha preparado para sincronizar.
    /// </summary>
    public bool Operado { get; set; }
}
