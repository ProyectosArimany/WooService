
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using WooService.Utils;

namespace WooService.Providers;

/// <summary>
/// La clase  SMTPService define los metodos necesarios,
/// para enviar correos electrónicos utilizando el protocolo SMTP.
/// </summary>
/// <param name="host">Nombre del Servidor o IP de correo</param>
/// <param name="port">Puerto del servicio</param>
/// <param name="emailUser">Cuenta para conectar al servidor</param>
/// <param name="emailPassword">Contraseña de la cuenta de correo para conectar al servidor</param>
/// <param name="from">Reminete del correo a enviar.</param>
/// <param name="to">Destinatario del correo a envir.</param>
/// <param name="subject">Asunto del correo electrónico</param>
/// <param name="body">cuerpo del correo electrónico</param>
public class SMTPService(String host, int port, string emailUser, string emailPassword,
                         String from, String to, String subject, string body)
{
    /// <summary>
    /// Instancia una la clase MailMessage para preparar
    /// un mensaje de correo electrónico
    /// </summary>
    readonly MailMessage message = new(
        from,
        to,
        subject,
        body);

    /// <summary>
    /// Adjuntar el objeto [dataData] al mensaje [message]
    /// </summary>
    Attachment? dataData = null;

    /// <summary>
    /// Crea un adjunto utilizando un archivo, para
    /// anexarlo al mensaje [message]
    /// </summary>
    /// <param name="fileName">Nombre del archivo a adjuntar, full path</param>
    /// <returns>String vacio o mensaje de error si falla en la operación.</returns>
    public string AttachFile(String fileName)
    {
        if (String.IsNullOrEmpty(fileName))
        {
            return "";
        }


        try
        {
            // Intentar cargar archivo en memoria.
            using FileStream fileStream = new(fileName, FileMode.Open, FileAccess.Read);
            // Crear el adjunto.
            ContentType contentType = new(MediaTypeNames.Application.Octet);
            dataData = new(fileStream, contentType);
            ContentDisposition? disposition = dataData.ContentDisposition;
            if (disposition is not null)
            {
                fileName = Path.GetFileName(fileName);
                disposition.FileName = fileName;
            }
            message.Attachments.Add(dataData);
            return "";
        }
        catch (Exception ex)
        {

            return Global.GetExceptionError(ex);
        }
    }


    /// <summary>
    /// Envia un mensaje utilizando los datos del mensaje [message]
    /// </summary>
    /// <param name="enableSSL">True si se debe especificar que la conexión es cifrada.</param>
    /// <returns>String vacio o un mensaje de error si falla la operación.</returns>
    public string SendEmail(bool enableSSL = true)
    {
        
        //Send the message.
        NetworkCredential credentials = new(emailUser, emailPassword);
        SmtpClient client = new()
        {
            // Add credentials if the SMTP server requires them.
            Host = host,
            Port = port,
            Credentials = credentials,
            EnableSsl = enableSSL
        };

        string result = IsValidEmail(emailUser);
        if (result != "")
            return emailUser + "\n" + result;
        result = IsValidEmail(from);
        if (result != "")
            return from + "\n" + result;
        result = IsValidEmail(to);
        if (result != "")
            return to + "\n" + result;

        try
        {
            message.IsBodyHtml = true;
            client.Send(message);
            this.dataData?.Dispose();

            return "";
        }
        catch (Exception ex)
        {
            return Global.GetExceptionError(ex);
        }
    }

    public static string IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "Se debe especificar una dirección de correo electrónico";

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                static string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new System.Globalization.IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return Global.GetExceptionError(e);
            }
            catch (ArgumentException e)
            {
                return Global.GetExceptionError(e);
            }

            try
            {
                bool isAddrOk = Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                if (isAddrOk)
                    return "";
                else
                    return "La dirección de correo electrónico no es válida";
            }
            catch (RegexMatchTimeoutException e)
            {
                return Global.GetExceptionError(e);
            }
        }

}