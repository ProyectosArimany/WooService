using Microsoft.EntityFrameworkCore;
using WooService.Contexts;
using WooService.Models;
using WooService.Utils;
using WooService.Providers.AXServices;

namespace WooService.BL;

public static class ServiciosAXBL
{

    /// <summary>
    /// Consulta el catálogo de productos en sistema AX.
    /// </summary>
    /// <param name="axContext">Conexión a base de datos de DynamicsAX</param>
    /// <param name="producto">Producto a consultar</param>
    /// <returns>Datos del artículo buscado</returns>
    public static async Task<(CatalogoProductos?, string)> ObtenerCatalogoProductos(AXContext axContext, String producto)
    {
        String mensajeError = "";
        try
        {
            var articulo = await axContext.CatalogoProductos.FirstOrDefaultAsync(cp => cp.CodigoProducto == producto);
            if (articulo != null) return (articulo, mensajeError);
            mensajeError = $"No se encontró el producto {producto}.";
        }
        catch (Exception ex)
        {
            mensajeError = $"Error al consultar producto {producto}." + Environment.NewLine + Global.GetExceptionError(ex);
        }
        return (null, mensajeError);
    }

    /// <summary>
    /// Consulta un cliente en sistema AX.
    /// </summary>
    /// <param name="axContext">Conexión a base de datos de sistema AX.</param>
    /// <param name="cliente">Código del cliene en AX, a consultar.</param>
    /// <returns>El cliente encontrado, o null si ocurre algún error.</returns>
    public static async Task<(ClienteAX?, string)> ObtenerClienteAX(AXContext axContext, String cliente)
    {
        String mensajeError = "";
        try
        {
            var clienteAX = await axContext.ClientesAX.FirstOrDefaultAsync(c => c.Cliente == cliente);
            if (clienteAX != null) return (clienteAX, mensajeError);
            mensajeError = $"No se encontró el cliente {cliente}.";
        }
        catch (Exception ex)
        {
            mensajeError = $"Error al obtener el cliente {cliente}." + Environment.NewLine + Global.GetExceptionError(ex);
        }
        return (null, mensajeError);
    }

    /// <summary>
    /// Obtiene los datos que deben utilizarce por defecto cuando,
    /// se crea o actualiza un cliente en Sistema AX.
    /// </summary>
    /// <param name="context">Conexión a base de datos</param>
    /// <returns>Registro con parametros para clientes WEB nuevos.</returns>
    public static async Task<(ParametrosClientesNuevosWEB?, string)> ObtenerParametrosClientesNuevosWEB(AXContext context)
    {
        try
        {
            ParametrosClientesNuevosWEB? parametros = await context.ParametrosClientesNuevosWEB.FirstOrDefaultAsync();
            return (parametros, "");
        }
        catch (Exception ex)
        {
            return (null, "No pudieron obtener parametros de clientes en sistema AX." +
            Environment.NewLine + Global.GetExceptionError(ex));
        }
    }



}