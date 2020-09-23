﻿using Newtonsoft.Json;
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
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Resources;
using WPFEmpresaEPM.Windows;

namespace WPFEmpresaEPM.Classes
{
    public class Utilities
    {
        #region "Referencias"
        public static Navigation navigator { get; set; }

        private static SpeechSynthesizer speechSynthesizer;

        private static ModalWindow modal { get; set; }
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
                    value = Encryptor.Decrypt(value);
                }
                return value;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return string.Empty;
            }
        }

        public static bool ShowModal(string message, EModalType type, bool timer = false)
        {
            bool response = false;
            try
            {
                ModalModel model = new ModalModel
                {
                    Tittle = "Estimado Cliente: ",
                    Messaje = message,
                    Timer = timer,
                    TypeModal = type,
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
                    si.FileName = Path.Combine(Directory.GetCurrentDirectory(), GetConfiguration("NAME_APLICATION"));
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

        public static void UpdateApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = GetConfiguration("APLICATION_UPDATE");
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

        public static void PrintVoucher(Transaction transaction)
        {
            try
            {
                if (transaction != null)
                {
                    SolidBrush color = new SolidBrush(Color.Black);
                    Font fontKey = new Font("Arial", 8, System.Drawing.FontStyle.Bold);
                    Font fontValue = new Font("Arial", 8, System.Drawing.FontStyle.Regular);
                    int y = 0;
                    int sum = 25;
                    int x = 150;
                    int xKey = 15;

                    var data = new List<DataPrinter>()
                    {
                        new DataPrinter{ image = GetConfiguration("ImageBoucher"),  x = 80, y = 2 },

                        new DataPrinter{ brush = color, font = fontKey,   value = "Comprobante de pago", x = 80, y = y+=120 },

                        new DataPrinter { brush = color, font = fontValue, value = "CAMARA DE COMERCIO SANTA MARTA PARA EL", x = xKey, y = y += sum+10 },
                        new DataPrinter { brush = color, font = fontValue, value = "MAGDALENA", x = 100, y = y += sum },
                        new DataPrinter { brush = color, font = fontValue, value = "CLL 24 No 2-66", x = 100, y = y += 30 },
                        new DataPrinter { brush = color, font = fontValue, value = "TEL. 4209909", x = 100, y = y += 30 },

                        new DataPrinter{ brush = color, font = fontKey,   value = "NIT:", x = xKey, y = y+=sum+10 },
                        new DataPrinter{ brush = color, font = fontValue, value = "891 780 160-9" ?? string.Empty, x = x, y = y },

                        new DataPrinter{ brush = color, font = fontKey,   value = "========================================", x = xKey, y = y+=sum },

                        new DataPrinter{ brush = color, font = fontKey,   value = "", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value = transaction.IdTransactionAPi.ToString(), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value = "", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value = DateTime.Now.ToString("yyyy/MM/dd"), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value = "", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value = DateTime.Now.ToString("hh:mm:ss"), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value = "", x = xKey, y = y+=sum },

                        new DataPrinter{ brush = color, font = fontKey,   value = "========================================", x = xKey, y = y+=sum },

                        new DataPrinter{ brush = color, font = fontKey,   value =  "", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value =  String.Format("{0:C0}", transaction.Amount), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value =  "", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value =  String.Format("{0:C0}", transaction.Payment.ValorIngresado), x = x, y = y },
                        new DataPrinter{ brush = color, font = fontKey,   value =  "", x = xKey, y = y+=sum },
                        new DataPrinter{ brush = color, font = fontValue, value =  String.Format("{0:C0}", transaction.Payment.ValorDispensado), x = x, y = y },

                        new DataPrinter{ brush = color, font = fontKey,   value = "========================================", x = xKey, y = y+=sum },
                    };

                    if (!transaction.StateReturnMoney)
                    {
                        data.Add(new DataPrinter { brush = color, font = fontValue, value = MessageResource.ReturnMoneyMs1, x = xKey, y = y += sum });
                        data.Add(new DataPrinter { brush = color, font = fontValue, value = MessageResource.ReturnMoneyMs2, x = xKey, y = y += 20 });
                    }

                    data.Add(new DataPrinter { brush = color, font = fontValue, value = "EL IMPUESTO DE REGISTRO RECAUDADO POR ", x = xKey, y = y += sum });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = "LA CAMARA DE COMERCIO SE TRANSFIERE ", x = xKey, y = y += sum });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = "EN SU TOTALIDAD A LA GOBERNACIÓN DE ", x = xKey, y = y += sum });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = "ACUERDO CON LAS NORMAS VIGENTES. ", x = xKey, y = y += 20 });

                    data.Add(new DataPrinter { brush = color, font = fontValue, value = MessageResource.PrintMs1, x = xKey, y = y += sum });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = MessageResource.PrintMs2, x = xKey, y = y += 20 });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = "E-city Software", x = 100, y = y += sum });

                    AdminPayPlus.PrintService.Start(data);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex, ex.ToString());
            }
        }

        public static decimal RoundValue(decimal Total)
        {
            try
            {
                decimal roundTotal = 0;
                roundTotal = Math.Floor(Total / 100) * 100;
                return roundTotal;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
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

        public static string[] ErrorVector = new string[]
        {
            "STACKER_OPEN",
            "JAM_IN_ACCEPTOR",
            "PAUSE",
            "ER:MD",
            "thickness",
            "Scan",
            "FATAL",
            "Printer"
        };
    }
}
