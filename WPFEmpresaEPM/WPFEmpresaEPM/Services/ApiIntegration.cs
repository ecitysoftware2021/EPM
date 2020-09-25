using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Models;
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
        private static ResponsePaymedida payMedida;
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

                AdminPayPlus.SaveErrorControl("DATA ENVIADA: " + url, "", EError.Aplication, ELevelError.Mild);

                var responseApi = await client.GetAsync(url);

                if (!responseApi.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + responseApi.ReasonPhrase, "", EError.Api, ELevelError.Mild);
                    return null;
                }

                var result = await responseApi.Content.ReadAsStringAsync();
                var responseClient = JsonConvert.DeserializeObject<Response>(result);
                if (responseClient.ResponseCode != ResponseCode.OK)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + responseClient.ResponseMessage, "", EError.Customer, ELevelError.Mild);
                    return null;
                }

                var json = JsonConvert.DeserializeObject<ResponseConsultPayFactura>(responseClient.ResponseData.ToString());

                AdminPayPlus.SaveErrorControl("DATA RECIBIDA: " + json, "", EError.Customer, ELevelError.Mild);

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

                AdminPayPlus.SaveErrorControl("DATA ENVIADA: " + url, "", EError.Aplication, ELevelError.Mild);

                var responseApi = await client.GetAsync(url);

                if (!responseApi.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + responseApi.ReasonPhrase, "", EError.Api, ELevelError.Mild);
                    return null;
                }

                var result = await responseApi.Content.ReadAsStringAsync();
                var responseClient = JsonConvert.DeserializeObject<Response>(result);
                if (responseClient.ResponseCode != ResponseCode.OK)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + responseClient.ResponseMessage, "", EError.Customer, ELevelError.Mild);
                    return null;
                }
                var json = JsonConvert.DeserializeObject<ResponseConsultMedida>(responseClient.ResponseData.ToString());

                AdminPayPlus.SaveErrorControl("DATA RECIBIDA: " + json, "", EError.Customer, ELevelError.Mild);

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

                AdminPayPlus.SaveErrorControl("DATA ENVIADA: " + url, "", EError.Aplication, ELevelError.Mild);

                var responseApi = await client.GetAsync(url);

                if (!responseApi.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + responseApi.ReasonPhrase, "", EError.Api, ELevelError.Mild);
                    return null;
                }

                var result = await responseApi.Content.ReadAsStringAsync();
                var responseClient = JsonConvert.DeserializeObject<Response>(result);
                if (responseClient.ResponseCode != ResponseCode.OK)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + responseClient.ResponseMessage, "", EError.Customer, ELevelError.Mild);
                    return null;
                }
                var json = JsonConvert.DeserializeObject<ResponseConsultFacturaPrepago>(responseClient.ResponseData.ToString());

                AdminPayPlus.SaveErrorControl("DATA RECIBIDA: " + json, "", EError.Customer, ELevelError.Mild);

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

        public static async Task<bool> ReportPay(Transaction ts, string url)
        {
            try
            {
                bool state = false;

                AdminPayPlus.SaveErrorControl("DATA ENVIADA: " + url, "", EError.Aplication, ELevelError.Mild);

                switch (ts.typeTransaction)
                {
                    case ETypeTransaction.PagoFactura:
                        state = await ReportPayPagoFacturaAsync(url);
                        break;
                    case ETypeTransaction.FacturaPrepago:
                        state = await ReportPayFacturaPrepago(url);
                        break;
                    case ETypeTransaction.PagoMedida:
                        state = await ReportPayMedida(url);
                        break;
                }


                ts.Payprepago = payFacturaPrepago;
                ts.Paymedida = payMedida;

                return state;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "ReportPay", ex, ex.ToString());
                return false;
            }
        }

        private static async Task<bool> ReportPayPagoFacturaAsync(string url)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("basseAddresEPM"))
                };

                var responseApi = await client.GetAsync(url);

                if (!responseApi.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + responseApi.ReasonPhrase, "", EError.Api, ELevelError.Mild);
                    return false;
                }

                var result = await responseApi.Content.ReadAsStringAsync();
                var responseClient = JsonConvert.DeserializeObject<Response>(result);
                if (responseClient.ResponseCode != ResponseCode.OK)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + responseClient.ResponseMessage, "", EError.Customer, ELevelError.Mild);
                    return false;
                }
                var json = JsonConvert.DeserializeObject<ResponsePayFactura.RootObject>(responseClient.ResponseData.ToString());

                AdminPayPlus.SaveErrorControl("DATA RECIBIDA: " + json, "", EError.Customer, ELevelError.Mild);

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

                var responseApi = await client.GetAsync(url);

                if (!responseApi.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + responseApi.ReasonPhrase, "", EError.Api, ELevelError.Mild);
                    return false;
                }

                var result = await responseApi.Content.ReadAsStringAsync();
                var responseClient = JsonConvert.DeserializeObject<Response>(result);
                if (responseClient.ResponseCode != ResponseCode.OK)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + responseClient.ResponseMessage, "", EError.Customer, ELevelError.Mild);
                    return false;
                }
                var json = JsonConvert.DeserializeObject<ResponsePaymedida>(responseClient.ResponseData.ToString());

                AdminPayPlus.SaveErrorControl("DATA RECIBIDA: " + json, "", EError.Customer, ELevelError.Mild);

                if (json.CdError == 0 && json != null)
                {
                    payMedida = new ResponsePaymedida
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

                var responseApi = await client.GetAsync(url);

                if (!responseApi.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + responseApi.ReasonPhrase, "", EError.Api, ELevelError.Mild);
                    return false;
                }

                var result = await responseApi.Content.ReadAsStringAsync();
                var responseClient = JsonConvert.DeserializeObject<Response>(result);
                if (responseClient.ResponseCode != ResponseCode.OK)
                {
                    AdminPayPlus.SaveErrorControl("ERROR: " + responseClient.ResponseMessage, "", EError.Customer, ELevelError.Mild);
                    return false;
                }
                var json = JsonConvert.DeserializeObject<ResponsePayFacturaPrepago>(responseClient.ResponseData.ToString());

                AdminPayPlus.SaveErrorControl("DATA RECIBIDA: " + json, "", EError.Customer, ELevelError.Mild);

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
