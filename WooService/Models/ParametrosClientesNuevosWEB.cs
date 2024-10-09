using System.ComponentModel.DataAnnotations.Schema;

namespace WooService.Models;
/// <summary>
/// Valores utilizados para creación o actualización de clientes en sistema AX.
/// </summary>
[Table("TRIParametrosClientesNuevosWEB")]
public class ParametrosClientesNuevosWEB
{
    /// <summary>
    /// Indica si el cliente está bloqueado.
    /// </summary>
    [Column("BLOCKED")]
    public Boolean Bloqueo { get; set; }

    /// <summary>
    /// Código de la moneda del cliente.
    /// </summary>
    [Column("CURRENCYCODE")]
    public string Moneda { get; set; } = "";

    /// <summary>
    /// Clasificación del cliente.
    /// </summary>
    [Column("CUSTCLASSIFICATIONID")]
    public string Clasificacion { get; set; } = "";

    /// <summary>
    /// Grupo del cliente.
    /// </summary>
    [Column("CUSTGROUP")]
    public string Grupo { get; set; } = "";

    /// <summary>
    /// Modo de entrega del cliente.
    /// </summary>
    [Column("DLVMODE")]
    public string ModoDeEntrega { get; set; } = "";

    /// <summary>
    /// Términos o condiciones de entrega del cliente.
    /// </summary>
    [Column("DLVTERM")]
    public string TerminoEntrega { get; set; } = "";

    /// <summary>
    /// Indica si se incluye el impuesto en las transacciones del cliente.
    /// </summary>
    [Column("INCLTAX")]
    public Boolean IncluyeImpuesto { get; set; }

    /// <summary>
    /// Almacén del cliente.
    /// </summary>
    [Column("INVENTLOCATION")]
    public string Almacen { get; set; } = "";

    /// <summary>
    /// Sitio del cliente.
    /// </summary>
    [Column("INVENTSITEID")]
    public string Sitio { get; set; } = "";

    /// <summary>
    /// Empleado responsable del cliente.
    /// </summary>
    [Column("MAINCONTACTWORKER")]
    public string EmpleadoResponsable { get; set; } = "";

    /// <summary>
    /// Indica si se debe limitar el crédito del cliente.
    /// </summary>
    [Column("MANDATORYCREDITLIMIT")]
    public bool LimitarCredito { get; set; }

    /// <summary>
    /// Conjunto de secuencias numéricas del cliente.
    /// </summary>
    [Column("NUMBERSEQUENCEGROUP")]
    public string ConjuntoDeSecuenciasNumericas { get; set; } = "";

    /// <summary>
    /// Forma de pago del cliente.
    /// </summary>
    [Column("PAYMMODE")]
    public string FormaPago { get; set; } = "";

    /// <summary>
    /// Términos de pago del cliente.
    /// </summary>
    [Column("PAYMTERMID")]
    public string TerminosDePago { get; set; } = "";

    /// <summary>
    /// Lista de precios del cliente.
    /// </summary>
    [Column("PRICEGROUP")]
    public string ListaDePrecios { get; set; } = "";

    /// <summary>
    /// Distrito de ventas del cliente.
    /// </summary>
    [Column("SALESDISTRICTID")]
    public string DistritoVentas { get; set; } = "";

    /// <summary>
    /// Segmento de ventas del cliente.
    /// </summary>
    [Column("SEGMENTID")]
    public string SegementoVentas { get; set; } = "";

    /// <summary>
    /// Grupo de impuestos del cliente.
    /// </summary>
    [Column("TAXGROUP")]
    public string GrupoImpuestos { get; set; } = "";
}