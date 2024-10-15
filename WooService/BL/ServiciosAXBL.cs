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
    public static async Task<(CatalogoProductos?, string, string, string)> ObtenerCatalogoProductos(AXContext axContext, String producto)
    {
        String Error = "", Causa = "", Solucion = "";
        CatalogoProductos? articulo = null;

        try
        {
            articulo = await axContext.CatalogoProductos.FirstOrDefaultAsync(cp => cp.CodigoProducto == producto);
            if (articulo is null)
            {
                Causa = $"No se encontró el producto {producto}. en el catálogo de productos.";
                Solucion = "Verifique que el producto exista en el catálogo de productos de Dynamics AX." + Environment.NewLine +
                           "Verifire que sku en el pedido de arimanycom, sea correcto.";
            }

        }
        catch (Exception ex)
        {
            Error = "Error al consultar producto en sistema AX.";
            Causa = $"La base de datos de Dynamics AX emitio un error. Producto {producto}." + Environment.NewLine + Global.GetExceptionError(ex);
            Solucion = "Verifique que la base de datos de Dynamics AX este disponible." + Environment.NewLine +
                        "O llame al administrador del sistema";
        }
        return (articulo, Error, Causa, Solucion);
    }

    /// <summary>
    /// Consulta un cliente en sistema AX.
    /// </summary>
    /// <param name="axContext">Conexión a base de datos de sistema AX.</param>
    /// <param name="cliente">Código del cliene en AX, a consultar.</param>
    /// <returns>El cliente encontrado, o null si ocurre algún error.</returns>
    public static async Task<(ClienteAX?, string, string, string)> ObtenerClienteAX(AXContext axContext, String cliente)
    {
        String Error = "", Causa = "", Solucion = "";
        ClienteAX? clienteAX = null;

        try
        {
            clienteAX = await axContext.ClientesAX.FirstOrDefaultAsync(c => c.Cliente == cliente);
            if (clienteAX is null)
            {
                Causa = "No  existe el cliente, en el catálogo de clientes del sistema AX.";
                Solucion = "Verifique que el cliente exista en el catálogo de productos de Dynamics AX." + Environment.NewLine +
                           "Verifire que sku en el pedido de arimanycom, sea correcto.";
                return (clienteAX, Error, Causa, Solucion);
            }
        }
        catch (Exception ex)
        {
            Error = "Error al consultar cliente en sistema AX.";
            Causa = $"La base de datos emitió errores al consultar el cliente {cliente}." + Environment.NewLine + Global.GetExceptionError(ex);
            Solucion = "Verifique que la base de datos de Dynamics AX este disponible." + Environment.NewLine +
                       "Que este creada la vista ClientesAX en la base de datos de Dynamics AX." + Environment.NewLine +
                       "Y llame al administrador del sistema.";
        }
        return (null, Error, Causa, Solucion);
    }

    /// <summary>
    /// Obtiene los datos que deben utilizarce por defecto cuando,
    /// se crea o actualiza un cliente en Sistema AX.
    /// </summary>
    /// <param name="context">Conexión a base de datos</param>
    /// <returns>Registro con parametros para clientes WEB nuevos.</returns>
    public static async Task<(ParametrosClientesNuevosWEB?, string, string, string)> ObtenerParametrosClientesNuevosWEB(AXContext context)
    {
        String Error = "", Causa = "", Solucion = "";
        ParametrosClientesNuevosWEB? parametros = null;
        try
        {
            parametros = await context.ParametrosClientesNuevosWEB.FirstOrDefaultAsync();
            if (parametros is null)
            {
                Error = "No se encontraron parametros para clientes en sistema AX.";
                Causa = "No se obtuvo ningun registro de parametros," + Environment.NewLine +
                        "para crear, clientes nuevos, en sistema AX.";
                Solucion = "Verifique que exista un registro en la tabla TRIParametrosClientesNuevosWEB." + Environment.NewLine +
                           "Llame al administrador para que revise la configuración del sistema.";
            }
        }
        catch (Exception ex)
        {
            Error = "Error al consultar parametros de clientes en sistema AX.";
            Causa = "La base de datos de Dynamics AX emitió un error al consultar parametros de clientes." + Environment.NewLine +
                    Global.GetExceptionError(ex);
            Solucion = "Verifique que la base de datos de Dynamics AX este disponible." + Environment.NewLine +
                       "Verifique que exista la tabla TRIParametrosClientesNuevosWEB." + Environment.NewLine +
                       "Y llame al administrador del sistema.";
        }
        return (parametros, Error, Causa, Solucion);
    }



}