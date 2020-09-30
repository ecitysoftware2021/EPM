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
}
