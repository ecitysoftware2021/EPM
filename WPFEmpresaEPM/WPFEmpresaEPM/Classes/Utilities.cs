using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Classes.Printer;
using WPFEmpresaEPM.Resources;
using WPFEmpresaEPM.Windows;
using Zen.Barcode;
using Encryptor.Ecity.Dll;
using System.Net.NetworkInformation;

namespace WPFEmpresaEPM.Classes
{
    public class Utilities
    {
        #region "Referencias"
        public static Navigation navigator { get; set; }

        private static SpeechSynthesizer speechSynthesizer;

        public static UserControl UCSupport;

        private static ModalWindow modal { get; set; }
        public static int paypadID { get; set; }
        #endregion

        public static string GetConfiguration(string key, bool decodeString = false)
        {
            try
            {
                string value = "";
                AppSettingsReader reader = new AppSettingsReader();
                value = reader.GetValue(key, typeof(String)).ToString();
                if (decodeString)
                {
                    value = EncryptorData(value,false);
                }
                return value;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return string.Empty;
            }
        }

        public static bool ShowModal(string message, EModalType type, bool timer = true)
        {
            bool response = false;
            try
            {
                ModalModel model = new ModalModel
                {
                    Tittle = "Estimado Cliente: ",
                    Messaje = message,
                    TypeModal = type,
                    Timer = timer,
                    ImageModal = @"Images/Backgrounds/modal.png",
                };

                if (type == EModalType.Error)
                {
                    model.ImageModal = @"Images/Backgrounds/modal-error.png";
                }
                else if (type == EModalType.Information)
                {
                    model.ImageModal = @"Images/Backgrounds/modal-info.png";
                }
                else if (type == EModalType.NoPaper)
                {
                    model.ImageModal = @"Images/Backgrounds/modal-info.png";
                }
                else if (type == EModalType.Preload)
                {
                    model.ImageModal = @"Images/Backgrounds/modal-info.png";
                }

                Application.Current.Dispatcher.Invoke(delegate
                {
                    modal = new ModalWindow(model);
                    modal.ShowDialog();

                    if (modal.DialogResult.HasValue && modal.DialogResult.Value)
                    {
                        response = true;
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            GC.Collect();
            return response;
        }

        public static void CloseModal() => Application.Current.Dispatcher.Invoke((Action)delegate
        {
            try
            {
                if (modal != null)
                {
                    modal.Close();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
            }
        });

        public static void RestartApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), AdminPayPlus.DataPayPlus.PayPadConfiguration.ExtrA_DATA.dataComplementary.NAME_APLICATION);
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static string EncryptorData(string plainText, bool encrypt = true, string Key = null)
        {
            try
            {
                if (Key == null)
                {
                    Key = Assembly.GetExecutingAssembly().EntryPoint.DeclaringType.Namespace;
                }

                if (encrypt)
                {
                    return EncryptorEcity.Encrypt(plainText, Key);
                }
                else
                {
                    return EncryptorEcity.Decrypt(plainText, Key);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
                return string.Empty;
            }
        }

        public static string[] ErrorDevice()
        {
            try
            {
                string[] keys = Utilities.ReadFile(@"" + ConstantsResource.PathDevice);

                return keys;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return null;
            }
        }

        public static void PrintVoucherSuccess(Transaction ts)
        {
            try
            {
                CLSPrint objPrint = new CLSPrint();
                objPrint.TOKEN = EncryptorData(string.Concat("Token: ", AdminPayPlus.DataConfiguration.TOKEN_API, "|", "Transaccion: ", ts.TransactionId, "|", "Valor: ", ts.Amount));
                switch (ts.typeTransaction)
                {
                    case ETypeTransaction.PagoFactura:

                        objPrint.FACTURA = ts.Document;
                        objPrint.FECHA_FACTURA = DateTime.Now.ToString("yyyy/MM/dd");
                        objPrint.HORA_FACTURA = DateTime.Now.ToString("hh:mm:ss tt");
                        objPrint.VALOR = String.Format("{0:C0}", ts.Amount);
                        break;

                    case ETypeTransaction.FacturaPrepago:

                        objPrint.APL_DEU = ts.Payprepago.ValorAplicadoDeuda.ToString();
                        objPrint.APL_SA = ts.Payprepago.ValSaldoPenTAseo.ToString();
                        objPrint.CONTRIB = ts.Payprepago.Contribucion.ToString();
                        objPrint.FECHA = ts.Payprepago.Fecha;
                        objPrint.KWH = ts.Payprepago.CantidadKwh.ToString();
                        objPrint.MED = ts.Payprepago.ElementoMedicion;
                        objPrint.OPSA = ts.Payprepago.NomOperAseo;
                        objPrint.OTROS = ts.Payprepago.OtrosCobros.ToString();
                        if (ts.Payprepago.Pin.Length == 20)
                        {
                            objPrint.PIN = ts.Payprepago.Pin.Substring(0, 20).Insert(4, "-").Insert(9, "-").Insert(14, "-").Insert(19, "-");
                        }
                        else
                        {
                            objPrint.PIN = ts.Payprepago.Pin.ToString();
                        }
                        objPrint.PUNTO_DE_VENTA = "1";
                        objPrint.SALD = "";
                        objPrint.SALDO_SA = "";
                        objPrint.SUBCATEGORIA = ts.Payprepago.Estrato.ToString();
                        objPrint.SUB_SID = ts.Payprepago.Subsidio.ToString();
                        objPrint.TARIFA = ts.Payprepago.Tarifa.ToString();
                        objPrint.TEL = ts.Payprepago.TelOperAseo;
                        objPrint.VR = String.Format("{0:C0}", ts.Payprepago.ValorPagado);
                        objPrint.VR_NETO = String.Format("{0:C0}", ts.Payprepago.ValorPrepagoNeto);
                        break;

                    case ETypeTransaction.PagoMedida:

                        objPrint.CONTRATO = ts.Paymedida.Contrato.ToString();
                        objPrint.FECHA_DE_PAGO = ts.Paymedida.Fecha;
                        objPrint.FECHA_DE_VENCI_FACT = ts.Paymedida.FechaVenciFactura;
                        objPrint.IDENTIFICACION_C = ts.Paymedida.Identificacion;
                        objPrint.NUEVO_SALDO = String.Format("{0:C0}", ts.Paymedida.NuevoSaldo);
                        objPrint.NUM_REST_PAGOS = ts.Paymedida.NumRestantePagos.ToString();
                        objPrint.REFERENTE_DE_PAGO = ts.Paymedida.Cupon;
                        objPrint.SALDO_ANTERIOR_TOTAL = String.Format("{0:C0}", ts.Paymedida.SaldoAnteriorTotal);
                        objPrint.SALDO_ANTERIOR_VENCIDO = String.Format("{0:C0}", ts.Paymedida.SaldoAnteriorVencido);
                        objPrint.SALDO_ANTERIOR_VIGENTE = String.Format("{0:C0}", ts.Paymedida.SaldoAnteriorVigente);
                        objPrint.SALDO_FAVOR = String.Format("{0:C0}", ts.Paymedida.SaldoAFavor);
                        objPrint.SERVICIO_A_SUSPENDER = ts.Paymedida.Servicio;
                        objPrint.VALOR_TOTAL_FACT = String.Format("{0:C0}", ts.Paymedida.ValorTotalFactura);
                        objPrint.VALOR_PAGO = String.Format("{0:C0}", ts.Paymedida.ValorPagado);
                        break;
                }

                objPrint.ImprimirComprobante(ts.typeTransaction);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                PrintVoucher(ts);
            }
        }

        public static void PrintVoucher(Transaction transaction)
        {
            try
            {
                if (transaction != null)
                {
                    SolidBrush color = new SolidBrush(Color.Black);
                    Font fontKey = new Font("Arial", 8, System.Drawing.FontStyle.Bold);
                    Font fontValue = new Font("Arial", 8, System.Drawing.FontStyle.Regular);
                    int y = 50;
                    int sum = 25;
                    int x = 150;
                    int xKey = 15;

                    string TOKEN = EncryptorData(string.Concat("Token: ", AdminPayPlus.DataConfiguration.TOKEN_API, "|", "Transaccion: ", transaction.TransactionId, "|", "Valor: ", transaction.Amount));
                    
                    string Boucher = AdminPayPlus.DataPayPlus.PayPadConfiguration.imageS_PATH;
                    string im1 = Path.Combine(Boucher, "Others", "logoEPM1.png");

                    var data = new List<DataPrinter>()
                    {
                        new DataPrinter{ image = im1,  x = 50, y = 0 },

                        new DataPrinter{ brush = color, font = fontKey,   value = "NIT:", x = xKey, y = y+=sum+10 },
                        new DataPrinter{ brush = color, font = fontValue, value = "890 904 996-1" ?? string.Empty, x = x, y = y },

                        new DataPrinter{ brush = color, font = fontKey,   value = "========================================", x = xKey, y = y+=sum },

                        new DataPrinter{ brush = color, font = fontKey,   value = "Transacción", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value = transaction.IdTransactionAPi.ToString(), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value = "Fecha", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value = DateTime.Now.ToString("yyyy/MM/dd"), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value = "Hora", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value = DateTime.Now.ToString("hh:mm:ss"), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value = "Estado", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value = transaction.StatePay, x = x, y = y },

                        new DataPrinter{ brush = color, font = fontKey,   value = "========================================", x = xKey, y = y+=sum },

                        new DataPrinter{ brush = color, font = fontKey,   value =  "Valor a Pagar", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value =  String.Format("{0:C0}", transaction.Amount), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value =  "Valor Ingresado", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value =  String.Format("{0:C0}", transaction.Payment.ValorIngresado), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value =  "Valor Devuelto", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value =  String.Format("{0:C0}", transaction.Payment.ValorDispensado), x = x, y = y },

                        new DataPrinter{ brush = color, font = fontKey,   value = "========================================", x = xKey, y = y+=sum },
                    };

                    if (!transaction.StateReturnMoney)
                    {
                        data.Add(new DataPrinter { brush = color, font = fontValue, value = MessageResource.ReturnMoneyMs1, x = xKey, y = y += sum });
                        data.Add(new DataPrinter { brush = color, font = fontValue, value = MessageResource.ReturnMoneyMs2, x = xKey, y = y += 20 });
                    }

                    data.Add(new DataPrinter { brush = color, font = fontValue, value = MessageResource.PrintMs1, x = xKey, y = y += sum });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = MessageResource.PrintMs2, x = xKey, y = y += 20 });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = "E-city Software", x = 100, y = y += sum });
                    data.Add(new DataPrinter { imageQR = GenerateCode(TOKEN), x = 90, y = y += sum });
                    AdminPayPlus.PrintService.Start(data);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex, ex.ToString());
            }
        }

        public static void UpdateApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = AdminPayPlus.DataPayPlus.PayPadConfiguration.keyS_PATH;
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static System.Drawing.Image GenerateCode(string code)
        {
            CodeQrBarcodeDraw qrcode = BarcodeDrawFactory.CodeQr;

            return qrcode.Draw(code, 1, 2);
        }

        public static decimal RoundValue(decimal Total, bool arriba)
        {
            try
            {
                decimal roundTotal = 0;

                if (arriba)
                {
                    roundTotal = Math.Ceiling(Total / 100) * 100;
                }
                else
                {
                    roundTotal = Math.Floor(Total / 100) * 100;
                }

                return roundTotal;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
                return Total;
            }
        }

        public static bool ValidateModule(decimal module, decimal amount)
        {
            try
            {
                var result = (amount % module);
                return result == 0 ? true : false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
                return false;
            }
        }

        public static T ConverJson<T>(string path)
        {
            T response = default(T);
            try
            {
                using (StreamReader file = new StreamReader(path, Encoding.UTF8))
                {
                    try
                    {
                        var json = file.ReadToEnd().ToString();
                        if (!string.IsNullOrEmpty(json))
                        {
                            response = JsonConvert.DeserializeObject<T>(json);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            return response;
        }

        public static bool IsValidEmailAddress(string email)
        {
            try
            {
                Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,8}$");
                return regex.IsMatch(email);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return false;
            }
        }

        public static void Speak(string text)
        {
            try
            {
                if (GetConfiguration("ActivateSpeak").ToUpper() == "SI")
                {
                    if (speechSynthesizer == null)
                    {
                        speechSynthesizer = new SpeechSynthesizer();
                    }

                    speechSynthesizer.SpeakAsyncCancelAll();
                    speechSynthesizer.SelectVoice("Microsoft Sabina Desktop");
                    speechSynthesizer.SpeakAsync(text);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static string[] ReadFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return File.ReadAllLines(path);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            return null;
        }

        public static string GetIpPublish()
        {
            try
            {
                using (var client = new WebClient())
                {
                    return client.DownloadString(GetConfiguration("UrlGetIp"));
                }
            }
            catch (Exception ex)
            {
                // Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            return GetConfiguration("IpDefoult");
        }

        public static void OpenKeyboard(bool keyBoard_Numeric, TextBox textBox, object thisView, int x = 0, int y = 0)
        {
            try
            {
                WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
                {
                    control = textBox,
                    userControl = thisView is UserControl ? thisView as UserControl : null,
                    eType = (keyBoard_Numeric == true) ? WPKeyboard.Keyboard.EType.Numeric : WPKeyboard.Keyboard.EType.Standar,
                    window = thisView is Window ? thisView as Window : null,
                    X = x,
                    Y = y
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static void CloseKeyboard(object thisView)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    WPKeyboard.Keyboard.CloseKeyboard
                    (
                        user: thisView is UserControl ? thisView as UserControl : null,
                        window: thisView is Window ? thisView as Window : null
                    );
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static bool IsConnectedToInternet()
        {
            try
            {
                string host = "8.8.8.8";

                Ping p = new Ping();

                PingReply reply = p.Send(host, 3000);

                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
            }
            catch (Exception ex)
            { }
            return false;
        }
    }
}
