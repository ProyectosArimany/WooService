namespace WooService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



/// <summary>
/// Clase Clientes: Se utiliza para almacenar los datos de los clientes,
/// asociados a los pedidos registrados en portal web arimany.com (woocommerce).
/// </summary>
[Table("Cliente")]
public class Cliente
{
    /// <summary>
    /// Código de cliente generado desde este servicio. Para cada cliente,
    /// obtenido del portal web arimany.com.  Este utiliza la fecha + un 
    /// correlativo interno de 3 digitos.
    /// <example>
    /// Si la fecha es 21 de Abril del 2024; el correlativo para un cliente 
    /// seria: 20240421001.  El campo Correlativo contiene el correlativo 1,
    /// se concatena al final de la fecha utilizando 3 digitos..
    /// </example>
    /// </summary>
    [Key, Column(name: "Cliente_Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long ClienteId { get; set; } = 0;

    /// <summary>
    /// Código de cliente asignado en el portal de arimany.com
    /// </summary>
    [Column("Cliente_Woo_Id")]
    public int ClienteWooId { get; set; } = 0;

    /// <summary>
    /// Codigó de cliente asignado por sistema AX despues de sincronizar.
    /// </summary>
    [Column("Cliente_AX_Id")]
    [StringLength(50)]
    public string ClienteAXId { get; set; } = "";


    /// <summary>
    /// Numero de identificación tributaria ante SAT
    /// </summary>
    [StringLength(20)]
    public string Nit { get; set; } = "";

    /// <summary>
    /// Nombres del cliente, para registro de pedido y facturación
    /// </summary>
    [StringLength(300)]
    public string Nombres { get; set; } = "";

    /// <summary>
    /// Apellidos del cliente, para registro de pedido y facturación
    /// </summary>
    [StringLength(300)]
    public string Apellidos { get; set; } = "";

    /// <summary>
    /// teléfono del cliene.
    /// </summary>
    [StringLength(50)]
    public string Telefono { get; set; } = "";

    /// <summary>
    /// Correo eléctronico, del cliente.
    /// </summary>
    [Column("Correo_Electronico")]
    public string CorreoElectronico { get; set; } = "";

    /// <summary>
    /// Fecha de registro del pedido en woo
    /// </summary>
    public DateTime Fecha { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Numero de cliente generado en un día, Numero de cliente
    /// generado internamente iniciando en el cliente 1.
    /// </summary>
    public int Correlativo { get; set; } = 0;

    /// <summary>
    /// Listado de direcciones registradas por los clientes.
    /// </summary>
    public List<ClienteDireccion> Direcciones { get; set; } = null!;

}
