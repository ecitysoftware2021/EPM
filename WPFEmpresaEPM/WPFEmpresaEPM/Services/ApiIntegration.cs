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
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddresEPM"))
                };

                AdminPayPlus.SaveErrorControl("DATA ENVIADA: " + url, "", EError.Api, ELevelError.Mild);

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + response.ReasonPhrase, "", EError.Api, ELevelError.Mild);
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ResponseConsultPayFactura>(result);

                AdminPayPlus.SaveErrorControl("DATA RECIBIDA: " + json, "", EError.Api, ELevelError.Mild);

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

        public static async Task<ResponseConsultMedida> SearchPagoMedida(string url)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddresEPM"))
                };

                AdminPayPlus.SaveErrorControl("DATA ENVIADA: " + url, "", EError.Api, ELevelError.Mild);

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + response.ReasonPhrase, "", EError.Api, ELevelError.Mild);
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ResponseConsultMedida>(result);

                AdminPayPlus.SaveErrorControl("DATA RECIBIDA: " + json, "", EError.Api, ELevelError.Mild);

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
                return null;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "SearchPagoMedida", ex, ex.ToString());
                return null;
            }
        }

        public static async Task<ResponseConsultFacturaPrepago> SearchFacturaPrepago(string url)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddresEPM"))
                };

                AdminPayPlus.SaveErrorControl("DATA ENVIADA: " + url, "", EError.Api, ELevelError.Mild);

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + response.ReasonPhrase, "", EError.Api, ELevelError.Mild);
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ResponseConsultFacturaPrepago>(result);

                AdminPayPlus.SaveErrorControl("DATA RECIBIDA: " + json, "", EError.Api, ELevelError.Mild);

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
                return null;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "SearchFacturaPrepago", ex, ex.ToString());
                return null;
            }
        }

        public static async Task<bool> ReportPay(ETypeTransaction type, string url)
        {
            AdminPayPlus.SaveErrorControl("DATA ENVIADA: " + url, "", EError.Api, ELevelError.Mild);

            switch (type)
            {
                case ETypeTransaction.PagoFactura:
                    return await ReportPayPagoFacturaAsync(url);
                case ETypeTransaction.FacturaPrepago:
                    return await ReportPayFacturaPrepago(url);
                case ETypeTransaction.PagoMedida:
                    return await ReportPayMedida(url);
            }
            return false;
        }

        private static async Task<bool> ReportPayPagoFacturaAsync(string url)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddresEPM"))
                };

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + response.ReasonPhrase, "", EError.Api, ELevelError.Mild);
                    return false;
                }

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ResponsePayFactura.RootObject>(result);

                AdminPayPlus.SaveErrorControl("DATA RECIBIDA: " + json, "", EError.Api, ELevelError.Mild);

                if (json.PaySvcRs.PmtAddRs.Status.StatusCode == "0" && json != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "ReportPayPagoFacturaAsync", ex, ex.ToString());
                return false;
            }
        }

        private static async Task<bool> ReportPayMedida(string url)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddresEPM"))
                };

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + response.ReasonPhrase, "", EError.Api, ELevelError.Mild);
                    return false;
                }

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ResponsePayMedida>(result);

                AdminPayPlus.SaveErrorControl("DATA RECIBIDA: " + json, "", EError.Api, ELevelError.Mild);

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
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "ReportPayMedida", ex, ex.ToString());
                return false;
            }
        }

        private static async Task<bool> ReportPayFacturaPrepago(string url)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddresEPM"))
                };

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + response.ReasonPhrase, "", EError.Api, ELevelError.Mild);
                    return false;
                }

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ResponsePayFacturaPrepago>(result);

                AdminPayPlus.SaveErrorControl("DATA RECIBIDA: " + json, "", EError.Api, ELevelError.Mild);

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
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "ReportPayFacturaPrepago", ex, ex.ToString());
                return false;
            }
        }
        #endregion
    }
}
