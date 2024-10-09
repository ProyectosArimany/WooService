using System.Runtime.Serialization;

namespace WooService.Models;

/// <summary>
/// Datos utilizados para la creación de un nuevo cliente en sistema AX, 
/// para actualización de la ficha.
/// </summary>
[DataContract]
public class DireccionClienteJSON
{
    /// <summary>
    /// Código del cliente en el sistema AX
    /// </summary>
    [DataMember]
    public string CodigoCliente { get; set; } = "";

    /// <summary>
    /// Identificador único de la dirección o RecId en el sistema AX.
    /// </summary>
    [DataMember]
    public Int64 RecIdDeDireccion { get; set; } = 0;

    /// <summary>
    /// Tipo de dirección (por ejemplo, de envío, de facturación, etc.)
    /// </summary>
    [DataMember]
    public int TipoDireccion { get; set; } = 0;

    /// <summary>
    /// Descripción o nombre de la dirección
    /// </summary>
    [DataMember]
    public string Descripcion { get; set; } = "";

    /// <summary>
    /// País de la dirección
    /// </summary>
    [DataMember]
    public string Pais { get; set; } = "";

    /// <summary>
    /// Departamento o región de la dirección
    /// </summary>
    [DataMember]
    public string Depto { get; set; } = "";

    /// <summary>
    /// Municipio o ciudad de la dirección
    /// </summary>
    [DataMember]
    public string Municipio { get; set; } = "";

    /// <summary>
    /// Calle o dirección específica
    /// </summary>
    [DataMember]
    public string Calle { get; set; } = "";

    /// <summary>
    /// Indica si esta dirección es la principal (1) o no (0)
    /// </summary>
    [DataMember]
    public int EsPrimaria { get; set; } = 0;
}