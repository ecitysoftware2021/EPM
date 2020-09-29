using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WPFEmpresaEPM.Classes;

namespace WPFEmpresaEPM.Services
{
    public  class ApiIntegration2
    {
        #region "Referencias"
        private HttpClient client;
        #endregion

        #region "Constructor"
        public ApiIntegration2()
        {
            try
            {
                client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddresEPM"))
                };
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Métodos"
        #endregion
    }
}
