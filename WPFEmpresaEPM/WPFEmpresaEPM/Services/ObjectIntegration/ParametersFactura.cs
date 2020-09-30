using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEmpresaEPM.Services.ObjectIntegration
{
    public class DetailsPagoFactura
    {
        public string Referencia { get; set; }
        public string NumeroCuenta { get; set; }
        public string FechaLimite { get; set; }
        public decimal ValorPagar { get; set; }
        public string MensajeError { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class ClientApp
    {
        public string Org { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }

    public class SignonRs
    {
        public DateTime ClientDt { get; set; }
        public string CustLangPref { get; set; }
        public ClientApp ClientApp { get; set; }
        public string ServerDt { get; set; }
        public string Language { get; set; }
    }

    public class Status
    {
        public string StatusCode { get; set; }
        public string Severity { get; set; }
        public string StatusDesc { get; set; }
    }

    public class CurAmt
    {
        public string Amt { get; set; }
    }

    public class RemitInfo
    {
        public string CustPayeeId { get; set; }
        public string BillId { get; set; }
        public string BillRefInfo { get; set; }
        public CurAmt CurAmt { get; set; }
        public string BillDt { get; set; }
        public string BillingAcct { get; set; }
    }

    public class PmtInfo
    {
        public RemitInfo RemitInfo { get; set; }
    }

    public class PmtAddRs
    {
        public Status Status { get; set; }
        public string RqUID { get; set; }
        public string AsyncRqUID { get; set; }
        public PmtInfo PmtInfo { get; set; }
    }

    public class PaySvcRs
    {
        public string RqUID { get; set; }
        public string SPName { get; set; }
        public PmtAddRs PmtAddRs { get; set; }
    }

    public class IFX
    {
        public SignonRs SignonRs { get; set; }
        public PaySvcRs PaySvcRs { get; set; }
    }

    public class ResponseDataInvoice
    {
        public List<IFX> IFX { get; set; }
    }



    public class ResponsePayFactura
    {
        public string Code { get; set; }

        public class Status
        {
            public string StatusCode { get; set; }
            public string Severity { get; set; }
            public string StatusDesc { get; set; }
        }

        public class CurAmt
        {
            public string Amt { get; set; }
        }

        public class RemitInfo
        {
            public string BillId { get; set; }
            public string BillRefInfo { get; set; }
            public CurAmt CurAmt { get; set; }
        }

        public class PmtInfo
        {
            public RemitInfo RemitInfo { get; set; }
        }

        public class CurAmt2
        {
            public string Amt { get; set; }
        }

        public class RemitInfo2
        {
            public string BillId { get; set; }
            public string BillRefInfo { get; set; }
            public CurAmt2 CurAmt { get; set; }
        }

        public class PmtInfo2
        {
            public RemitInfo2 RemitInfo { get; set; }
        }

        public class PmtStatus
        {
            public string PmtStatusCode { get; set; }
            public DateTime EffDt { get; set; }
        }

        public class PmtRec
        {
            public string PmtId { get; set; }
            public PmtInfo2 PmtInfo { get; set; }
            public PmtStatus PmtStatus { get; set; }
        }

        public class PmtAddRs
        {
            public Status Status { get; set; }
            public PmtInfo PmtInfo { get; set; }
            public PmtRec PmtRec { get; set; }
        }

        public class PaySvcRs
        {
            public PmtAddRs PmtAddRs { get; set; }
        }

        public class RootObject
        {
            public PaySvcRs PaySvcRs { get; set; }
        }
    }

}
