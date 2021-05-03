using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Models;

namespace WPFEmpresaEPM.Services.Object
{
    public class DataPayPlus
    {
        public bool State { get; set; }

        public bool StateAceptance { get; set; }

        public bool StateDispenser { get; set; }

        public string Message { get; set; }

        public bool StateBalanece { get; set; }

        public bool StateUpload { get; set; }

        public bool StateUpdate { get; set; }

        public bool StateDiminish { get; set; }

        public object ListImages { get; set; }

        public int ContTransactionsNotific { get; set; }

        public int ContTransactions { get; set; }
        public PaypadConfiguration PayPadConfiguration { get; set; }
    }

    public class PaypadConfiguration
    {
        public int paypaD_ID { get; set; }
        public string bilL_PORT { get; set; }
        public string coiN_PORT { get; set; }
        public string unifieD_PORT { get; set; }
        public string printeR_PORT { get; set; }
        public int printeR_BAUD_RATE { get; set; }
        public string dispenseR_CONFIGURATION { get; set; }
        public string localdB_PATH { get; set; }
        public string publicitY_TIMER { get; set; }
        public string generiC_TIMER { get; set; }
        public string modaL_TIMER { get; set; }
        public string inactivitY_TIMER { get; set; }
        public string extrA_DATA { get; set; }
        public ExtraData ExtrA_DATA { get; set; }
        public bool enablE_CARD { get; set; }
        public bool enablE_VALIDATE_PERIPHERALS { get; set; }
        public bool iS_UNIFIED { get; set; }
        public bool iS_PRODUCTION { get; set; }
        public string keyS_PATH { get; set; }
        public string imageS_PATH { get; set; }
        public string scanneR_PORT { get; set; }

        public void DeserializarExtraData()
        {
            try
            {
                ExtrA_DATA = JsonConvert.DeserializeObject<ExtraData>(extrA_DATA);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }
    }

    public class ExtraData
    {
        public DataComplementary dataComplementary { get; set; }
        public DataIntegration dataIntegration { get; set; }
    }

    public class DataComplementary
    {
        public string DurationAlert { get; set; }
        public string NAME_PAYPAD { get; set; }
        public string LAST_NAME_PAYPAD { get; set; }
        public string NAME_APLICATION { get; set; }
        public string PathRedeban { get;  set; }
        public string ValorMinPrepago { get;  set; }
        public string ValorMaxPrepago { get;  set; }
        public string NumbersPhone { get;  set; }
        public string NumbersSerial { get;  set; }
    }

    public class DataIntegration
    {
        public string basseAddresEPM { get; set; }
        public string ConsultarFactura { get; set; }
        public string ValidarPagoMedida { get; set; }
        public string ValidarCompra { get; set; }
        public string RegistrarPagoFactura { get; set; }
        public string RegistrarPagoMedida { get; set; }
        public string RegistarCompraEnergia { get; set; }
    }
}
