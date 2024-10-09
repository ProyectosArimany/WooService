namespace WooService.Models
{
    /// <summary>
    /// Lista de departamentos y municipios. Para obtener,
    /// los ID's de los departamentos y municipios en Sistema AX.
    /// Vista que consulta tablas de departamentos y municipios, 
    /// base de datos arimany-order.
    /// </summary>
    public class DepartamentosYMunicipios
    {
        /// <summary>
        /// Código de departamento en Sistema AX.
        /// </summary>
        public string Departamento { get; set; } = "";
        /// <summary>
        /// Nombre de departamento en catalogo de departamentos,
        /// en base de datos arimany-order.        /// </summary>
        public string NombreDepartamento { get; set; } = "";
        /// <summary>
        /// Código de municipio en Sistema AX.
        /// </summary>
        public string Municipio { get; set; } = "";
        /// <summary>
        /// Nombre de municipio en catalogo de municipios,
        /// en base de datos arimany-order.
        /// </summary>
        public string NombreMunicipio { get; set; } = "";
    }
}