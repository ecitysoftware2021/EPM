using System;
using System.Text;
using WPFEmpresaEPM.DataModel;
using WPFEmpresaEPM.Resources;

namespace WPFEmpresaEPM.Classes
{
    public static class Error
    {
        public static void SaveLogError(string metod, string classError, Exception exeption, string description = null)
        {
            try
            {
                AdminPayPlus.SaveLog(new ERROR_LOG
                {
                    DATE = DateTime.Now,
                    DESCRIPTION = description,
                    MESSAGE_ERROR = Exception(exeption),
                    NAME_CLASS = classError,
                    NAME_FUNCTION = metod,
                    TYPE = 1,
                }, ELogType.Error);
            }
            catch (Exception)
            {
                //Utilities.ShowModal(MessageResource.Error, EModalType.Error);
            }
        }

        /// <summary>
        /// Metodo para recorrer los errores encontrados
        /// </summary>
        /// <param name="exception">Objecto Exception</param>
        /// <returns>Mensaje con el error</returns>
        private static string Exception(Exception exception)
        {
            var sb = new StringBuilder();
            var counter = 1;
            //Recorrer todas las excepciones del object.
            while (exception != null && counter <= 20)
            {
                sb.AppendLine(string.Format(CommonResource.ErrorLevel, counter, counter));

                sb.AppendLine(string.Format(CommonResource.ErrorMessage, counter, exception.Message));

                sb.AppendLine(string.Format(CommonResource.ErrorSource, counter, exception.Source));

                sb.AppendLine(string.Format(CommonResource.ErrorTargetSite, counter, exception.TargetSite));

                sb.Append(string.Format(CommonResource.ErrorStackTrace, counter, exception.StackTrace));

                exception = exception.InnerException;
                counter++;
            }
            //Agregar error
            return sb.ToString();
        }
    }
}
