using System;

namespace WooService.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WooService.Providers.AXServices;

/// <summary>
/// Almacena la dirección de envío o entrega, en base de datos local,
/// se obtienen de los pedidos de ventas,
/// registrados en el portal web arimany.com.
/// </summary>
[Table("Cliente_Direccion")]
public class ClienteDireccion
{
    /// <summary>
    /// Identifica el tipo de direccion
    /// </summary>
    [Key, Column(name: "Detalle_Id", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public LogisticsLocationRoleType TipoDireccion { get; set; }

    /// <summary>
    /// Código de cliente interno, generado para la tabla.
    /// Cliente de base de datos arimany-woocommerce.
    /// </summary>
    [Key, Column(name: "Cliente_Id", Order = 1)]
    public long ClienteId { get; set; }

    /// <summary>
    /// Relación con la entidad Cliente.
    /// </summary>
    [ForeignKey("ClienteId")]
    public Cliente Cliente { get; set; } = null!;

    /// <summary>
    /// Dirección principal del cliente.
    /// </summary>
    public string Direccion { get; set; } = "";

    /// <summary>
    /// Información adicional o complementaria de la dirección.
    /// </summary>
    [Column("Direccion_Complemento")]
    public string DireccionComplemento { get; set; } = "";

    /// <summary>
    /// Ciudad donde reside el cliente.
    /// </summary>
    [StringLength(150)]
    public string Ciudad { get; set; } = "";

    /// <summary>
    /// Identificador del departamento en el sistema AX.
    /// </summary>
    [Column("Departamento_AX_Id")]
    [StringLength(6)]
    public string DepartamentoAXId { get; set; } = "";

    /// <summary>
    /// Identificador del municipio en el sistema AX.
    /// </summary>
    [Column("Municipio_AX_Id")]
    [StringLength(6)]
    public string MunicipioAXId { get; set; } = "";

    /// <summary>
    /// Indica si esta dirección es para envío.
    /// </summary>
    public bool Envio { get; set; }
    [NotMapped]

    /// <summary>
    /// ID de dirección en sistema AX.
    /// </summary>
    public long DireccionId { get; set; }
}
