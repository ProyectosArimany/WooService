using Microsoft.EntityFrameworkCore;
using WooService.Workers;
using WooService.Contexts;
using WooService.Utils;
using WooService.Models;
using System.Text.Json;
using WooCommerceNET.WooCommerce.v3;
using WooService.Providers.AXServices;
using WooService.Providers;
using Microsoft.EntityFrameworkCore.Metadata;

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
    private static async Task<(long, int, string)> GenerateLocalOrderId(WooCommerceContext _context)
    {
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
                    return (correl, 1, "");
                }
                else
                {
                    return (0, 0, $"Error al generar ID de pedido local");
                }
            }
            else
            {
                long correl = data.PedidoId + 1;
                string strCorrel = correl.ToString();
                if (int.TryParse(strCorrel.AsSpan(strCorrel.Length - 3, 3), out int intcorrelativo))
                {
                    return (correl, intcorrelativo, "");
                }
                else
                {
                    return (0, 0, "Error al generar ID de pedido local");
                }
            }
        }
        catch (Exception ex)
        {
            return (0, 0, "Error al generar ID de pedido local" + Environment.NewLine + Global.GetExceptionError(ex));
        }
    }


    /// <summary>
    /// Genera un ID de cliente local usando la fecha (formato yyyyMMdd),y
    /// un correlativo de 3 digitos.
    /// </summary>
    /// <returns>Tuple con el ID del cliente generado, o Cero y un mensaje de error (si lo hubiese).</returns>
    private static async Task<(long, string)> GenerateLocalClienteId(WooCommerceContext _context)
    {
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
                    return (correl, "");
                }
                else
                {
                    return (0, "Error al obtener numero de pedido local");
                }
            }
            else
            {
                return (data.ClienteId + 1, "");
            }
        }
        catch (Exception ex)
        {
            return (0, "Error generating local order id" + Environment.NewLine + Global.GetExceptionError(ex));
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
    private static async Task<(DepartamentosYMunicipios?, string)> BuscarDepartamentoPorMunicipio(WooCommerceContext _context, string municipio)
    {
        try
        {
            var data = await _context.DepartamentosYMunicipios.FirstOrDefaultAsync(c => c.Municipio == municipio);
            if (data != null)
            {
                return (data, "");
            }
            else
            {
                return (null, "No se encontró el Departamento/Municipio.");
            }
        }
        catch (Exception ex)
        {
            return (null, "Se produjo un error al buscar el Departamento/Municipio." + Environment.NewLine + Global.GetExceptionError(ex));
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
    private static async Task<(Cliente?, string)> BuscarClientePorId(WooCommerceContext _context, long clienteId, string NIT)
    {
        if (clienteId == 0 && Global.StrIsBlank(NIT))
        {
            return (null, "Debe proporcionar un ID de cliente o NIT.");
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
            string msg = cliente is null ? $"No se encontró el cliente. ID: {clienteId}, NIT: {NIT}" : "";

            return (cliente, msg);
        }
        catch (Exception ex)
        {
            string msg = $"Se produjo un error al buscar el cliente. ID: {clienteId}, NIT: {NIT}" + Environment.NewLine + Global.GetExceptionError(ex);
            return (null, msg);
        }
    }

    /// <summary>
    /// Busca las direcciones de un cliente.
    /// </summary>
    /// <param name="logger">Se utiliza para registro de eventos en el sistema.</param>
    /// <param name="_context">Conexión a base de datos arimany-woocommerce</param>
    /// <param name="clienteId">Id del cliente en portal web (CustumerID)</param>
    /// <returns>Listado de direcciones o null si ocurre un error.</returns>
    private static async Task<(List<ClienteDireccion>?, string)> BuscarDireccionesCliente(WooCommerceContext _context, long clienteId)
    {
        try
        {
            var data = await _context.ClienteDirecciones.Where(c => c.ClienteId == clienteId).ToListAsync();
            return (data, "");
        }
        catch (Exception ex)
        {
            return (null, "Error al buscar direcciones del cliente." + Global.GetExceptionError(ex));
        }
    }

    /// <summary>
    /// Elimina las direcciones asociadas a un cliente.
    /// </summary>
    /// <param name="_context">Conexión a base de datos local.</param>
    /// <param name="direcciones">Lista de direcciones a quitar del cliente.</param>
    /// <returns>True si la operación no produjo errores, de lo contratrio False.</returns>
    private static (bool, string) BorrarDireccionesDelCliente(WooCommerceContext _context, List<ClienteDireccion> direcciones)
    {
        try
        {
            _context.ClienteDirecciones.RemoveRange(direcciones);
            return (true, "");
        }
        catch (Exception ex)
        {
            return (false, "Error al borrar direcciones del cliente." + Global.GetExceptionError(ex));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger">Se utiliza para registro de eventos en el sistema.</param>
    /// <param name="_context">Conexión a base de datos arimany-woocommerce.</param>
    /// <param name="wooPedido">Datos del pedido obtenidos del portal web.</param>
    /// <returns>El registro creado o actualizado. Null si ocurre un error.</returns>
    public static async Task<(WooPedido?, string, string)> CreateOrUpdateWooPedido(WooCommerceContext _context,
                                                                                    AXContext aXContext,
                                                                                    Order pedido,
                                                                                    ParametrosClientesNuevosWEB ParamsClientes,
                                                                                    ParametrosWooCommerce? ParametrosLineasPedido,
                                                                                    AppSettings appsets)
    {
        _context.Database.BeginTransaction();
        try
        {
            int pedidoWooId = ((int?)pedido.id) ?? 0;
            var data = await _context.WooPedidos.FirstOrDefaultAsync(c => c.WooId == pedidoWooId);

            WooPedido wooPedido = new();
            if (data != null) return (wooPedido, "", "Pedido ya se registro"); // Pedido ya existe.

            /// Pedido Nuevo, procesar y registrar en base de datos local, los datos del cliente,
            /// que vienen el pedido.

            (Cliente? cliente, string msg, String warning) = await CreateOrUpdateWooCliente(_context, aXContext, appsets, pedido, ParamsClientes);

            /// Si ocurre un error al crear o actualizar el cliente, entonces retornar el error.
            if (!Global.StrIsBlank(msg))
            {
                _context.Database.RollbackTransaction();
                return (null, msg, warning);
            }

            /// Generar ID de pedido local.
            (long correl, int intcorrelativo, msg) = await GenerateLocalOrderId(_context);
            if (correl == 0)
            {
                _context.Database.RollbackTransaction();
                return (null, msg, warning);
            }

            wooPedido.PedidoId = 0;
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

            (bool ok, msg) = await AgregarProductosWoo(aXContext, pedido, wooPedido, ParametrosLineasPedido!.CodigoDeCargosPorEnvio, CargoPorEnvio);

            if (!ok)
            {
                _context.Database.RollbackTransaction();
                return (null, msg, warning);
            }
            _context.WooPedidos.Add(wooPedido);
            await _context.SaveChangesAsync();

            /// Crear registro de pedido en base de datos local.
            (Pedido? pedidoLocal, msg) = await CreateOrUpdatePedido(_context, wooPedido, appsets.Pais);
            if (Global.StrIsBlank(msg))
            {
                _context.Database.RollbackTransaction();
                return (null, msg, warning);
            }

            // Registrar pedido en Sistema AX.
            (ok, msg) = await CrearPedidoEnAX(_context, appsets, ParamsClientes, wooPedido, pedidoLocal!);
            if (!ok)
            {
                _context.Database.RollbackTransaction();
                return (null, msg, warning);
            }

            _context.Database.CommitTransaction();
            return (wooPedido, "", warning);
        }
        catch (Exception ex)
        {
            string msg = $"Error al crear o actualizar pedido.\n WOOID: {pedido.id}, cliente: {pedido.billing.first_name} {pedido.billing.last_name}" +
                         Environment.NewLine + Global.GetExceptionError(ex);
            _context.Database.RollbackTransaction();
            return (null, msg, "");
        }

    }

    /// <summary>
    /// Agrega los productos de un pedido al registro de la base de datos arimany-woocommerce.
    /// </summary>
    /// <param name="aXContext">Conexión a base de datos de AX</param>
    /// <param name="pedido"></param>
    /// <param name="wooPedido"></param>
    /// <returns></returns>
    private static async Task<(Boolean, string)> AgregarProductosWoo(AXContext aXContext,
                                                                     Order pedido,
                                                                     WooPedido wooPedido,
                                                                     String CodigoPorCargoEnvio,
                                                                     decimal CargoPorEnvio)
    {
        string errors = "";
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
            (CatalogoProductos? producto, string msg) = await ServiciosAXBL.ObtenerCatalogoProductos(aXContext, item.sku);
            if (producto is null)
            {
                errors += msgError + msg + Environment.NewLine;
                continue;
            }

            decimal precio = item.price ?? 0;
            decimal cantidad = item.quantity ?? 0;
            decimal total = item.total ?? precio * cantidad;

            if (precio == 0 || cantidad == 0)
            {
                errors += msgError + $"Falta el precio o la cantidad, Producto: {item.sku}." + Environment.NewLine;
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
            if (Global.StrIsBlank(errors))
            {
                productos.Add(wooPedidoNuevo);
            }
        }

        if (CargoPorEnvio != 0 && !Global.StrIsBlank(CodigoPorCargoEnvio))
            productos.Add(new WooPedidoProducto() { WooSKU = CodigoPorCargoEnvio, WooNombre = "Cargo por envios", WooCantidad = 1, WooPrecio = CargoPorEnvio });

        wooPedido.Productos = productos;
        return (!Global.StrIsBlank(errors), errors);
    }


    /// <summary>
    /// Crea o actualiza un cliente en la base de datos arimany-woocommerce.
    /// </summary>
    /// <param name="logger">Se utiliza para registro de eventos en el sistema.</param>
    /// <param name="_context">Conexión a base de datos arimany-woocommerce</param>
    /// <param name="wooCliente">Datos del cliente obtenidos del portal web, 
    /// para crear o actualizar un registro de cliente en base de datos local.</param>
    /// <returns>El registro del cliente, creado o actualizado. Null si ocurre un error</returns>
    private static async Task<(Cliente?, string, string)> CreateOrUpdateWooCliente(WooCommerceContext _context, AXContext aXContext, AppSettings appsets, Order pedido, ParametrosClientesNuevosWEB ParamsClientes)
    {
        /// Llenar ficha del cliente, con datos del pedido obtenidos del portal web.
        Cliente wooCliente = LLenarFichaCliente(pedido);
        String warning = "";
        String Error = "";

        /// Crear o actualizar cliente en base de datos arimany-woocommerce.
        if (!Global.StrIsBlank(SMTPService.IsValidEmail(wooCliente.CorreoElectronico)))
        {
            warning = "Correo electrónico no válido. No se asignará al cliente.";
            wooCliente.CorreoElectronico = "";
        }

        // Crear servicio para clientes en AX.
        AXClientesServices axservice = new(appsets);
        try
        {
            // Buscar cliente por ID. de lo contrario buscar por NIT.
            (Cliente? data, Error) = await BuscarClientePorId(_context, wooCliente.ClienteWooId, wooCliente.Nit);

            /// Si ocurre un error al buscar el cliente.
            /// entonces retornar el error.
            if (Global.StrIsBlank(Error)) return (null, Error, warning);

            /// No se encontró el cliente, crear uno nuevo.
            if (data is null)
            {
                // Cliente Nuevo.  Generar ID de cliente local.
                (long correl, Error) = await GenerateLocalClienteId(_context);
                if (correl == 0)
                {
                    return (null, Error, warning);
                }

                /// Buscar cliente en base de datos de AX.
                (ClienteAX? clienteAX, Error) = await ServiciosAXBL.ObtenerClienteAX(aXContext, wooCliente.Nit);

                /// returnar si hay un error al buscar el cliente en AX.
                if (Global.StrIsBlank(Error)) return (null, Error, warning);

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
            if (Global.StrIsBlank(result.ErrorMsg)) return (null, result.ErrorMsg, result.Advertencia);
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
            String textError = $"Error al crear o actualizar cliente. ID: {wooCliente.ClienteId}, NIT: {wooCliente.Nit}";
            return (null, textError + Environment.NewLine + Global.GetExceptionError(ex), warning);
        }

        /// Procesar direcciones de cliente
        (List<ClienteDireccion> direcciones, string msg) = await ProcesarDireccionesDePedido(pedido, wooCliente, _context);
        if (direcciones.Count == 0 || !Global.StrIsBlank(msg)) return (null, msg, warning);

        /// Guardar direcciones de cliente.
        /// Si hay direcciones entonces guardarlas.

        try
        {
            /// Crear o actualizar direcciones del cliente.
            wooCliente.Direcciones = direcciones;
            await _context.SaveChangesAsync();

            /// Registrar direcciones en AX.
            warning = "";
            Error = "";

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
                warning += result.Advertencia + Environment.NewLine;
            });
            if (!Global.StrIsBlank(Error)) return (null, Error, warning);
        }
        catch (Exception ex)
        {
            String textError = $"Error al guardar direcciones del cliente. ID: {wooCliente.ClienteId}, NIT: {wooCliente.Nit}";
            return (null, textError + Environment.NewLine + Global.GetExceptionError(ex), warning);
        }

        //Sincronizar contactos en ax telefono y correo electrónico.
        (bool ok, msg) = await CrearContactosEnAX(wooCliente, appsets);
        return (wooCliente, "", warning + Environment.NewLine + msg);
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
    private static async Task<(Pedido?, string)> CreateOrUpdatePedido(WooCommerceContext _context, WooPedido WooPedido, String EmpresaId)
    {
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
            var data = await _context.Pedidos.FirstOrDefaultAsync(c => c.Pedido_Id == pedido.Pedido_Id);
            if (data == null)
            {
                _context.Pedidos.Add(pedido);
            }
            else
            {
                _context.Pedidos.Update(pedido);
            }
            await _context.SaveChangesAsync();
            return (pedido, "");
        }
        catch (Exception ex)
        {
            string msg = $"Error al crear o actualizar pedido. ID: {WooPedido.WooId}" + Environment.NewLine + Global.GetExceptionError(ex);
            return (null, msg);
        }
    }


    /// <summary>
    /// Crear un pedido de ventas en sistema AX.
    /// </summary>
    /// <param name="_context">Contexto de base de datos local</param>
    /// <param name="appsets">Valores para configuración del servicio.</param>
    /// <param name="wooPedido"></param>
    /// <param name="pedido"></param>
    /// <returns></returns>
    private static async Task<(Boolean, string)> CrearPedidoEnAX(WooCommerceContext _context, AppSettings appsets, ParametrosClientesNuevosWEB param, WooPedido wooPedido, Pedido pedido)
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

        /// Si ocurre un error al enviar el pedido a AX, entonces retornar el error.
        if (result.MsgError is not null) return (false, result.MsgError);

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
        return (true, "");
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


    private static async Task<(List<ClienteDireccion>, string)> ProcesarDireccionesDePedido(Order pedido, Cliente cliente, WooCommerceContext _context)
    {

        /// Procesar dirección de facturación.
        /// 1. Buscar dirección en metadatos del pedido.
        /// 2. Si no hay dirección de facturación, entonces usar la dirección de envio.
        /// 3. Si no hay dirección de envio, entonces usar la dirección de facturación.

        /// Obtener listado de direcciones en base de datos local.

        (List<ClienteDireccion>? direccionesCliente, string Error) = await BuscarDireccionesCliente(_context, cliente.ClienteId);
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
            return ([], "No se encontró el municipio de la dirección de facturación.");
        }
        (DepartamentosYMunicipios? municipio, string msgError) = await BuscarDepartamentoPorMunicipio(_context, dirFacturacion.MunicipioAXId);
        if (Global.StrIsBlank(msgError)) return ([], msgError);

        // terminar el municipio y departamento no existen.
        if (municipio is null && Global.StrIsBlank(msgError))
        {
            return ([], $"No se encontró el municipio de la dirección de facturación {dirFacturacion.MunicipioAXId}.");
        }
        dirFacturacion.MunicipioAXId = municipio!.Municipio;
        dirFacturacion.DepartamentoAXId = municipio!.Departamento;

        dirEnvio.MunicipioAXId = dirFacturacion.MunicipioAXId;
        dirEnvio.DepartamentoAXId = dirFacturacion.DepartamentoAXId;

        if (direccionesCliente.Count != 0)
        {
            (bool ok, string msg) = BorrarDireccionesDelCliente(_context, direccionesCliente);
            if (!ok) return ([], msg);
        }

        dirFacturacion.ClienteId = cliente.ClienteId;
        dirFacturacion.Cliente = cliente;
        dirEnvio.ClienteId = cliente.ClienteId;
        dirEnvio.Cliente = cliente;

        direccionesCliente.Add(dirFacturacion);
        direccionesCliente.Add(dirEnvio);
        return (direccionesCliente, "");
    }


    private static async Task<(bool, string)> CrearContactosEnAX(Cliente wooClient, AppSettings appsets)
    {
        String warning = "";

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
            if (!Global.StrIsBlank(result.ErrorMsg)) warning += result.ErrorMsg + Environment.NewLine;
        }

        /// Registrar correo electrónico en ficha del cliente.
        if (!Global.StrIsBlank(wooClient.Telefono))
        {
            contacto = new()
            {
                CodigoCliente = wooClient.ClienteAXId,
                TipoDatoContacto = LogisticsElectronicAddressMethodType.Email,
                Descripcion = "Buzón de correo electrónico",
                Dato = wooClient.CorreoElectronico,
                EsPrimario = true
            };
            String jsontelefono = JsonSerializer.Serialize(contacto);
            AXClientesServices axservice = new(appsets);
            ResultadoOperacionAX result = await axservice.CrearContactoDeCliente(jsontelefono);
            if (!Global.StrIsBlank(result.ErrorMsg)) warning += result.ErrorMsg + Environment.NewLine;
        }
        return (Global.StrIsBlank(warning), warning);
    }
    /// <summary>
    /// Obtener los parametros del servicio, 
    /// para el registro de clientes en Dynamics AX.
    /// </summary>
    /// <param name="context">Conexión a base de datos arimany-woocommerce</param>
    /// <returns>Registro con parametros del sistema, o null si ocurre un error</returns>
    public static async Task<(ParametrosWooCommerce?, string)> ObtenerParametros(WooCommerceContext context)
    {
        String mensajeError = string.Empty;
        try
        {
            var parametros = await context.ParametrosWooCommerce.FirstOrDefaultAsync();

            if (parametros is not null) return (parametros, string.Empty);
            mensajeError = "No se encontraron parametros";
        }
        catch (Exception ex)
        {
            mensajeError = "Error al obtener parametros" + Environment.NewLine + Global.GetExceptionError(ex);
        }
        return (null, mensajeError);
    }
}
