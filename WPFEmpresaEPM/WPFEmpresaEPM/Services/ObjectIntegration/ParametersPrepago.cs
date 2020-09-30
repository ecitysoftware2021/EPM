using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEmpresaEPM.Services.ObjectIntegration
{
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
        public decimal Valor { get; set; }
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
}
