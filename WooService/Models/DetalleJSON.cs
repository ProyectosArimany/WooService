using System;

namespace WooService.Models;

/// <summary>
/// Detalle de productos de pedido de venta, enviado a sistema AX.
/// </summary>
public class DetalleJSON
{
    /// <summary>
    /// Código de producto (base) en sistema AX
    /// </summary>
    public string ItemId { get; set; } = "";

    /// <summary>
    /// Numero de variante del producto, 
    /// utilizado para construir, 
    /// código unico de productos
    /// </summary>
    public string VariantId { get; set; } = "";

    /// <summary>
    /// Precio de venta asignado al cliente,
    /// webifica en AX es el código de lista de precios en AX.
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Cantidad solicita de productos en pedido de venta.
    /// </summary>
    public int Cantidad { get; set; }
}

