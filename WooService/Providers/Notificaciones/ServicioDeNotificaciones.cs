namespace WooService.Providers.Notificaciones;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using WooService.Contexts;
using WooService.Utils;

/// <summary>
/// Servicio de notificaciones WooService. Se almacena las alertas en base de datos,
/// y envía correos electrónicos a los destinatarios especificados. Notificando sobre,
/// las alertas producidas por el servicio..
/// </summary>
/// <param name="recipients">Lista de destinatarios para envíar las notificaciones.</param>
/// <param name="smtpServer">Servidor de correo saliente</param>
/// <param name="smtpPort">Puerto del servicio smtp</param>
/// <param name="senderEmail">Usuario de correo, utilizadola para conectar al servidor smtp</param>
/// <param name="senderPassword">Contraseña del usuario utilizado para enviar correos.</param>
/// <param name="dbContext">Conexión a base de datos para almacenar las notificaciones</param>
/// <param name="emailTemplatePath">Ruta del archivo html que se utilizara como cuerpo de los mensajes de correo.</param>
/// <param name="logger">Loggin manager</param>
public partial class ErrorNotificationService(
    List<string> recipients,
    string smtpServer,
    int smtpPort,
    string senderEmail,
    string senderPassword,
    WooCommerceContext dbContext,
    string emailTemplatePath,
    bool smtpEnableSSL,
    bool isLoggingEnabled,
    ILogger logger)
{


    /// <summary>
    /// Maneja los errores producidos por el servicio WooService.
    /// </summary>
    /// <param name="handleErrorParams.ErrorMessage"></param>
    /// <param name="handleErrorParams.PossibleCause"></param>
    /// <param name="handleErrorParams.suggestedSolution"></param>
    /// <param name="handleErrorParams.TipoNotificacion"></param>
    /// <param name="handleErrorParams.wooPedidoId"></param>
    public void HandleError(HandleErrorParams handleErrorParams)
    {

        string fullNotification = $"Detalle del mensaje:\n{handleErrorParams.ErrorMessage}\n\n" +
                                  $"Posible Causa:\n{handleErrorParams.PossibleCause}\n\n" +
                                  $"Posible Solucción:\n{handleErrorParams.SuggestedSolution}";

        // Save notification to database
        SaveNotification(handleErrorParams.WooPedidoId, handleErrorParams.TipoNotificacion, fullNotification);

        // Prepare data for email template
        var templateData = new Dictionary<string, string>
            {
                { "TipoNotificacion", handleErrorParams.TipoNotificacion },
                { "ErrorMessage", handleErrorParams.ErrorMessage },
                { "PossibleCause", handleErrorParams.PossibleCause },
                { "SuggestedSolution", handleErrorParams.SuggestedSolution },
                { "WooPedidoId", handleErrorParams.WooPedidoId.ToString() }
            };

        // Terminar si se produse un error al analizar la plantilla de correo electrónico
        (string emailBody, string error, string causa, string soluccion) = ParseEmailTemplate(templateData);
        if (!Global.StrIsBlank(error)) return;

        String subject = $"El servicio WooService ha emitido un mensaje de {handleErrorParams.TipoNotificacion}";

        // Send email notification
        SendNotificationEmail(subject, emailBody, true);
    }

    /// <summary>
    /// Lee la plantilla del archivo emailTemplatePath y reemplaza las 
    /// claves con los valores proporcionados en templateData.
    /// </summary>
    /// <param name="templateData">Diccionario con datos para formar el cuerpo del mensaje de correo.</param>
    /// <returns>Mensaje de texto en formato HTML</returns>
    private (String, String, String, String) ParseEmailTemplate(Dictionary<string, string> templateData)
    {

        string template = ""; // Plantilla de correo electrónico
        string error = ""; // Error al leer la plantilla
        string possibleCause = ""; // Posible causa del error
        string suggestedSolution = ""; // Solución sugerida
        try
        {
            template = File.ReadAllText(emailTemplatePath);
        }
        catch (Exception ex)
        {
            string logtext = "";
            switch (ex)
            {
                case FileNotFoundException:
                    error = Global.GetExceptionError(ex);
                    possibleCause = $"El archivo de plantilla de correo electrónico no se encontró en\n {emailTemplatePath}";
                    suggestedSolution = $"Verifique que el archivo {emailTemplatePath},\n se encuentra en la ruta indicada.";
                    logtext = "No se encontró la plantilla {EmailTemplatePath}";
                    break;
                case UnauthorizedAccessException:
                    error = Global.GetExceptionError(ex);
                    possibleCause = $"El archivo no tiene permisos suficientes para lectura.\n {emailTemplatePath}";
                    suggestedSolution = $"Verifique que el archivo {emailTemplatePath},\n" +
                                        "Tenga permisos de lectura para el servicio.";
                    logtext = "El archivo no tiene permisos suficientes para lectura {EmailTemplatePath}";
                    break;
                case ArgumentNullException:
                    error = Global.GetExceptionError(ex);
                    possibleCause = "Un argumento requerido es nulo.";
                    suggestedSolution = "Asegúrese de que todos los argumentos requeridos no sean nulos.";
                    logtext = "Argumento nulo {EmailTemplatePath}";
                    break;
                case ArgumentException:
                    error = Global.GetExceptionError(ex);
                    possibleCause = "El argumento proporcionado no es válido.";
                    suggestedSolution = "Verifique los argumentos proporcionados.";
                    logtext = "Argumento no válido {EmailTemplatePath}";
                    break;
                case PathTooLongException:
                    error = Global.GetExceptionError(ex);
                    possibleCause = "La ruta del archivo es demasiado larga.";
                    suggestedSolution = "Asegúrese de que la ruta del archivo sea más corta.";
                    logtext = "Ruta del archivo demasiado larga. {EmailTemplatePath}";
                    break;
                case DirectoryNotFoundException:
                    error = Global.GetExceptionError(ex);
                    possibleCause = $"El directorio no se encontró en la ruta especificada.\n {emailTemplatePath}";
                    suggestedSolution = $"Verifique que el directorio {emailTemplatePath} exista.";
                    logtext = "Directorio no encontrado {EmailTemplatePath}";
                    break;
                case IOException:
                    error = Global.GetExceptionError(ex);
                    possibleCause = "Ocurrió un error de E/S al intentar leer el archivo.";
                    suggestedSolution = "Verifique que el archivo no esté en uso por otro proceso;\n" +
                                        "o que no esté dañado o el medio de almacenamiento exista.";
                    logtext = "Error de E/S, al leer archivo {EmailTemplatePath}";
                    break;
                case NotSupportedException:
                    error = Global.GetExceptionError(ex);
                    possibleCause = "La operación no es compatible."; if (isLoggingEnabled)
                        suggestedSolution = "Verifique que la operación sea soportada.";
                    logtext = "Operación no compatible  {EmailTemplatePath}";
                    break;
                case System.Security.SecurityException:
                    error = Global.GetExceptionError(ex);
                    possibleCause = "No tiene los permisos necesarios para acceder al archivo.";
                    suggestedSolution = "Asegúrese de tener los permisos necesarios.";
                    logtext = "Permisos insuficientes {EmailTemplatePath}";
                    break;
                default:
                    error = Global.GetExceptionError(ex);
                    possibleCause = "Ocurrió un error desconocido.";
                    suggestedSolution = "Revise los detalles del error.";
                    logtext = "Error desconocido  {EmailTemplatePath}";
                    break;
            }
            if (isLoggingEnabled) logger.LogError(ex, logtext, emailTemplatePath);
            return ("", error, possibleCause, suggestedSolution);
        }
        return (MyRegex().Replace(template, match =>
        {
            string key = match.Groups[1].Value;
            return templateData.TryGetValue(key, out string? value) ? value : match.Value;
        }), "", "", "");
    }

    /// <summary>
    /// Almacena la notificación en la base de datos.
    /// </summary>
    /// <param name="wooPedidoId">Id de pedido WooCommerce WooId</param>true
    /// <param name="tipoNotificacion">Tipo de notificaciones (Error, Advetencia)</param>
    /// <param name="notificacion">Mensaje con el texto a notificar.</param>
    private void SaveNotification(long wooPedidoId, string tipoNotificacion, string notificacion)
    {
        try
        {
            var newNotificacion = new Models.Notificaciones
            {
                WooPedidoId = wooPedidoId,
                TipoNotificacion = tipoNotificacion,
                Notificacion = notificacion
            };

            dbContext.Notificaciones.Add(newNotificacion);
            dbContext.SaveChanges();
            if (isLoggingEnabled)
                logger.LogInformation("Se almacenó la notificación en la base de datos para WooPedidoId: {WooPedidoId}", wooPedidoId);
        }
        catch (Exception ex)
        {
            if (isLoggingEnabled)
                logger.LogError(ex, "Error al almacenar la notificación en la base de datos.");
        }
    }
    // Método para enviar la notificación por correo electrónico
    private void SendNotificationEmail(string subject, string body, bool isHtml = false)
    {
        using SmtpClient smtpClient = new(smtpServer, smtpPort);
        smtpClient.EnableSsl = smtpEnableSSL;
        smtpClient.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);

        MailMessage mailMessage = new()
        {
            From = new MailAddress(senderEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };

        foreach (string recipient in recipients)
        {
            mailMessage.To.Add(recipient);
        }

        try
        {
            smtpClient.Send(mailMessage);
            if (isLoggingEnabled) logger.LogInformation("Notificación enviada correctamente.");
        }
        catch (Exception ex)
        {
            if (isLoggingEnabled) logger.LogError(ex, "Falló el envío de la notificación.");
        }
    }

    [GeneratedRegex(@"\$\{(\w+)\}")]
    private static partial Regex MyRegex();
}

/// <summary>
/// Parametros utilizados por el métoho HandleErrorParams
/// </summary>
public class HandleErrorParams
{
    public string ErrorMessage { get; set; } = "";
    public string PossibleCause { get; set; } = "";
    public string SuggestedSolution { get; set; } = "";
    public string TipoNotificacion { get; set; } = "";
    public long WooPedidoId { get; set; }
}