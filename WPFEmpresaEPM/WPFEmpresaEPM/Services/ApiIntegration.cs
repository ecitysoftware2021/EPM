using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
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
        private static ResponseDataInvoice details;
        private static ResponseConsultMedida medida;
        private static ResponseConsultFacturaPrepago prepago;
        private static ResponsePayFacturaPrepago payFacturaPrepago;
        private static ResponsePaymedida payMedida;
        #endregion

        public ApiIntegration()
        {
        }

        #region "Métodos"
        public static async Task<ResponseDataInvoice> SearchPagoFactura(string reference)
        {
            try
            {
                InvoiceSearchRequest request = new InvoiceSearchRequest
                {
                    reference = reference,
                    typeSearch = 0
                };
                AdminPayPlus.SaveErrorControl($"Request Consulta Factura: reference: {reference}", "", EError.Aplication, ELevelError.Mild);

                var response = CallApiEpm(request, Utilities.GetConfiguration("ConsultarFactura"));
                if (string.IsNullOrEmpty(response))
                {
                    AdminPayPlus.SaveErrorControl("ERROR Consulta Factura: Data Null o Vacía", "", EError.Api, ELevelError.Mild);
                    return null;
                }
                var responseClient = JsonConvert.DeserializeObject<Response>(response);
                if (responseClient.ResponseCode != ResponseCode.OK)
                {
                    AdminPayPlus.SaveErrorControl("ERROR Consulta Factura: " + responseClient.ResponseMessage, "", EError.Api, ELevelError.Mild);
                    return null;
                }

                var json = JsonConvert.DeserializeObject<ResponseDataInvoice>(responseClient.ResponseData.ToString());

                AdminPayPlus.SaveErrorControl("Response Consulta Factura: " + json, "", EError.Api, ELevelError.Mild);

                if (json != null && json.IFX.Count > 0 && json.IFX[0].PaySvcRs.PmtAddRs.Status.StatusCode == "0")
                {
                    details = new ResponseDataInvoice
                    {
                        IFX = json.IFX
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

        public static async Task<ResponseConsultMedida> SearchPagoMedida(int contract = -1, string document = null)
        {
            try
            {
                PayRequest request = new PayRequest
                {
                    contract = contract,
                    document = document
                };
                AdminPayPlus.SaveErrorControl($"Request Consulta Pago Medida: contract: {contract}, document: {document}", "", EError.Aplication, ELevelError.Mild);


                var result = CallApiEpm(request, Utilities.GetConfiguration("ValidarPagoMedida"));
                if (string.IsNullOrEmpty(result))
                {
                    AdminPayPlus.SaveErrorControl("ERROR Consulta Pago Medida: Data Null o Vacía", "", EError.Api, ELevelError.Mild);
                    return null;
                }
                var responseClient = JsonConvert.DeserializeObject<Response>(result);
                if (responseClient.ResponseCode != ResponseCode.OK)
                {
                    AdminPayPlus.SaveErrorControl("ERROR Pago Medida: " + responseClient.ResponseMessage, "", EError.Api, ELevelError.Mild);
                    return null;
                }
                var json = JsonConvert.DeserializeObject<ResponseConsultMedida>(responseClient.ResponseData.ToString());

                AdminPayPlus.SaveErrorControl("Response Pago Medida: " + json, "", EError.Api, ELevelError.Mild);

                if (json != null && json.CdError == 0)
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

        public static async Task<ResponseConsultFacturaPrepago> SearchFacturaPrepago(decimal payValue, string medidor)
        {
            try
            {

                InvoicePayRequest request = new InvoicePayRequest
                {
                    payValue = payValue,
                    reference = medidor
                };

                AdminPayPlus.SaveErrorControl($"Request Consulta Factura Prepago: payValue: {payValue}, medidor: {medidor}", "", EError.Aplication, ELevelError.Mild);

                var result = CallApiEpm(request, Utilities.GetConfiguration("ValidarCompra"));
                if (string.IsNullOrEmpty(result))
                {
                    AdminPayPlus.SaveErrorControl("ERROR Consulta Factura Prepago: Data Null o Vacía", "", EError.Api, ELevelError.Mild);
                    return null;
                }
                var responseClient = JsonConvert.DeserializeObject<Response>(result);
                if (responseClient.ResponseCode != ResponseCode.OK)
                {
                    AdminPayPlus.SaveErrorControl("ERROR Consulta Factura Prepago: " + responseClient.ResponseMessage, "", EError.Api, ELevelError.Mild);
                    return null;
                }
                var json = JsonConvert.DeserializeObject<ResponseConsultFacturaPrepago>(responseClient.ResponseData.ToString());

                AdminPayPlus.SaveErrorControl("Response Consulta Factura Prepago: " + json, "", EError.Api, ELevelError.Mild);

                if (json != null && json.CdError == 0)
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

        public static async Task<bool> ReportPay(Transaction ts, object request)
        {
            try
            {
                bool state = false;
                switch (ts.typeTransaction)
                {
                    case ETypeTransaction.PagoFactura:
                        state = await ReportPayPagoFacturaAsync((InvoicePayRequest)request);
                        break;
                    case ETypeTransaction.FacturaPrepago:
                        state = await ReportPayFacturaPrepago((PrepaidPayRequest)request);
                        break;
                    case ETypeTransaction.PagoMedida:
                        state = await ReportPayMedida((PayRequest)request);
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

        private static async Task<bool> ReportPayPagoFacturaAsync(InvoicePayRequest request)
        {
            try
            {

                AdminPayPlus.SaveErrorControl($"Request Notificar Pago Factura: {JsonConvert.SerializeObject(request)}", "", EError.Aplication, ELevelError.Mild);
                var result = CallApiEpm(request, Utilities.GetConfiguration("RegistrarPagoFactura"));
                if (string.IsNullOrEmpty(result))
                {
                    AdminPayPlus.SaveErrorControl("ERROR Notificar Pago Factura: Data Null o Vacía", "", EError.Api, ELevelError.Mild);
                    return false;
                }
                var responseClient = JsonConvert.DeserializeObject<Response>(result);
                if (responseClient.ResponseCode != ResponseCode.OK)
                {
                    AdminPayPlus.SaveErrorControl("ERROR Notificar Pago Factura: " + responseClient.ResponseMessage, "", EError.Api, ELevelError.Mild);
                    return false;
                }
                var json = JsonConvert.DeserializeObject<ResponsePayFactura.RootObject>(responseClient.ResponseData.ToString());

                AdminPayPlus.SaveErrorControl("Response Notificar Pago Factura: " + json, "", EError.Api, ELevelError.Mild);

                if (json != null && json.PaySvcRs.PmtAddRs.Status.StatusCode == "0")
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

        private static async Task<bool> ReportPayMedida(PayRequest request)
        {
            try
            {
                AdminPayPlus.SaveErrorControl($"Request Notificar Pago a tu Medida: {JsonConvert.SerializeObject(request)}", "", EError.Aplication, ELevelError.Mild);
                var result = CallApiEpm(request, Utilities.GetConfiguration("RegistrarPagoMedida"));
                if (string.IsNullOrEmpty(result))
                {
                    AdminPayPlus.SaveErrorControl("ERROR Notificar Pago a tu Medida: Data Null o Vacía", "", EError.Api, ELevelError.Mild);
                    return false;
                }
                var responseClient = JsonConvert.DeserializeObject<Response>(result);
                if (responseClient.ResponseCode != ResponseCode.OK)
                {
                    AdminPayPlus.SaveErrorControl("ERROR Notificar Pago a tu Medida: " + responseClient.ResponseMessage, "", EError.Api, ELevelError.Mild);
                    return false;
                }
                var json = JsonConvert.DeserializeObject<ResponsePaymedida>(responseClient.ResponseData.ToString());

                AdminPayPlus.SaveErrorControl("Response Notificar Pago a tu Medida: " + json, "", EError.Api, ELevelError.Mild);

                if (json != null && json.CdError == 0)
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

        private static async Task<bool> ReportPayFacturaPrepago(PrepaidPayRequest request)
        {
            try
            {
                AdminPayPlus.SaveErrorControl($"Request Notificar Factura Prepago: {JsonConvert.SerializeObject(request)}", "", EError.Aplication, ELevelError.Mild);
                var result = CallApiEpm(request, Utilities.GetConfiguration("RegistarCompraEnergia"));
                if (string.IsNullOrEmpty(result))
                {
                    AdminPayPlus.SaveErrorControl("ERROR Notificar Factura Prepago: Data Null o Vacía", "", EError.Api, ELevelError.Mild);
                    return false;
                }
                var responseClient = JsonConvert.DeserializeObject<Response>(result);
                if (responseClient.ResponseCode != ResponseCode.OK)
                {
                    AdminPayPlus.SaveErrorControl("ERROR Notificar Factura Prepago: " + responseClient.ResponseMessage, "", EError.Customer, ELevelError.Mild);
                    return false;
                }
                var json = JsonConvert.DeserializeObject<ResponsePayFacturaPrepago>(responseClient.ResponseData.ToString());

                AdminPayPlus.SaveErrorControl("Response Notificar Factura Prepago: " + json, "", EError.Customer, ELevelError.Mild);

                if (json != null && json.CdError == 0)
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

        public static string CallApiEpm(object data, string url)
        {
            try
            {
                var client = new RestClient(string.Format(Utilities.GetConfiguration("basseAddresEPM"), url));
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(data), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "CallApiEpm", ex, ex.ToString());
                return null;
            }
        }
        #endregion
    }
    public class InvoiceSearchRequest
    {
        public short typeSearch { get; set; }
        public string reference { get; set; }
    }
    public class InvoicePayRequest
    {
        public decimal payValue { get; set; }
        public string reference { get; set; }
    }
    public class PrepaidPayRequest : InvoicePayRequest
    {
        public int transactionID { get; set; }
    }
    public class PayRequest : PrepaidPayRequest
    {
        public int contract { get; set; }
        public string document { get; set; }
    }
}
