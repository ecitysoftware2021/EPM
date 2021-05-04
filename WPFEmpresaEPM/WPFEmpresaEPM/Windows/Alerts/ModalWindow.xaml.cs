using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Classes.UseFull;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Resources;

namespace WPFEmpresaEPM.Windows
{
    /// <summary>
    /// Lógica de interacción para ModalWindow.xaml
    /// </summary>
    public partial class ModalWindow : Window
    {
        #region "Referencias"
        private ModalModel modal;
        private TimerGeneric timer;
        #endregion

        #region "Constructor"
        public ModalWindow(ModalModel modal)
        {
            InitializeComponent();

            this.modal = modal;

            this.DataContext = this.modal;

            ConfigureModal();
        }
        #endregion

        #region "Métodos"
        private void ConfigureModal()
        {
            try
            {
                if (this.modal.Timer && this.modal.TypeModal != EModalType.Preload)
                {
                    GoTimer();
                }

                if (this.modal.TypeModal == EModalType.Preload)
                {
                    this.BtnOk.Visibility = Visibility.Hidden;
                    this.BtnYes.Visibility = Visibility.Hidden;
                    this.BtnNo.Visibility = Visibility.Hidden;
                    this.LblMessageTouch.Visibility = Visibility.Hidden;
                    GifLoadder.Visibility = Visibility.Visible;
                }
                else if (this.modal.TypeModal == EModalType.NotExistAccount)
                {
                    this.BtnOk.Visibility = Visibility.Visible;
                    this.BtnYes.Visibility = Visibility.Hidden;
                    this.BtnNo.Visibility = Visibility.Hidden;
                    this.LblMessageTouch.Visibility = Visibility.Hidden;
                    GifLoadder.Visibility = Visibility.Hidden;
                }
                else if (this.modal.TypeModal == EModalType.MaxAmount || this.modal.TypeModal == EModalType.Error)
                {
                    this.BtnOk.Visibility = Visibility.Visible;
                    this.BtnYes.Visibility = Visibility.Hidden;
                    this.BtnNo.Visibility = Visibility.Hidden;
                    this.LblMessageTouch.Visibility = Visibility.Hidden;
                    GifLoadder.Visibility = Visibility.Hidden;
                }
                else if (this.modal.TypeModal == EModalType.Information || this.modal.TypeModal == EModalType.NoPaper)
                {
                    this.BtnOk.Visibility = Visibility.Hidden;
                    this.BtnYes.Visibility = Visibility.Visible;
                    this.LblMessageTouch.Visibility = Visibility.Hidden;
                    this.BtnNo.Visibility = Visibility.Visible;
                    GifLoadder.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Eventos"
        private void BtnOk_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                StopTimer();
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void BtnYes_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                StopTimer();
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void BtnNo_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                StopTimer();
                this.DialogResult = false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Timer"
        public void GoTimer()
        {
            try
            {
                if (this.modal.Timer && this.modal.TypeModal != EModalType.Preload)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        timer = new TimerGeneric(AdminPayPlus.DataPayPlus.PayPadConfiguration.modaL_TIMER);
                        timer.CallBackClose = response =>
                        {
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                DialogResult = true;
                            });
                            GC.Collect();
                        };
                    });
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        public void StopTimer()
        {
            try
            {
                if (this.modal.Timer && this.modal.TypeModal != EModalType.Preload)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        timer.CallBackClose = null;
                        timer.CallBackTimer = null;
                        timer.CallBackStop?.Invoke(1);
                    });
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion
    }
}

