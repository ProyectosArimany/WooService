using System;

namespace WooService.Utils
{
    /// <summary>
    /// Clase [Global]. Funciones manejo de cadenas de caracteres y otros propositos.
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// Reemplaza todas las ocurrencias del caracter [oldmetachar] que encuentre,
        /// en la cadena [str] con el valor [newmetachar].
        /// </summary>
        /// <param name="str">Cadena de caracteres donde se realizará el reemplazo</param>
        /// <param name="oldmetachar">Caracter a buscar y sustituir en [str]</param>
        /// <param name="newmetachar">Nuevo caracter que sustituirá a [olmetacar] en [str]</param>
        /// <returns>Cadena de caracters ya con susitución.</returns>
        public static string ChangeMetaChar(String str, String oldmetachar = "*", String newmetachar = "%")
        {
            if (String.IsNullOrWhiteSpace(str)) str = newmetachar;
            else str = str.Replace(oldmetachar, newmetachar);
            return str;
        }

        /// <summary>
        /// Comprueba si la cadena está vacía o contiene valor null.
        /// </summary>
        /// <param name="str">Cadena a comprobar</param>
        /// <returns>Devuelta true si la cadena está vacía o es nula.  false de lo contrario</returns>
        public static bool StrIsBlank(string str) => string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// Concatena los textos de error en la excepción [ex]
        /// </summary>
        /// <param name="ex">Exception de donde se extraeran los mensajes de error</param>
        /// <returns>Cadena de caracteres con los mensajes de error separados por el caracter de fin de línea.</returns>
        public static string GetExceptionError(Exception ex)
        {
            string fullMsgError = ex.Message;
            Exception? innerEx = ex.InnerException;
            while (innerEx != null)
            {
                fullMsgError += $" {innerEx.Message}";
                innerEx = innerEx.InnerException;
            }
            return fullMsgError;
        }

        /// <summary>
        /// Escapa el caracte comilla doble con el formato \" para 
        /// crear un texto JSON.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ScapeQuotation(string str)
        {
            str = string.IsNullOrWhiteSpace(str) ? "" : str.Replace("\"", "\\\"");
            str = string.IsNullOrWhiteSpace(str) ? "" : str.Replace("/", "\\/");
            return str;
        }

    }
}

