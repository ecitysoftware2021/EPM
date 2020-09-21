using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFEmpresaEPM.Models;

namespace WPFEmpresaEPM.Services
{
    public class Request
    {
        public string I_CANAL { get; set; }
        public string I_DIRECCIONIP { get; set; }
        public string I_ENTIDADORIGEN { get; set; }
        public string I_TERMINAL { get; set; }
        public string I_TIMESTAMP { get; set; }
        public string I_LENGUAJE { get; set; }
        public string I_NUMDOC { get; set; }
        public string I_TIPDOC { get; set; }
        public string I_CTANRO { get; set; }
        public string I_CODSIS { get; set; }
        public string I_CODPRO { get; set; }
        public string I_REFTRN { get; set; }
        public string I_TOKTRN { get; set; }
        public string I_VLRTRN { get; set; }
        public string CodOTP { get; set; }
        public string Text_QR { get; set; }

        public object Objeto { get; set; }
    }

    public class RequestSearch
    {
        public string codigoempresa { get; set; }
        public string usuariows { get; set; }
        public string token { get; set; }
        public string identificacion { get; set; }
        public int matriculainicial { get; set; }
        public string nombreinicial { get; set; }
        public string semilla { get; set; }
    }
}
