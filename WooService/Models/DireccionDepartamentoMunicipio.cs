using System;

namespace WooService.Models;

/// <summary>
/// Listado de Municipios y departamentos, para verificar,
/// Los municipios y departamentos estan registrados en la
/// base de datos arimany-order.
/// </summary>
public class DireccionDepartamentoMunicipio
{
    /// <summary>
    /// Código de departamento en sistema AX
    /// </summary>
    public string DepartamentoId { get; set; } = "";

    /// <summary>
    /// Código del municipio en sistema AX
    /// </summary>
    public string MunicipioId { get; set; } = "";
}
