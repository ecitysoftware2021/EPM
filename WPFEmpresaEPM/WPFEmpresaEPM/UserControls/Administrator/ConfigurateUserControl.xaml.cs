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
        private ApiIntegration cootregua;
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

                if (cootregua == null)
                {
                    cootregua = new ApiIntegration();
                }

                txtMs.DataContext = init;

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
                if (AdminPayPlus.DataPayPlus.StateUpdate)
                {
                    Utilities.ShowModal(MessageResource.UpdateAplication, EModalType.Error);
                    Utilities.UpdateApp();
                }
                else if (AdminPayPlus.DataPayPlus.StateBalanece)
                {
                    Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, null, MessageResource.ModoAdministrativo);
                    //Utilities.navigator.Navigate(UserControlView.Login, false, ETypeAdministrator.Balancing);
                }
                else if (AdminPayPlus.DataPayPlus.StateUpload)
                {
                    Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, null, MessageResource.ModoAdministrativo);
                    //Utilities.navigator.Navigate(UserControlView.Login, false, ETypeAdministrator.Upload);
                }
                else
                {
                    Finish(result);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                Utilities.ShowModal(MessageResource.NoService, EModalType.Error);
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
                    Thread.Sleep(10000);

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
        #endregion
    }
}
