using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public decimal SaldoPendiente { get; set; }
        public decimal ValorTotalFactura { get; set; }
        public decimal ValorMin { get; set; }

        private decimal _valorMinimoPago;
        public decimal ValorMinimoPago
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
        public decimal NuevoSaldo { get; set; }
        public int NumRestantePagos { get; set; }
        public string ProductosASupenderse { get; set; }
        public decimal SaldoAFavor { get; set; }
        public decimal SaldoAnteriorTotal { get; set; }
        public decimal SaldoAnteriorVencido { get; set; }
        public decimal SaldoAnteriorVigente { get; set; }
        public string Servicio { get; set; }
        public int Transaccion { get; set; }
        public decimal ValorPagado { get; set; }
        public decimal ValorTotalFactura { get; set; }
    }
}
