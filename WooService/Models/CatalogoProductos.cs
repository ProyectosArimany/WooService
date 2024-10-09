using System.ComponentModel.DataAnnotations.Schema;

namespace WooService.Models
{
    /// <summary>
    /// Catalogo de productos en en sistema AX.
    /// </summary>
    [Table("TRICATALOGODEARTICULOSCOMPLETO")]
    public class CatalogoProductos
    {
        /// <summary>
        /// Código unico de producto.
        /// </summary>
        [Column("ItemCode")]
        public string CodigoProducto { get; set; } = "";
        /// <summary>
        /// Nombre de producto en AX.
        /// </summary>
        [Column("DescripcionUnica")]
        public string NombreProducto { get; set; } = "";
        /// <summary>
        /// Codigo base del producto. (Código de producto NO variante).
        /// </summary>
        [Column("ItemId")]
        public decimal CodigoBase { get; set; } = 0;
        /// <summary>
        /// Numero de variante del producto <see cref="ItemId"/>.
        /// </summary>
        [Column("RetailVariantId")]
        public string Variante { get; set; } = "";
        /// <summary>
        /// Unidad de empaque de producto en AX.
        /// </summary>
        [Column("UnitId")]
        public string UnidadEmpaque { get; set; } = "";
    }
}