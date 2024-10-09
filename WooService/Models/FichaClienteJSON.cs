using System.Runtime.Serialization;

namespace WooService.Models;

/// <summary>
/// Ficha de cliente, utilizada para la creación de un nuevo cliente en sistema AX.
/// </summary>
[DataContract]
public class FichaClienteJSON
{
    /// <summary>
    /// Código de cliente en sistema AX, o vacío para crear un nuevo cliente.
    /// </summary>
    [DataMember]
    public string CodigoCliente { get; set; } = "";

    /// <summary>
    /// Condiciones de entrega para el cliente
    /// </summary>
    [DataMember]
    public string CondicionEntrega { get; set; } = "";

    /// <summary>
    /// Modo o condiciones de entrega por el cliente, se obtiene,
    /// de la tabla de parámetros para wooservice en sistema AX.
    /// </summary>
    [DataMember]
    public string ModoEntrega { get; set; } = "";

    /// <summary>
    /// Forma de pago acordada con el cliente, se obtiene,
    /// de la tabla de parámetros para wooservice en sistema AX.
    /// </summary>
    [DataMember]
    public string FormaPago { get; set; } = "";

    /// <summary>
    /// Número de Identificación Tributaria del cliente
    /// </summary>
    [DataMember]
    public string NIT { get; set; } = "";

    /// <summary>
    /// Tipo de cliente (por ejemplo, individual, corporativo, etc.)
    /// </summary>
    [DataMember]
    public int TipoCliente { get; set; } = 0;

    /// <summary>
    /// Nombre completo del cliente o razon social
    /// </summary>
    [DataMember]
    public string Nombre { get; set; } = "";

    /// <summary>
    /// Documento de identificación del cliente
    /// </summary>
    [DataMember]
    public string DoctoIdentificacion { get; set; } = "";

    /// <summary>
    /// País de residencia del cliente
    /// </summary>
    [DataMember]
    public string Pais { get; set; } = "";

    /// <summary>
    /// Departamento o región de residencia del cliente
    /// </summary>
    [DataMember]
    public string Depto { get; set; } = "";
}