using Microsoft.EntityFrameworkCore;
using WooService.Contexts;
using WooService.Utils;
using WooService.Models;
using System.Text.Json;
using WooCommerceNET.WooCommerce.v3;
using WooService.Providers.AXServices;
using WooService.Providers;
using Microsoft.IdentityModel.Tokens;


namespace WooService.BL;

/// <summary>
/// Lógica de negocio para registro de pedidos del portal web,
/// en base de datos woo-commerce.
/// </summary>
public static class WooServiceBL
{
    /// <summary>
    /// Genera un ID de pedido local usando la fecha (formato yyyyMMdd),y
    /// un correlativo de 3 digitos.
    /// </summary>
    /// <returns>Tuple con el ID del pedido generado, o Cero y un mensaje de error (si lo hubiese).</returns>
    private static async Task<(long, int, string, string, string)> GenerateLocalOrderId(WooCommerceContext _context)
    {
        string Error = String.Empty, Causa = String.Empty, Solucion = String.Empty;
        try
        {
            var data = await _context.WooPedidos
                .Where(c => c.Fecha.Date == DateTime.Now.Date)
                .OrderByDescending(c => c.PedidoId)
                .FirstOrDefaultAsync();
            if (data == null)
            {
                string strCorrel = DateTime.Now.ToString("yyyyMMdd") + "001";
                if (long.TryParse(strCorrel, out long correl))
                {
                    return (correl, 1, Error, Causa, Solucion);
                }
                else
                {
                    Error = "Error al generar ID de pedido local";
                    Causa = $"Valor que produjo el error de conversión {strCorrel}";
                    Solucion = $"Verificar si el valor de correlativo no excede {int.Max}.";
                    return (0, 0, Error, Causa, Solucion);
                }
            }
            else
            {
                long correl = data.PedidoId + 1;
                string strCorrel = correl.ToString();
                if (int.TryParse(strCorrel.AsSpan(strCorrel.Length - 3, 3), out int intcorrelativo))
                {
                    return (correl, intcorrelativo, Error, Causa, Solucion);
                }
                else
                {
                    Error = "Error al generar ID de pedido local";
                    Causa = $"Valor que produjo el error de conversión {strCorrel}";
                    Solucion = $"Verificar si el valor de correlativo no excede {int.Max}.";
                    return (0, 0, Error, Causa, Solucion);
                }
            }
        }
        catch (Exception ex)
        {
            Error = "Error al generar ID de pedido local";
            Causa = $"Error al recuperar datos para generar correlativo de ventas tabla WooPedidos.\n" +
                     Global.GetExceptionError(ex);
            Solucion = $"Verificar que la tabla y base de datos, para registro de notificaciones este disponible.";
            return (0, 0, Error, Causa, Solucion);
        }
    }


    /// <summary>
    /// Genera un ID de cliente local usando la fecha (formato yyyyMMdd),y
    /// un correlativo de 3 digitos.
    /// </summary>
    /// <returns>Tuple con el ID del cliente generado, o Cero y un mensaje de error (si lo hubiese).</returns>
    private static async Task<(long, string, string, string)> GenerateLocalClienteId(WooCommerceContext _context)
    {
        string Error = String.Empty, Causa = String.Empty, Solucion = String.Empty;
        try
        {
            var data = await _context.Clientes
                .Where(c => c.Fecha.Date == DateTime.Now.Date)
                .OrderByDescending(c => c.ClienteId)
                .FirstOrDefaultAsync();
            if (data == null)
            {
                string strCorrel = DateTime.Now.ToString("yyyyMMdd") + "001";
                if (long.TryParse(strCorrel, out long correl))
                {
                    return (correl, Error, Causa, Solucion);
                }
                else
                {
                    Error = "Error al generar ID de Cliente local.";
                    Causa = $"Valor que produjo el error de conversión {strCorrel}.";
                    Solucion = $"Verificar si el valor de correlativo no excede {int.Max}.";
                    return (0, Error, Causa, Solucion);
                }
            }
            else
            {
                return (data.ClienteId + 1, Error, Causa, Solucion);
            }
        }
        catch (Exception ex)
        {
            Error = "Error al generar ID de Cliente local.";
            Causa = $"Error al leer datos de cliente para genera correlativo." + Environment.NewLine +
                     Global.GetExceptionError(ex);
            Solucion = $"Verificar que la tabla Clientes este disponible,\n y que la base de datos este también disponible.";
            return (0, Error, Causa, Solucion);
        }
    }


    /// <summary>
    /// Busca un departamento utiliza el nombre del municipio,
    /// para la busqueda.
    /// </summary>
    /// <param name="logger">Se utiliza para registro de eventos en el sistema.</param>
    /// <param name="_context">Conexión a base de datos arimany-woocommerce</param>
    /// <param name="municipio">Filtar utilzando este campo</param>
    /// <returns>El municipio encontrado o null si ocurre algun error</returns>
    private static async Task<(DepartamentosYMunicipios?, string, string, string)> BuscarDepartamentoPorMunicipio(WooCommerceContext _context, string municipio)
    {
        string Error = String.Empty, Causa = String.Empty, Solucion = String.Empty;
        try
        {
            var data = await _context.DepartamentosYMunicipios.FirstOrDefaultAsync(c => c.Municipio == municipio);
            if (data != null)
            {
                return (data, Error, Causa, Solucion);
            }
            else
            {
                Error = $"No se encontró el Departamento/Municipio. {municipio}";
                Causa = "El municipio no ha sido registrado o el valor es incorrecto.";
                Solucion = "Verificar que el municipio este registrado en la tabla DepartamentosYMunicipios." +
                           Environment.NewLine +
                           "Verificar el dato de departamento/municipio en" + Environment.NewLine +
                           "el pedido del portal web.";
                return (null, Error, Causa, Solucion);
            }
        }
        catch (Exception ex)
        {
            Error = "Error al recuperar datos del Departamento/Municipio.";
            Causa = "Se produjo un error en la lectura de la tabla DepartamentosYMunicipios.";
            Solucion = "1. Verificar que la tabla DepartamentosYMunicipios este disponible." + Environment.NewLine + Global.GetExceptionError(ex) +
                       Environment.NewLine + "2. Verificar que la base de datos este disponible." + Environment.NewLine +
                       "3. Verificar la conexión a la base de datos.";
            return (null, Error, Causa, Solucion);
        }
    }

    /// <summary>
    /// Busca un cliente por el ID asignado en el portal web (woocommerce) o NIT.
    /// </summary>
    /// <param name="logger">Se utiliza para registro de eventos en el sistema.</param>
    /// <param name="_context">Conexión a base de datos arimany-woocommerce</param>
    /// <param name="clienteId">Id del cliente en portal web (CustumerID)</param>
    /// <param name="NIT">NIT del cliente</param>
    /// <returns>El cliente encontrado o null si ocurre un error</returns>
    private static async Task<(Cliente?, string, string, string)> BuscarClientePorId(WooCommerceContext _context, long clienteId, string NIT)
    {
        string Error = String.Empty, Causa = String.Empty, Solucion = String.Empty;
        if (clienteId == 0 && Global.StrIsBlank(NIT))
        {
            Error = "Debe proporcionar un ID de cliente o NIT.";
            Causa = "No se proporcionó un ID de cliente o NIT.";
            Solucion = "Proporcione un ID de cliente o NIT para buscar el cliente.";
            return (null, Error, Causa, Solucion);
        }
        try
        {
            // Buscar cliente por ID. de lo contrario buscar por NIT.
            Cliente? cliente = null;

            if (clienteId != 0)
            {
                cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ClienteId == clienteId);
            }

            if (cliente is null && !Global.StrIsBlank(NIT))
            {
                cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ClienteId == clienteId || c.Nit == NIT);
            }

            if (cliente != null)
            {
                Error = $"";
                Causa = "No se encontró el cliente. ID: {clienteId}, NIT: {NIT}." + Environment.NewLine +
                        "El cliente no ha sido registrado o el valor es incorrecto.";
                Solucion = "1. Verificar que el cliente este registrado en la tabla Clientes." + Environment.NewLine +
                           "2. Verificar si el cliente tiene un ID asignado en el portal web." + Environment.NewLine;
            }

            return (cliente, Error, Causa, Solucion);
        }
        catch (Exception ex)
        {
            Error = $"Error al consultar cliente. ID: {clienteId}, NIT: {NIT}";
            Causa = "Error en la lectura de la tabla Clientes." + Environment.NewLine +
                    Global.GetExceptionError(ex);
            Solucion = "Verificar que la tabla Clientes este disponible " + Environment.NewLine +
                       "Que no este bloqueada. Que tenga permisos de lectura." + Environment.NewLine +
                       "2. Verificar que la base de datos este disponible." + Environment.NewLine;

            return (null, Error, Causa, Solucion);
        }
    }

    /// <summary>
    /// Busca las direcciones de un cliente.
    /// </summary>
    /// <param name="logger">Se utiliza para registro de eventos en el sistema.</param>
    /// <param name="_context">Conexión a base de datos arimany-woocommerce</param>
    /// <param name="clienteId">Id del cliente en portal web (CustumerID)</param>
    /// <returns>Listado de direcciones o null si ocurre un error.</returns>
    private static async Task<(List<ClienteDireccion>, string, string, string)> BuscarDireccionesCliente(WooCommerceContext _context, long clienteId)
    {
        string Error = String.Empty, Causa = String.Empty, Solucion = String.Empty;
        List<ClienteDireccion> data = [];
        try
        {
            data = await _context.ClienteDirecciones.Where(c => c.ClienteId == clienteId).ToListAsync();
            if (data.Count != 0)
            {
                Error = "No se encontrarion direcciones para el cliente.";
                Causa = "No existe un registro de direcciones para el cliente.";
                Solucion = "Verifique que el pedido de venta de arimany.com," + Environment.NewLine +
                            "contenga la dirección de envío y facturación del cliente.";
            }

        }
        catch (Exception ex)
        {
            Error = "Error al buscar direcciones del cliente.";
            Causa = "Error en la lectura de la tabla Cliente_Direccion." + Environment.NewLine +
                    Global.GetExceptionError(ex);
            Solucion = "Verificar que la tabla Cliente_Direccion y la base de datos arimany-woocommerce, esten disponibles." + Environment.NewLine +
                       "Verificar que la conexión a la base de datos este disponible." + Environment.NewLine +
                       "Verificar que la tabla Cliente_Direccion no este bloqueada." + Environment.NewLine +
                       "Verificar que la tabla Cliente_Direccion tenga permisos de lectura." + Environment.NewLine +
                       "LLamar a soporte técnico.";
        }
        return (data, Error, Causa, Solucion);
    }



    /// <summary>
    /// Elimina las direcciones asociadas a un cliente.
    /// </summary>
    /// <param name="_context">Conexión a base de datos local.</param>
    /// <param name="direcciones">Lista de direcciones a quitar del cliente.</param>
    /// <returns>True si la operación no produjo errores, de lo contratrio False.</returns>
    private static (bool, string, string, string) BorrarDireccionesDelCliente(WooCommerceContext _context, List<ClienteDireccion> direcciones)
    {
        if (direcciones.Count == 0) return (true, "", "", "");

        string Error = String.Empty, Causa = String.Empty, Solucion = String.Empty;
        bool ok = false;
        try
        {
            _context.ClienteDirecciones.RemoveRange(direcciones);
            ok = true;
        }
        catch (Exception ex)
        {
            Error = "Error al borrar direcciones del cliente.";
            Causa = $"La base de datos emitio un error al intentar borrar direcciones del clientes. {direcciones[0].ClienteId}" + Environment.NewLine +
                    Global.GetExceptionError(ex);
            Solucion = "Verificar que la tabla Cliente_Direccion este disponible. " + Environment.NewLine +
                       "(Ver la conexión, que exista la tabla y que la base de datos este disponible)";
        }
        return (ok, Error, Causa, Solucion);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger">Se utiliza para registro de eventos en el sistema.</param>}
    /// <param name="_context">Conexión a base de datos arimany-woocommerce.</param>
    /// <param name="wooPedido">Datos del pedido obtenidos del portal web.</param>
    /// <returns>El registro creado o actualizado. Null si ocurre un error.</returns>
    public static async Task<(WooPedido?, string, string, string)> CreateOrUpdateWooPedido(WooCommerceContext _context,
                                                                                    AXContext aXContext,
                                                                                    Order pedido,
                                                                                    ParametrosClientesNuevosWEB ParamsClientes,
                                                                                    ParametrosWooCommerce? ParametrosLineasPedido,
                                                                                    AppSettings appsets)
    {
        String Error = String.Empty, Causa = String.Empty, Solucion = String.Empty;
        WooPedido? wooPedido = null;


        var strategy = _context.Database.CreateExecutionStrategy();
        bool changeState = false;

        await strategy.ExecuteAsync(async () =>
            {

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    int pedidoWooId = ((int?)pedido.id) ?? 0;
                    var data = await _context.WooPedidos.FirstOrDefaultAsync(c => c.WooId == pedidoWooId);

                    if (data != null)
                    {
                        Causa = $"El pedido {pedidoWooId} ya existe en la base de datos.";
                        Solucion = "No se puede crear un pedido que ya existe.";
                        await transaction.RollbackAsync();
                        return;
                    }
                    changeState = true;
                    /// Pedido Nuevo, procesar y registrar en base de datos local, los datos del cliente,
                    /// que vienen el pedido.

                    (Cliente? cliente, Error, Causa, Solucion) = await CreateOrUpdateWooCliente(_context, aXContext, appsets, pedido, ParamsClientes);

                    /// Si ocurre un error al crear o actualizar el cliente, entonces retornar el error.
                    if (!Global.StrIsBlank(Error))
                    {
                        await transaction.RollbackAsync();
                        return;
                    }

                    /// Generar ID de pedido local.
                    (long correl, int intcorrelativo, Error, Causa, Solucion) = await GenerateLocalOrderId(_context);
                    if (correl == 0)
                    {
                        await transaction.RollbackAsync();
                        return;
                    }

                    wooPedido!.PedidoId = 0;
                    wooPedido.WooId = pedidoWooId;
                    wooPedido.WooClienteId = ((int?)pedido.customer_id) ?? 0;
                    wooPedido.ClienteId = cliente!.ClienteId;
                    wooPedido.WooKey = pedido.order_key;
                    wooPedido.WooEstado = pedido.status;
                    wooPedido.WooCantidadProducto = pedido.line_items.Count;
                    wooPedido.WooTotal = pedido.total ?? 0;
                    wooPedido.WooTituloMetodoPago = pedido.payment_method_title;
                    wooPedido.WooMetodoPago = pedido.payment_method;
                    wooPedido.WooJSON = JsonSerializer.Serialize(pedido);
                    wooPedido.Operado = false;
                    wooPedido.SincronizarProducto = false;
                    wooPedido.GenerarEncabezadoJSON = false;
                    wooPedido.GenerarDetalleJSON = false;
                    wooPedido.WooFecha = pedido.date_created!.Value;
                    wooPedido.Fecha = DateTime.Now;
                    wooPedido.Correlativo = intcorrelativo;
                    wooPedido.PedidoId = correl;
                    wooPedido.Cliente = cliente;


                    // Agregar producto por cargo de envio.
                    decimal CargoPorEnvio = pedido.shipping_total ?? 0;

                    (bool ok, Error, Causa, Solucion) = await AgregarProductosWoo(aXContext, pedido, wooPedido, ParametrosLineasPedido!.CodigoDeCargosPorEnvio, CargoPorEnvio);

                    if (!ok)
                    {
                        await transaction.RollbackAsync();
                        wooPedido = null;
                        return;
                    }
                    _context.WooPedidos.Add(wooPedido);
                    await _context.SaveChangesAsync();

                    /// Crear registro de pedido en base de datos local.
                    (Pedido? pedidoLocal, Error, Causa, Solucion) = await CreateOrUpdatePedido(_context, wooPedido, appsets.Pais);
                    if (Global.StrIsBlank(Error))
                    {
                        await transaction.RollbackAsync();
                        wooPedido = null;
                        return;
                    }

                    // Registrar pedido en Sistema AX.
                    (ok, Error, Causa, Solucion) = await CrearPedidoEnAX(_context, appsets, ParamsClientes, wooPedido, pedidoLocal!);
                    if (!ok)
                    {
                        await transaction.RollbackAsync();
                        wooPedido = null;
                        return;
                    }

                    await transaction.CommitAsync();

                    return;
                }
                catch (Exception ex)
                {
                    Error = $"No se actualizó o creó pedido.\n WOOID: {pedido.id}, cliente: {pedido.billing.first_name} {pedido.billing.last_name}";
                    Causa = "Se produjo un error al intentar actualizar o crear pedido." + Environment.NewLine + Global.GetExceptionError(ex);
                    Solucion = "LLamar a soporte IT.";
                    wooPedido = null;
                    await transaction.RollbackAsync();
                    return;
                }
            });
        /// Actualizar el estado del pedido en woocommerce (arimany.com) a procesando.
        if (changeState)
        {

            var wooprovider = new WooProvider(appsets.WooURL, appsets.WooConsumerKey, appsets.WooConsumerSecret);
            if (!Global.StrIsBlank(wooprovider.GetMsgError))
            {
                Error = wooprovider.GetMsgError;
                Causa = "Error al actualizar el estado del pedido en el portal web.";
                Solucion = "Verificar la conexión a internet." + Environment.NewLine +
                           "Verificar que el portal web este disponible." + Environment.NewLine +
                           "Verificar que el servicio de woocommerce este disponible.";
            }
            else
            {
                bool ok = await wooprovider.ActualizarEstadoPedido(pedido.id ?? 0, "processing");
                if (!ok)
                {
                    Error = "Error al intentar cambiar el estado del pedido en el portal web.";
                    Causa = "Se produjo un erro durante la actualización." + wooprovider.GetMsgError;
                    Solucion = "Verificar la conexión a internet." + Environment.NewLine +
                               "Verificar que el portal web este disponible." + Environment.NewLine +
                               "Verificar que el servicio de woocommerce este disponible." + Environment.NewLine +
                               "Verificar el estado del peido en el portal web." +
                               "Llamar a soporte IT.";

                }
            }
        }

        return (wooPedido, Error, Causa, Solucion);
    }

    /// <summary>
    /// Agrega los productos de un pedido al registro de la base de datos arimany-woocommerce.
    /// </summary>
    /// <param name="aXContext">Conexión a base de datos de AX</param>
    /// <param name="pedido"></param>
    /// <param name="wooPedido"></param>
    /// <returns></returns>
    private static async Task<(Boolean, string, string, string)> AgregarProductosWoo(AXContext aXContext,
                                                                     Order pedido,
                                                                     WooPedido wooPedido,
                                                                     String CodigoPorCargoEnvio,
                                                                     decimal CargoPorEnvio)
    {
        string Error = "", Causa = "", Solucion = "";
        string msgError = $"WooPedido: {pedido.id}, Cliente: {pedido.billing.first_name} {pedido.billing.last_name}." + Environment.NewLine;
        int i = 0;
        List<WooPedidoProducto> productos = [];

        // Agregar productos al pedido.
        foreach (var item in pedido.line_items)
        {
            // saltar lineas sin SKU.
            if (Global.StrIsBlank(item.sku))
            {
                --wooPedido.WooCantidadProducto;
                continue;
            }

            // Verificar si existe el producto
            (CatalogoProductos? producto, String msg, Causa, Solucion) = await ServiciosAXBL.ObtenerCatalogoProductos(aXContext, item.sku);
            if (producto is null)
            {
                Error += msgError + msg + Environment.NewLine;
                Causa += Causa + Environment.NewLine;
                Solucion += Solucion + Environment.NewLine;
                continue;
            }

            decimal precio = item.price ?? 0;
            decimal cantidad = item.quantity ?? 0;
            decimal total = item.total ?? precio * cantidad;

            if (precio == 0 || cantidad == 0)
            {
                Error += msgError + Environment.NewLine + $"Falta el precio o la cantidad, Producto: {item.sku}." + Environment.NewLine;
                Causa += Causa + Environment.NewLine + "Datos requeridos no proporcionados." + Environment.NewLine;
                Solucion += Solucion + Environment.NewLine + "Asigne precio y la cantidad en el pedido del portal web";
                continue;
            }

            WooPedidoProducto wooPedidoNuevo = new()
            {
                DetalleId = i++,
                PedidoId = wooPedido.PedidoId,
                ItemId = producto.CodigoProducto,
                RetailVariantId = producto.Variante,
                WooSKU = item.sku,
                WooNombre = item.name,
                WooCantidad = item.quantity ?? 0,
                WooPrecio = precio,
                Operado = false
            };
            if (Global.StrIsBlank(Error))
            {
                productos.Add(wooPedidoNuevo);
            }
        }

        if (CargoPorEnvio != 0 && !Global.StrIsBlank(CodigoPorCargoEnvio))
            productos.Add(new WooPedidoProducto() { WooSKU = CodigoPorCargoEnvio, WooNombre = "Cargo por envios", WooCantidad = 1, WooPrecio = CargoPorEnvio });

        wooPedido.Productos = productos;
        return (!Global.StrIsBlank(Error), Error, Causa, Solucion);
    }


    /// <summary>
    /// Crea o actualiza un cliente en la base de datos arimany-woocommerce.
    /// </summary>
    /// <param name="logger">Se utiliza para registro de eventos en el sistema.</param>
    /// <param name="_context">Conexión a base de datos arimany-woocommerce</param>
    /// <param name="wooCliente">Datos del cliente obtenidos del portal web, 
    /// para crear o actualizar un registro de cliente en base de datos local.</param>
    /// <returns>El registro del cliente, creado o actualizado. Null si ocurre un error</returns>
    private static async Task<(Cliente?, string, string, string)> CreateOrUpdateWooCliente(WooCommerceContext _context, AXContext aXContext, AppSettings appsets, Order pedido, ParametrosClientesNuevosWEB ParamsClientes)
    {
        /// Llenar ficha del cliente, con datos del pedido obtenidos del portal web.
        Cliente wooCliente = LLenarFichaCliente(pedido);

        String Error = "";
        String Causa = "";
        String Solucion = "";

        /// Crear o actualizar cliente en base de datos arimany-woocommerce.
        if (!Global.StrIsBlank(SMTPService.IsValidEmail(wooCliente.CorreoElectronico)))
        {
            Causa = "Correo electrónico no válido. No se asignará al cliente.";
            Solucion = "Verificar el correo electrónico registrado en el portal web.";
            wooCliente.CorreoElectronico = "";
        }

        // Crear servicio para clientes en AX.
        AXClientesServices axservice = new(appsets);
        try
        {
            // Buscar cliente por ID. de lo contrario buscar por NIT.
            (Cliente? data, Error, string causa, string solucion) = await BuscarClientePorId(_context, wooCliente.ClienteWooId, wooCliente.Nit);

            /// Si ocurre un error al buscar el cliente.
            Causa += Causa.IsNullOrEmpty() ? causa : Environment.NewLine + causa;
            Solucion += Solucion.IsNullOrEmpty() ? solucion : Environment.NewLine + solucion;
            if (Global.StrIsBlank(Error))
            {
                return (null, Error, Causa, Solucion);
            }


            /// No se encontró el cliente, crear uno nuevo.
            if (data is null)
            {
                // Cliente Nuevo.  Generar ID de cliente local.
                (long correl, Error, causa, solucion) = await GenerateLocalClienteId(_context);
                Causa += Causa.IsNullOrEmpty() ? causa : Environment.NewLine + causa;
                Solucion += Solucion.IsNullOrEmpty() ? solucion : Environment.NewLine + solucion;
                if (correl == 0)
                {
                    return (null, Error, Causa, Solucion);
                }

                /// Buscar cliente en base de datos de AX.
                (ClienteAX? clienteAX, Error, causa, solucion) = await ServiciosAXBL.ObtenerClienteAX(aXContext, wooCliente.Nit);

                /// returnar si hay un error al buscar el cliente en AX.
                Causa += Causa.IsNullOrEmpty() ? causa : Environment.NewLine + causa;
                Solucion += Solucion.IsNullOrEmpty() ? solucion : Environment.NewLine + solucion;
                if (Global.StrIsBlank(Error)) return (null, Error, Causa, Solucion);

                if (clienteAX is not null) wooCliente.ClienteAXId = clienteAX!.Cliente;

                wooCliente.ClienteId = correl;
                _context.Clientes.Add(wooCliente);
            }
            else
            {

                data!.Nombres = wooCliente.Nombres;
                data!.Apellidos = wooCliente.Apellidos;
                data!.Nit = wooCliente.Nit;
                data!.Telefono = wooCliente.Telefono;
                data!.CorreoElectronico = wooCliente.CorreoElectronico;
                _context.Clientes.Update(data);
                wooCliente = data;
            }
            await _context.SaveChangesAsync();

            ResultadoOperacionAX result = await axservice.CrearClienteWEB(wooCliente, ParamsClientes, appsets.Pais);
            if (Global.StrIsBlank(result.ErrorMsg))
            {
                Error = result.ErrorMsg;
                Causa += Causa.IsNullOrEmpty() ? result.Advertencia : Environment.NewLine + result.Advertencia;
                string soluccion = "Verifique el estado de error para conocer detalles del error." + Environment.NewLine +
                                   "Y llame al administrador del sistema.";
                Solucion += Solucion.IsNullOrEmpty() ? solucion : Environment.NewLine + solucion;
                return (null, Error, Causa, Solucion);
            }
            if (wooCliente.ClienteAXId != result.IdCliente)
            {
                wooCliente.ClienteAXId = result.IdCliente;
                _context.Clientes.Update(wooCliente);
                await _context.SaveChangesAsync();
            }

            _context.Clientes.Update(wooCliente);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Error = $"Error al crear o actualizar cliente. ID: {wooCliente.ClienteId}, NIT: {wooCliente.Nit}";
            String causa = "La base de datos ha emitido un error al intentar guardar el cliente." + Environment.NewLine +
                    Global.GetExceptionError(ex);
            String solucion = "Verificar que la tabla Clientes este disponible." + Environment.NewLine +
                          "Verificar que la base de datos arimany-woocommerce este disponible." + Environment.NewLine +
                            "Verificar la conexión a la base de datos." +
                            "Verificar que la tabla Clientes tenga permisos de escritura." + Environment.NewLine +
                            "Verificar que la tabla Clientes no este bloqueada." + Environment.NewLine +
                            "LLame al administrador del sistema.";

            Causa += Causa.IsNullOrEmpty() ? causa : Environment.NewLine + causa;
            Solucion += Solucion.IsNullOrEmpty() ? solucion : Environment.NewLine + solucion;

            return (null, Error, Causa, Solucion);
        }

        /// Procesar direcciones de cliente
        (List<ClienteDireccion> direcciones, Error, Causa, Solucion) = await ProcesarDireccionesDePedido(pedido, wooCliente, _context);
        if (direcciones.Count == 0 || !Global.StrIsBlank(Error)) return (null, Error, Causa, Solucion);

        /// Guardar direcciones de cliente.
        /// Si hay direcciones entonces guardarlas.

        try
        {
            /// Crear o actualizar direcciones del cliente.
            wooCliente.Direcciones = direcciones;
            await _context.SaveChangesAsync();


            wooCliente.Direcciones.ForEach(async c =>
            {
                ResultadoOperacionAX result;
                c.ClienteId = wooCliente.ClienteId;
                result = await axservice.CrearDireccionCliente(c, $"{wooCliente.Nombres}  {wooCliente.Apellidos}", appsets.Pais);
                if (Global.StrIsBlank(result.ErrorMsg))
                {
                    c.DireccionId = result.RecId;
                }
                Error += result.ErrorMsg + Environment.NewLine;
                Causa += result.Advertencia + Environment.NewLine;
                Solucion += "Verifique que el servicio AIF de Dynamics Ax, este funcionado" + Environment.NewLine;
            });
            if (!Global.StrIsBlank(Error)) return (null, Error, Causa, Solucion);
        }
        catch (Exception ex)
        {
            Error = $"Se produjo un error al guardar direcciones del cliente. ID: {wooCliente.ClienteId}, NIT: {wooCliente.Nit}";
            Causa = "La base de dato emitió un error." + Environment.NewLine + Global.GetExceptionError(ex);
            Solucion = "Verificar que la tabla Cliente_Direccion este disponible." + Environment.NewLine +
                          "Verificar que la base de datos arimany-woocommerce este disponible." + Environment.NewLine +
                            "Verificar la conexión a la base de datos." +
                            "Llame a sopore técnico.";
            return (null, Error, Causa, Solucion);
        }

        //Sincronizar contactos en ax telefono y correo electrónico.
        (bool ok, Causa, Solucion) = await CrearContactosEnAX(wooCliente, appsets);
        return (wooCliente, Error, Causa, Solucion);
    }


    /// <summary>
    /// Crea o actualiza un cliente en la base de datos arimany-woocommerce.  Tabla pedidos.
    /// En esta tabla se controla si un pedido ha sido procesado o no y si ya fue sincronizado,
    /// con el sistema AX.
    /// </summary>
    /// <param name="logger">Se utiliza para registro de eventos en el sistema.</param>
    /// <param name="_context">Conexión a bif (Global.StrIsBlank(result.ErrorMsg) && c.TipoDireccion == (int)LogisticsLocationRoleType.Invoice) wooCliente.ase de datos arimany-woocommerce</param>
    /// <param name="pedido">Datos del pedido a registrar y preparar para sincronizar con AX</param>
    /// <returns>El registro guardado.</returns>
    private static async Task<(Pedido?, string, String, string)> CreateOrUpdatePedido(WooCommerceContext _context, WooPedido WooPedido, String EmpresaId)
    {
        String Error = "", Causa = "", Solucion = "";
        Pedido? data = null;
        try
        {
            Pedido pedido = new()
            {
                Pedido_Id = WooPedido.PedidoId,
                Woo_Pedido_Id = (long)WooPedido.WooId,
                WooPedido = WooPedido,
                PedidoAXId = "",
                EmpresaId = EmpresaId,
                ClienteId = WooPedido.Cliente.ClienteAXId,
                DireccionId = WooPedido.Cliente.Direcciones.FirstOrDefault(c => c.TipoDireccion == LogisticsLocationRoleType.Invoice)!.DireccionId,
                EncabezadoJSON = "",
                DetalleJSON = "",
                MensajeAXId = 0,
                MensajeAX = "",
                Factura = "",
                Despachado = false,
                Operar = true,
                OperadoAX = false
            };
            data = await _context.Pedidos.FirstOrDefaultAsync(c => c.Pedido_Id == pedido.Pedido_Id);
            if (data == null)
            {
                _context.Pedidos.Add(pedido);
            }
            else
            {
                _context.Pedidos.Update(pedido);
            }
            await _context.SaveChangesAsync();
            data = pedido;
        }
        catch (Exception ex)
        {
            Error = "Error al crear o actualizar pedido.";
            Causa = "Error al guardar registro de pedido en base de datos local." + Environment.NewLine +
                    "la base de datos ha emitido un error" + Environment.NewLine +
                    Global.GetExceptionError(ex);
            Solucion = "Verificar que la tabla Pedido este disponible." + Environment.NewLine +
                       "Verificar que la base de datos arimany-woocommerce este disponible." + Environment.NewLine +
                       "Verificar la conexión a la base de datos." +
                       "Verificar que la tabla Pedido tenga permisos de escritura." + Environment.NewLine +
                       "Verificar que la tabla Pedido no este bloqueada." + Environment.NewLine +
                       "LLame al administrador del sistema.";


        }
        return (data, Error, Causa, Solucion);
    }


    /// <summary>
    /// Crear un pedido de ventas en sistema AX.
    /// </summary>
    /// <param name="_context">Contexto de base de datos local</param>
    /// <param name="appsets">Valores para configuración del servicio.</param>
    /// <param name="wooPedido"></param>
    /// <param name="pedido"></param>
    /// <returns></returns>
    private static async Task<(Boolean, string, string, string)> CrearPedidoEnAX(WooCommerceContext _context, AppSettings appsets, ParametrosClientesNuevosWEB param, WooPedido wooPedido, Pedido pedido)
    {
        /// LLenar encabezado del pedido para enviar a AX.
        EncabezadoJSON encabezado = new()
        {
            Cliente = wooPedido.Cliente!.ClienteAXId,
            Direccion = wooPedido.Cliente.Direcciones.FirstOrDefault(c => c.TipoDireccion == LogisticsLocationRoleType.Delivery)!.DireccionId,
            FechaRecepcion = DateTime.Now,
            FechaEnvio = DateTime.Now + TimeSpan.FromDays(1),
            CondicionesPago = param.TerminosDePago,
            PedidoTemporada = "0"
        };

        /// Llenar detalles del pedido para enviar a AX.
        List<DetalleJSON> detalles = [];

        foreach (var item in wooPedido.Productos)
        {
            DetalleJSON detalle = new() { ItemId = item.ItemId, VariantId = item.RetailVariantId, Cantidad = Decimal.ToInt32(item.WooCantidad), Precio = item.WooPrecio };
            detalles.Add(detalle);
        }

        /// Serializar encabezado y detalles del pedido en formato JSON.
        String jsonEncabezado = JsonSerializer.Serialize(encabezado);
        String jsonDetalle = JsonSerializer.Serialize(detalles);

        /// Crear servicio AX para pedidos de venta.
        AXPedidosService pedidosService = new(appsets);

        /// Enviar pedido a AX.
        EstadoOperacionPedidoAX result = await pedidosService.CrearPedido(jsonEncabezado, jsonDetalle);

        String Error = "", Causa = "", Solucion = "";

        /// Si ocurre un error al enviar el pedido a AX, entonces retornar el error.
        if (!Global.StrIsBlank(result.MsgError))
        {
            Error = $"Se produjo un error al enviar el pedido al sistema AX. {wooPedido.WooId}";
            Causa = "Error en el servicio AIF de AX al enviar el pedido de venta." + Environment.NewLine + result.MsgError;
            Solucion = "Verificar que el servicio AIF de AX este disponible." + Environment.NewLine +
                        "Verificar que el sistema AX este Activo." + Environment.NewLine +
                        "Llame a soporte técnico.";
            return (false, Error, Causa, Solucion);
        }

        /// Actualizar registro de pedido en base de datos local.
        pedido.PedidoAXId = result.SalesId;
        pedido.EncabezadoJSON = jsonEncabezado;
        pedido.DetalleJSON = jsonDetalle;
        pedido.MensajeAX = result.MsgError!;
        pedido.MensajeAXId = result.Error;
        pedido.Operar = false;
        pedido.OperadoAX = true;
        await _context.SaveChangesAsync();

        /// Actualizar registro de WooPedido en base de datos local.
        wooPedido.Operado = true;
        wooPedido.SincronizarProducto = false;
        wooPedido.GenerarEncabezadoJSON = false;
        wooPedido.GenerarDetalleJSON = false;
        await _context.SaveChangesAsync();
        return (true, Error, Causa, Solucion);
    }


    /// <summary>
    /// Obtener NIT del cliente.
    /// </summary>
    /// <param name="order">Pedido de venta registrado en woocommerce.</param>
    /// <returns>El NIT asociado al cliente</returns>
    private static string ObtenerNit(Order order)
    {
        String Nit = "C/F";

        // Buscar nit en los metados del pedido.
        var nit = order.meta_data.FirstOrDefault(c => c.key.Equals("_nit") ||
                                                      c.key.Equals("_billing_nit") ||
                                                      c.key.Equals("billing_nit") ||
                                                      c.key.Equals("nit"));
        if (nit != null) Nit = (nit.value as string) ?? "C/F";   // asumir consumidor final sino viene el nit.
        return Nit;
    }

    /// <summary>
    /// LLenar ficha del cliente para ser registrado en,
    /// la base de datos arimany-woocommerce.
    /// </summary>
    /// <param name="pedido">Datos del pedido woocommerce, de aqui se extraen los datos del cliente</param>
    /// <returns>Ficha de cliente</returns>
    private static Cliente LLenarFichaCliente(Order pedido)
    {
        String Nombre = "";
        String Apellido = "";
        String Nit = ObtenerNit(pedido);
        String Telefono = "";
        String Email = "";


        /// Obtener Nombre de facturacion de los metadatos, 
        /// omitir el nombre de cliente que viene en el objeto billing del JSON.
        var metadata = pedido.meta_data.Where(c => c.key.ToLower().Equals("_infilename"));
        if (metadata.Any()) Nombre = metadata.ToList()[0].value as string ?? "";

        /// Si hay datos de facturación entonces los datos que faltan,
        /// para completar la ficha del cliente.
        if (pedido.billing is not null)
        {
            if (Global.StrIsBlank(Nombre)) Nombre = pedido.billing.first_name;
            if (Global.StrIsBlank(Apellido)) Apellido = pedido.billing.last_name;
            Telefono = pedido.billing.phone ?? "";
            Email = pedido.billing.email ?? "";
        }

        /// Si hay datos de envio, entonces completar la ficha del cliente.
        if (pedido.shipping is not null)
        {
            if (Global.StrIsBlank(Nombre)) Nombre = pedido.shipping.first_name;
            if (Global.StrIsBlank(Apellido)) Apellido = pedido.shipping.last_name;
        }


        Cliente cliente = new()
        {
            ClienteId = 0,
            ClienteWooId = (int)(pedido.customer_id ?? 0),
            ClienteAXId = "",
            Nit = Nit,
            Nombres = Global.ScapeQuotation(Nombre),
            Apellidos = Global.ScapeQuotation(Apellido),
            Telefono = Global.ScapeQuotation(Telefono),
            CorreoElectronico = Global.ScapeQuotation(Email),
            Fecha = DateTime.Now,
        };
        return cliente;
    }

    /// <summary>
    /// Registrar direcciones de cliente en sistema AX.
    /// </summary>
    /// <param name="pedido">Pedido con datos de contacto de cliente</param>
    /// <param name="cliente">Datos del cliente a procesar</param>
    /// <param name="_context">Conexión a base de datos</param>
    /// <returns>Listado de direcciones registradas o error si falla el servicio.</returns>
    private static async Task<(List<ClienteDireccion>, string, string, string)> ProcesarDireccionesDePedido(Order pedido, Cliente cliente, WooCommerceContext _context)
    {

        /// Procesar dirección de facturación.
        /// 1. Buscar dirección en metadatos del pedido.
        /// 2. Si no hay dirección de facturación, entonces usar la dirección de envio.
        /// 3. Si no hay dirección de envio, entonces usar la dirección de facturación.

        /// Obtener listado de direcciones en base de datos local.

        String Error = "", Causa = "", Solucion = "";


        (List<ClienteDireccion>? direccionesCliente, Error, Causa, Solucion) = await BuscarDireccionesCliente(_context, cliente.ClienteId);
        direccionesCliente ??= direccionesCliente = [];

        /// obtener dirección de facturación.
        ClienteDireccion dirFacturacion = new() { TipoDireccion = LogisticsLocationRoleType.Invoice, ClienteId = cliente.ClienteId };
        ClienteDireccion dirEnvio = new() { TipoDireccion = LogisticsLocationRoleType.Delivery, ClienteId = cliente.ClienteId };

        /// Obtener Direccion de facturación de los metadatos, 
        /// omitir la dirección de facturación que viene en el objeto billing del pedido.
        var metadata = pedido.meta_data.Where(c => c.key.ToLower().Equals("_infileaddr"));
        if (metadata.Any()) dirFacturacion.Direccion = metadata.ToList()[0].value as string ?? "";

        if (pedido.billing is not null)
        {
            if (Global.StrIsBlank(dirFacturacion.Direccion))
            {
                dirFacturacion.Direccion = pedido.billing.address_1 ?? "";
                dirFacturacion.DireccionComplemento = pedido.billing.address_2 ?? "";
            }
            dirFacturacion.DepartamentoAXId = pedido.billing.state ?? "";
            dirFacturacion.MunicipioAXId = pedido.billing.city ?? "";
            dirFacturacion.Ciudad = pedido.billing.city ?? "";
        }

        if (pedido.shipping is not null)
        {
            if (Global.StrIsBlank(dirFacturacion.Direccion))
            {
                dirFacturacion.Direccion = pedido.shipping.address_1 ?? "";
                dirFacturacion.DireccionComplemento = pedido.shipping.address_2 ?? "";
            }
            if (Global.StrIsBlank(dirFacturacion.DepartamentoAXId)) dirFacturacion.DepartamentoAXId = pedido.shipping.state ?? "";
            if (Global.StrIsBlank(dirFacturacion.MunicipioAXId)) dirFacturacion.MunicipioAXId = pedido.shipping.city ?? "";

            dirEnvio.Direccion = pedido.shipping.address_1 ?? "";
            dirEnvio.DireccionComplemento = pedido.shipping.address_2 ?? "";
            dirEnvio.DepartamentoAXId = pedido.shipping.state ?? "";
            dirEnvio.MunicipioAXId = pedido.shipping.city ?? "";
            dirEnvio.Ciudad = pedido.shipping.city ?? "";
        }
        else
        {
            dirEnvio.Direccion = dirFacturacion.Direccion;
            dirEnvio.DireccionComplemento = dirFacturacion.DireccionComplemento;
            dirEnvio.DepartamentoAXId = dirFacturacion.DepartamentoAXId;
            dirEnvio.MunicipioAXId = dirFacturacion.MunicipioAXId;
            dirEnvio.Ciudad = dirFacturacion.Ciudad;
        }
        if (Global.StrIsBlank(dirFacturacion.MunicipioAXId))
        {
            Error = "No se encontró el municipio de la dirección de facturación.";
            Causa = "Se produjo un error al buscar el municipio de la dirección de facturación.";
            Solucion = "Verificar en el pedido registrado en el portal web. " + Environment.NewLine +
                       "1. La dirección de facturación tenga un municipio asignado." + Environment.NewLine +
                       "2. Que el municipio este registrado en la tabla de DepartamentosYMunicipios.";
            return ([], Error, Causa, Solucion);
        }


        (DepartamentosYMunicipios? municipio, Error, Causa, Solucion) = await BuscarDepartamentoPorMunicipio(_context, dirFacturacion.MunicipioAXId);
        if (Global.StrIsBlank(Error)) return ([], Error, Causa, Solucion);


        dirFacturacion.MunicipioAXId = municipio!.Municipio;
        dirFacturacion.DepartamentoAXId = municipio!.Departamento;

        dirEnvio.MunicipioAXId = dirFacturacion.MunicipioAXId;
        dirEnvio.DepartamentoAXId = dirFacturacion.DepartamentoAXId;

        if (direccionesCliente.Count != 0)
        {
            (bool ok, Error, Causa, Solucion) = BorrarDireccionesDelCliente(_context, direccionesCliente);
            if (!ok) return ([], Error, Causa, Solucion);
        }

        dirFacturacion.ClienteId = cliente.ClienteId;
        dirFacturacion.Cliente = cliente;
        dirEnvio.ClienteId = cliente.ClienteId;
        dirEnvio.Cliente = cliente;

        direccionesCliente.Add(dirFacturacion);
        direccionesCliente.Add(dirEnvio);
        return (direccionesCliente, Error, Causa, Solucion);
    }

    /// <summary>
    /// Registrar datos de contacto del cliente en sistema AX, teléfono y correo electrónico.
    /// </summary>
    /// <param name="wooClient">Datos del cliente, y contactos asociados</param>
    /// <param name="appsets">Configuración del servicio</param>
    /// <returns>True si el registro fue exitoso, false de lo contrario.  
    /// Si hay errores se devuelven un string con el texto y un string con posible solución.</returns>
    private static async Task<(bool, string, string)> CrearContactosEnAX(Cliente wooClient, AppSettings appsets)
    {
        String Causa = "", Solucion = "";

        InfoContactoClienteJSON contacto;

        /// Registrar numero de teléfono en ficha del cliente.
        if (!Global.StrIsBlank(wooClient.Telefono))
        {
            contacto = new()
            {
                CodigoCliente = wooClient.ClienteAXId,
                TipoDatoContacto = LogisticsElectronicAddressMethodType.Teléfono,
                Descripcion = "Numero de teléfonico",
                Dato = wooClient.Telefono,
                EsPrimario = true
            };
            String jsontelefono = JsonSerializer.Serialize(contacto);
            AXClientesServices axservice = new(appsets);
            ResultadoOperacionAX result = await axservice.CrearContactoDeCliente(jsontelefono);
            if (!Global.StrIsBlank(result.ErrorMsg))
            {
                Causa += "Error al crear teléfono En sistema AX" + Environment.NewLine + result.ErrorMsg + Environment.NewLine +
                          (result.Advertencia.IsNullOrEmpty() ? "Advertencias: " + result.Advertencia : "");
                Solucion += "Reporte el incidente al administrador del sistema.";
            }
        }

        /// Registrar correo electrónico en ficha del cliente.
        if (!Global.StrIsBlank(wooClient.CorreoElectronico))
        {
            contacto = new()
            {
                CodigoCliente = wooClient.ClienteAXId,
                TipoDatoContacto = LogisticsElectronicAddressMethodType.Email,
                Descripcion = "Buzón de correo electrónico",
                Dato = wooClient.CorreoElectronico,
                EsPrimario = true
            };
            String jsonCorreo = JsonSerializer.Serialize(contacto);
            AXClientesServices axservice = new(appsets);
            ResultadoOperacionAX result = await axservice.CrearContactoDeCliente(jsonCorreo);
            if (!Global.StrIsBlank(result.ErrorMsg))
            {
                Causa += "Error al crear email en sistema AX" + Environment.NewLine + result.ErrorMsg + Environment.NewLine +
                         (result.Advertencia.IsNullOrEmpty() ? "Advertencias: " + result.Advertencia : "");
                Solucion += "Reporte el incidente al administrador del sistema.";
            }
        }
        return (Global.StrIsBlank(Causa), Causa, Solucion);
    }


    /// <summary>
    /// Obtener los parametros del servicio, 
    /// para el registro de clientes en Dynamics AX.
    /// </summary>
    /// <param name="context">Conexión a base de datos arimany-woocommerce</param>
    /// <returns>Registro con parametros del sistema, o null si ocurre un error</returns>
    public static async Task<(ParametrosWooCommerce?, string, string, string)> ObtenerParametros(WooCommerceContext context)
    {
        String Error = "", Causa = "", Solucion = "";
        try
        {
            var parametros = await context.ParametrosWooCommerce.FirstOrDefaultAsync();

            if (parametros is not null) return (parametros, Error, Causa, Solucion);

            Error = "No se pudo obtener un registro con parametros para el servicio";
            Causa = "NO se encontro ningun registro al consultar tabla de ParametrosWooCommerce";
            Solucion = "Agregue un registro con parametros del servicio web.  " + Environment.NewLine +
                       "Asigne el código de producto que se utilizara para cargos por envio.";
        }
        catch (Exception ex)
        {
            Error = "Se produjo un error al obtener parametros del servicio web.";
            Causa = "La base de datos emitió un error al consultar la tabla de ParametrosWooCommerce." + Environment.NewLine +
                    Global.GetExceptionError(ex);
            Solucion = "1. Verificar, la disponibilidad de la tabla ParametrosWooCommerce." + Environment.NewLine +
                       "2. Verfiicar la conexión a la base de datos arimany-woocommerce." + Environment.NewLine +
                       "3. Notificar al administrador del sistema.";
        }
        return (null, Error, Causa, Solucion);
    }
}
