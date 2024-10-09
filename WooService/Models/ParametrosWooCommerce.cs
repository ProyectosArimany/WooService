namespace WooService.Models;

/// <summary>
/// Datos utilizados para creación de pedidos de venta en sistema AX
/// </summary>
public class ParametrosWooCommerce

{
    /// <summary>
    /// Codigo de producto utilizado para facturar cargos por envío, 
    /// para un pedido de venta, que debe ser entregado al cliente,
    /// utlizando transporte externo.
    /// </summary>
    public string CodigoDeCargosPorEnvio { get; set; } = "";
}