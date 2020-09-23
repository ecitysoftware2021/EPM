using System;
using System.ComponentModel;

namespace WPFEmpresaEPM.Services.ObjectIntegration
{
    public class ResponseConsultMedida : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int CdError { get; set; }
        public int Contrato { get; set; }
        public string Departamento { get; set; }
        public string Direccion { get; set; }
        public string Identificacion { get; set; }
        public string Localidad { get; set; }
        public string MensajeError { get; set; }
        public string Nombre { get; set; }
        public int NumRestantePagos { get; set; }
        public double SaldoPendiente { get; set; }
        public double ValorTotalFactura { get; set; }
        public double ValorMin { get; set; }

        private double _valorMinimoPago;
        public double ValorMinimoPago
        {
            get
            {
                return _valorMinimoPago;
            }
            set
            {
                _valorMinimoPago = value;
                RaisePropertyChange("ValorMinimoPago");
            }
        }

        public void RaisePropertyChange(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }

    public class DetailsPagoFactura
    {
        public string Referencia { get; set; }
        public string NumeroCuenta { get; set; }
        public string FechaLimite { get; set; }
        public decimal ValorPagar { get; set; }
        public string MensajeError { get; set; }
    }

    public class ResponseConsultFacturaPrepago
    {
        public int CdError { get; set; }
        public string Departamento { get; set; }
        public string Direccion { get; set; }
        public string Dni { get; set; }
        public string ElementoMedicion { get; set; }
        public string Localidad { get; set; }
        public string MensajeError { get; set; }
        public string Nombre { get; set; }
    }

    public class ResponsePayFacturaPrepago
    {
        public double CantidadKwh { get; set; }
        public int CdError { get; set; }
        public double Contribucion { get; set; }
        public string Cupon { get; set; }
        public string ElementoMedicion { get; set; }
        public int Estrato { get; set; }
        public string Fecha { get; set; }
        public string MensajeError { get; set; }
        public string Nit { get; set; }
        public string NomOperAseo { get; set; }
        public string Nombre { get; set; }
        public double OtrosCobros { get; set; }
        public string Pin { get; set; }
        public string PlanFacturacion { get; set; }
        public double SaldoPendiente { get; set; }
        public string Servicio { get; set; }
        public double Subsidio { get; set; }
        public double Tarifa { get; set; }
        public string TelOperAseo { get; set; }
        public int Transaccion { get; set; }
        public string Uso { get; set; }
        public double ValPaTAseo { get; set; }
        public double ValSaldoPenTAseo { get; set; }
        public double ValorAplicadoDeuda { get; set; }
        public double ValorPagado { get; set; }
        public double ValorPrepagoNeto { get; set; }
    }

    public class ResponsePaymedida
    {
        public int CdError { get; set; }
        public int Contrato { get; set; }
        public string Cupon { get; set; }
        public string Fecha { get; set; }
        public string FechaVenciFactura { get; set; }
        public string Identificacion { get; set; }
        public string MensajeError { get; set; }
        public string MensajeUltimaOportunidad { get; set; }
        public string Nit { get; set; }
        public string Nombre { get; set; }
        public double NuevoSaldo { get; set; }
        public int NumRestantePagos { get; set; }
        public string ProductosASupenderse { get; set; }
        public double SaldoAFavor { get; set; }
        public double SaldoAnteriorTotal { get; set; }
        public double SaldoAnteriorVencido { get; set; }
        public double SaldoAnteriorVigente { get; set; }
        public string Servicio { get; set; }
        public int Transaccion { get; set; }
        public double ValorPagado { get; set; }
        public double ValorTotalFactura { get; set; }
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

    public class ResponseConsultPayFactura
    {
        public PaySvcRs PaySvcRs { get; set; }
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

