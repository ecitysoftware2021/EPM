using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Resources;
using WPFEmpresaEPM.Services;
using WPFEmpresaEPM.Services.Object;

namespace WPFEmpresaEPM.UserControls.Administrator
{
    /// <summary>
    /// Lógica de interacción para ConfigurateUserControl.xaml
    /// </summary>
    public partial class ConfigurateUserControl : UserControl
    {
        #region "Referencias"
        private AdminPayPlus init;
        #endregion

        #region "Constructor"
        public ConfigurateUserControl()
        {
            try
            {
                InitializeComponent();

                if (init == null)
                {
                    init = new AdminPayPlus();
                }

                txtMs.DataContext = init;

                ExtraData();
                Initial();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Métodos"
        private async void Initial()
        {
            try
            {
                init.callbackResult = result =>
                {
                    ProccesResult(result);
                };

                init.Start();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private async void ProccesResult(bool result)
        {
            try
            {
                if (AdminPayPlus.DataPayPlus == null)
                {
                    Finish(result);
                }
                else
                if (AdminPayPlus.DataPayPlus.StateUpdate)
                {
                    Utilities.ShowModal(MessageResource.UpdateAplication, EModalType.Error, true);
                    Utilities.UpdateApp();
                }
                else if (AdminPayPlus.DataPayPlus.StateBalanece)
                {
                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Reference = "",
                        Description = string.Concat(MessageResource.NoGoInitial, " ", MessageResource.ModoAdministrativo),
                        State = 2,
                        Date = DateTime.Now
                    }, ELogType.General);

                    Utilities.navigator.Navigate(UserControlView.Login, ETypeAdministrator.Balancing);
                }
                else if (AdminPayPlus.DataPayPlus.StateUpload)
                {
                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Reference = "",
                        Description = string.Concat(MessageResource.NoGoInitial, " ", MessageResource.ModoAdministrativo),
                        State = 2,
                        Date = DateTime.Now
                    }, ELogType.General);

                    Utilities.navigator.Navigate(UserControlView.Login, ETypeAdministrator.Upload);
                }
                else
                {
                    Finish(result);
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowModal(string.Concat(init.DescriptionStatusPayPlus, " ", MessageResource.NoService), EModalType.Error, false);
                Initial();
            }
        }

        private void Finish(bool state)
        {
            try
            {
                if (Utilities.UCSupport == null)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Utilities.UCSupport = new SupportUserControl();
                        grvSupport.Content = Utilities.UCSupport;
                    });
                    GC.Collect();
                }

                Task.Run(() =>
                {
                    Thread.Sleep(3000);

                    if (state)
                    {
                        Utilities.navigator.Navigate(UserControlView.Main);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(AdminPayPlus.DataPayPlus.Message))
                        {
                            AdminPayPlus.SaveLog(new RequestLog
                            {
                                Reference = "",
                                Description = string.Concat(MessageResource.NoGoInitial, " ", init.DescriptionStatusPayPlus),
                                State = 6,
                                Date = DateTime.Now
                            }, ELogType.General);

                            Utilities.ShowModal(MessageResource.NoService + " " + MessageResource.NoMoneyKiosco, EModalType.Error);
                            Initial();
                        }
                        else
                        {
                            AdminPayPlus.SaveLog(new RequestLog
                            {
                                Reference = "",
                                Description = string.Concat(MessageResource.NoGoInitial, " ", init.DescriptionStatusPayPlus),
                                State = 2,
                                Date = DateTime.Now
                            }, ELogType.General);

                            Utilities.ShowModal(MessageResource.NoService + " " + init.DescriptionStatusPayPlus, EModalType.Error);
                            Initial();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void ExtraData()
        {
            try
            {
                string a = "usrapli 1Cero12019$/* ";

                string b = "Ecity.Software                                      Ecitysoftware2019#" +
                           "Pay+ EPM Ed. Inteligente 1                          EmpresasPublicasdeMedellin2020/" +
                           "Pay+ EPM Punto Facil Bello                          EmpresaPublicadeMedellin2020!" +
                           "Pay+ EPM Punto Miguel de Aguinaga 1                 EmpresaPublicadeMedellin2020*" +
                           "Pay+ EPM Punto Facil Belen 1                        EmpresaPublicadeMedellin2021%" +
                           "Pay+ EPM Punto Facil Envigado 1                     EmpresaPublicadeMedellin2021!" +
                           "Pay+ EPM Punto Terminal del Norte                   EmpresaPublicadeMedellin2021!" +
                           "Pay+ EPM Punto fácil Hospital Pablo Tobón Uribe     EmpresaPublicadeMedellin2021*" +
                           "Pay+ EPM Punto fácil oficina San Antonio de Prado   EmpresaPublicadeMedellin2021=" +
                           "Pay+ EPM Punto Facil Rionegro                       EmpresaPublicadeMedellin2021%" +
                           "Pay+ EPM Punto Facil La Ceja                        EmpresaPublicadeMedellin2021$" +
                           "Pay+ EPM Punto Facil Sabaneta                       EmpresaPublicadeMedellin2021=" ;
                          
                ExtraData data = new ExtraData();

                data.dataIntegration = new DataIntegration
                {
                    basseAddresEPM = "http://apiepm.e-citypay.co/{0}",
                    ConsultarFactura = "PagoFactura/ConsultarFactura",
                    ValidarPagoMedida = "PagaTuMedida/ValidarPagoMedida",
                    ValidarCompra = "PagoPrepago/ValidarCompra",
                    RegistrarPagoFactura = "PagoFactura/RegistrarPago",
                    RegistrarPagoMedida = "PagaTuMedida/PagarMedida",
                    RegistarCompraEnergia = "PagoPrepago/ComprarEnergia"
                };

                data.dataComplementary = new DataComplementary
                {
                    DurationAlert = "10000",
                    NAME_PAYPAD = "Pay+ EPM",
                    LAST_NAME_PAYPAD = "EPM",
                    NAME_APLICATION = "WPFEmpresaEPM.exe",
                    PathRedeban = @"C:\Redebam\",
                    ValorMinPrepago = "10000",
                    ValorMaxPrepago = "100000",
                    NumbersPhone = "(034)5606111 op 7",
                    NumbersSerial = "1111",
                };

                string json = JsonConvert.SerializeObject(data);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion
    }
}
