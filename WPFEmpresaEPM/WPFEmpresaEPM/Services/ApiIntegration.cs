using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Services.ObjectIntegration;

namespace WPFEmpresaEPM.Services
{
    public class ApiIntegration
    {
        #region "Referencias"
        private static DetailsPagoFactura details;
        private static ResponseConsultMedida medida;
        private static ResponseConsultFacturaPrepago prepago;
        private static ResponsePayFacturaPrepago payFacturaPrepago;
        private static ResponsePayMedida payMedida;
        private static ResponsePayFactura payFactura;
        #endregion

        #region "Métodos"
        public static async Task<DetailsPagoFactura> SearchPagoFactura(string url)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddressConsulta"))
                };

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ResponseConsultPayFactura>(result);

                if (json.PaySvcRs.PmtAddRs.Status.StatusCode == "0" && json != null)
                {
                    details = new DetailsPagoFactura
                    {
                        FechaLimite = json.PaySvcRs.PmtAddRs.PmtInfo.RemitInfo.BillDt,
                        NumeroCuenta = json.PaySvcRs.PmtAddRs.PmtInfo.RemitInfo.BillingAcct,
                        Referencia = json.PaySvcRs.PmtAddRs.PmtInfo.RemitInfo.BillId,
                        ValorPagar = Convert.ToDecimal(json.PaySvcRs.PmtAddRs.PmtInfo.RemitInfo.CurAmt.Amt)
                    };
                    return details;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "SearchPagoFactura", ex, ex.ToString());
            }
            return null;
        }

        public static async Task<ResponseConsultMedida> SearchPagoMedida(string controller, string value)
        {
            try
            {

                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddressConsulta"))
                };
                var url = string.Format("{0}{1}", controller, value);
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    medida = new ResponseConsultMedida
                    {
                        MensajeError = "No se pudo realizar la consulta, intente de nuevo mas tarde, Gracias."
                    };
                    return medida;
                }

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ResponseConsultMedida>(result);

                if (json.CdError == 0 && json != null)
                {
                    medida = new ResponseConsultMedida
                    {
                        CdError = json.CdError,
                        Departamento = json.Departamento,
                        Direccion = json.Direccion,
                        Contrato = json.Contrato,
                        Identificacion = json.Identificacion,
                        Localidad = json.Localidad,
                        MensajeError = json.MensajeError,
                        Nombre = json.Nombre,
                        NumRestantePagos = json.NumRestantePagos,
                        SaldoPendiente = json.SaldoPendiente,
                        ValorMinimoPago = json.ValorMinimoPago,
                        ValorTotalFactura = json.ValorTotalFactura,
                        ValorMin = json.ValorMinimoPago
                    };
                    return medida;
                }
                else if (json.CdError != 0 && json != null)
                {
                    medida = new ResponseConsultMedida
                    {
                        MensajeError = json.MensajeError
                    };
                    return medida;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<ResponseConsultFacturaPrepago> SearchFacturaPrepago(string controller, string value)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddressConsulta"))
                };
                var url = string.Format("{0}{1}", controller, value);
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    prepago = new ResponseConsultFacturaPrepago
                    {
                        MensajeError = "No se pudo realizar la consulta, intente de nuevo mas tarde, Gracias."
                    };
                    return prepago;
                }

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ResponseConsultFacturaPrepago>(result);

                if (json.CdError == 0 && json != null)
                {
                    prepago = new ResponseConsultFacturaPrepago
                    {
                        CdError = json.CdError,
                        Departamento = json.Departamento,
                        Direccion = json.Direccion,
                        Localidad = json.Localidad,
                        MensajeError = json.MensajeError,
                        Nombre = json.Nombre,
                        Dni = json.Dni,
                        ElementoMedicion = json.ElementoMedicion
                    };
                    return prepago;
                }
                else if (json.CdError != 0 && json != null)
                {
                    prepago = new ResponseConsultFacturaPrepago
                    {
                        MensajeError = json.MensajeError
                    };
                    return prepago;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<ResponsePayFactura> ReportPayPagoFactura(string controller, string value)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddressConsulta"))
                };
                var url = string.Format("{0}{1}", controller, value);
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ResponsePayFactura.RootObject>(result);

                if (json.PaySvcRs.PmtAddRs.Status.StatusCode == "0" && json != null)
                {
                    payFactura = new ResponsePayFactura
                    {
                        Code = "Ok"
                    };
                    return payFactura;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<ResponsePayMedida> ReportPayMedida(string controller, string value)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddressConsulta"))
                };
                var url = string.Format("{0}{1}", controller, value);
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ResponsePayMedida>(result);

                if (json.CdError == 0 && json != null)
                {
                    payMedida = new ResponsePayMedida
                    {
                        CdError = json.CdError,
                        Cupon = json.Cupon,
                        Fecha = json.Fecha,
                        MensajeError = json.MensajeError,
                        Nit = json.Nit,
                        Nombre = json.Nombre,
                        Servicio = json.Servicio,
                        Transaccion = json.Transaccion,
                        ValorPagado = json.ValorPagado,
                        Contrato = json.Contrato,
                        FechaVenciFactura = json.FechaVenciFactura,
                        Identificacion = json.Identificacion,
                        MensajeUltimaOportunidad = json.MensajeUltimaOportunidad,
                        NuevoSaldo = json.NuevoSaldo,
                        NumRestantePagos = json.NumRestantePagos,
                        ProductosASupenderse = json.ProductosASupenderse,
                        SaldoAFavor = json.SaldoAFavor,
                        SaldoAnteriorTotal = json.SaldoAnteriorTotal,
                        SaldoAnteriorVencido = json.SaldoAnteriorVencido,
                        SaldoAnteriorVigente = json.SaldoAnteriorVigente,
                        ValorTotalFactura = json.ValorTotalFactura
                    };
                    return payMedida;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<ResponsePayFacturaPrepago> ReportPayFacturaPrepago(string controller, string value)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddressConsulta"))
                };
                var url = string.Format("{0}{1}", controller, value);
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ResponsePayFacturaPrepago>(result);

                if (json.CdError == 0 && json != null)
                {
                    payFacturaPrepago = new ResponsePayFacturaPrepago
                    {
                        CantidadKwh = json.CantidadKwh,
                        CdError = json.CdError,
                        Contribucion = json.Contribucion,
                        Cupon = json.Cupon,
                        ElementoMedicion = json.ElementoMedicion,
                        Estrato = json.Estrato,
                        Fecha = json.Fecha,
                        MensajeError = json.MensajeError,
                        Nit = json.Nit,
                        Nombre = json.Nombre,
                        NomOperAseo = json.NomOperAseo,
                        OtrosCobros = json.OtrosCobros,
                        Pin = json.Pin,
                        PlanFacturacion = json.PlanFacturacion,
                        SaldoPendiente = json.SaldoPendiente,
                        Servicio = json.Servicio,
                        Subsidio = json.Subsidio,
                        Tarifa = json.Tarifa,
                        TelOperAseo = json.TelOperAseo,
                        Transaccion = json.Transaccion,
                        Uso = json.Uso,
                        ValorAplicadoDeuda = json.ValorAplicadoDeuda,
                        ValorPagado = json.ValorPagado,
                        ValorPrepagoNeto = json.ValorPrepagoNeto,
                        ValPaTAseo = json.ValPaTAseo,
                        ValSaldoPenTAseo = json.ValSaldoPenTAseo
                    };
                    return payFacturaPrepago;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
