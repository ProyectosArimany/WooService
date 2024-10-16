using WooService.BL;
using WooService.Contexts;
using WooService.Models;
using WooService.Providers;
using WooService.Providers.Notificaciones;
using WooService.Utils;

namespace WooService.Workers;


public class WooServiceWorker(ILogger logger, AppSettings appsets, AXContext aXContext, WooCommerceContext context)
{

    /// <summary>
    /// Procesa los pedidos de WooCommerce según el estado especificado.
    /// </summary>
    /// <param name="OrderStatus">Estado del pedido a procesar.</param>
    /// <returns>Nada</returns>

    public async Task ProcessOrders(string OrderStatus, bool isLoggingEnabled)
    {

        /// Obtener lista de destinatarios para notificaciones.
        List<string> destinatariosDeNotificaciones = [.. appsets.EMAILAdmin.Split(';')];

        /// Servicio de notificaciones.
        ErrorNotificationService notificacionesService = new(
                                                                destinatariosDeNotificaciones,
                                                                appsets.SMTPServer,
                                                                appsets.SMTPPort,
                                                                appsets.EMAILUser,
                                                                appsets.EMAILPass,
                                                                context,
                                                                appsets.EMAILTemplatePath,
                                                                appsets.SMTPEnableSSL,
                                                                isLoggingEnabled,
                                                                logger
                                                            );


        string Error, Causa, Solucion;

        /// Cliente para acceder al API de WooCommerce.
        WooProvider _wooProvider = new(appsets.WooURL, appsets.WooConsumerKey, appsets.WooConsumerSecret);
        if (!Global.StrIsBlank(_wooProvider.GetMsgError))
        {
            Error = "Error al inicializar el cliente de WooCommerce";
            Causa = "Se produjo un error al contactar el sitio web de arimany." + Environment.NewLine + _wooProvider.GetMsgError;
            Solucion = "LLamar a soporte de TI para verificar el estado del servicio arimany.com";
            notificacionesService.HandleError(new HandleErrorParams { ErrorMessage = Error, PossibleCause = Causa, SuggestedSolution = Solucion, TipoNotificacion = "Error", WooPedidoId = 0 });
            return;
        }

        /// Parámetros de WooCommerce, para obtener código utilizado para cargos por envio.
        (ParametrosWooCommerce? ParametrosLineasPedido, Error, Causa, Solucion) = await WooServiceBL.ObtenerParametros(context);
        if (!Global.StrIsBlank(Error))
        {
            notificacionesService.HandleError(new HandleErrorParams { ErrorMessage = Error, PossibleCause = Causa, SuggestedSolution = Solucion, TipoNotificacion = "Error", WooPedidoId = 0 });
            return;
        }

        /// Parámetros para creación de clientes nuevos, en sistema AX.
        (ParametrosClientesNuevosWEB? ParametrosClientes, Error, Causa, Solucion) = await ServiciosAXBL.ObtenerParametrosClientesNuevosWEB(aXContext);
        if (!Global.StrIsBlank(Error))
        {
            notificacionesService.HandleError(new HandleErrorParams { ErrorMessage = Error, PossibleCause = Causa, SuggestedSolution = Solucion, TipoNotificacion = "Error", WooPedidoId = 0 });
            return;
        }
        try
        {
            /// Obtener pedidos del portal WooCommerce.
            var orders = await _wooProvider.ObtenerPedidos(OrderStatus);
            if (orders.Count == 0 && !Global.StrIsBlank(_wooProvider.GetMsgError))
            {
                Error = "Se produjo un error al obtener pedidos del portal web";
                Causa = "No se obtuvieron pedidos del portal web." + Environment.NewLine + _wooProvider.GetMsgError;
                Solucion = "Llame a informatica para verificar el estado del servicio arimany.com";
                notificacionesService.HandleError(new HandleErrorParams { ErrorMessage = Error, PossibleCause = Causa, SuggestedSolution = Solucion, TipoNotificacion = "Advertencia", WooPedidoId = 0 });
                return;
            }

            foreach (var order in orders)
            {
                (WooPedido? pedido, Error, Causa, Solucion) = await WooServiceBL.CreateOrUpdateWooPedido(context, aXContext, order, ParametrosClientes!, ParametrosLineasPedido, appsets);
                if (!Global.StrIsBlank(Error)) notificacionesService.HandleError(new HandleErrorParams { ErrorMessage = Error, PossibleCause = Causa, SuggestedSolution = Solucion, TipoNotificacion = "Error", WooPedidoId = 0 });
                if (Global.StrIsBlank(Error) && !Global.StrIsBlank(Causa)) notificacionesService.HandleError(new HandleErrorParams { ErrorMessage = Error, PossibleCause = Causa, SuggestedSolution = Solucion, TipoNotificacion = "Advertencia", WooPedidoId = 0 });
            }
        }
        catch (Exception ex)
        {
            Error = "Error al procesar pedidos del portal web";
            Causa = $"La causa del problema fue {ex.Message}";
            Solucion = "Verificar que el servicio arimany.com este funcionando correcta." + Environment.NewLine +
                             "Reportar a soporte de TI.";
            notificacionesService.HandleError(new HandleErrorParams { ErrorMessage = Error, PossibleCause = Causa, SuggestedSolution = Solucion, TipoNotificacion = "Error", WooPedidoId = 0 });
            // await NotifyAdministrator(_appSettings.EMAILUser, _appSettings.EMAILPass, _appSettings.EMAILUser, "Error processing orders", ex.ToString());
        }
    }

}