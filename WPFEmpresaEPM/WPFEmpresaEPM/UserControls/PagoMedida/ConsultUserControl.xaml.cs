﻿using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Classes.UseFull;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Resources;
using WPFEmpresaEPM.Services;

namespace WPFEmpresaEPM.UserControls.PagoMedida
{
    /// <summary>
    /// Interaction logic for ConsultUserControl.xaml
    /// </summary>
    public partial class ConsultUserControl : UserControl
    {
        #region "Referencias"
        private Transaction transaction;
        private TimerGeneric timer;
        private CheckTypeSerch check;
        private bool document;
        #endregion

        #region "Constructor"
        public ConsultUserControl(Transaction ts)
        {
            InitializeComponent();

            try
            {
                transaction = ts;
                check = new CheckTypeSerch();
                check.Numero = GetImage(false);
                check.Referencia = GetImage(true);
                document = true;
                this.DataContext = check;
                ActivateTimer();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Eventos"
        private void BtnBack_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void BtnExit_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Main);
        }

        private void TxtNumDocument_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(true, sender as TextBox, this);
        }

        private void TxtNumDocument_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                txtErrorDocumento.Visibility = Visibility.Hidden;

                if (TxtNumDocument.Text.Length > 15)
                {
                    TxtNumDocument.Text = TxtNumDocument.Text.Remove(15, 1);
                    return;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void BtnDocumento_TouchDown(object sender, TouchEventArgs e)
        {
            check.Numero = GetImage(false);
            check.Referencia = GetImage(true);
            document = true;
        }

        private void BtnContrato_TouchDown(object sender, TouchEventArgs e)
        {
            check.Numero = GetImage(true);
            check.Referencia = GetImage(false);
            document = false;
        }

        private void btnConsult_TouchDown(object sender, TouchEventArgs e)
        {
            if (Validar())
            {
                Consult();
            }
        }
        #endregion

        #region "Métodos"

        private string GetImage(bool flag)
        {
            try
            {
                if (!flag)
                {
                    return "/Images/Others/circle.png";
                }

                return "/Images/Others/ok.png";
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return string.Empty;
        }

        private bool Validar()
        {
            try
            {
                if (string.IsNullOrEmpty(TxtNumDocument.Text))
                {
                    txtErrorDocumento.Visibility = Visibility.Visible;
                    return false;
                }

                transaction.Document = document ? TxtNumDocument.Text:null;
                transaction.NumeroContrato = document ? null:TxtNumDocument.Text;

                return true;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                return false;
            }
        }

        private void Consult()
        {
            try
            {
                Task.Run(async () =>
                {
                    Thread.Sleep(500);

                    string data = string.Format("?Contrato={0}&IdCliente={1}", transaction.NumeroContrato, null);
                    //string data = string.Format("?Contrato={0}&IdCliente={1}", transaction.NumeroContrato, transaction.Document);

                    string url = string.Concat(Utilities.GetConfiguration("PagoMedida"), data);

                    var response = await ApiIntegration.SearchPagoMedida(url);

                    Utilities.CloseModal();

                    if (response == null)
                    {
                        Utilities.ShowModal("No se encontrarón resultados. Por favor vuelve a intentarlo.", EModalType.Error);
                        ActivateTimer();
                    }
                    else
                    {
                        transaction.detailsPagoMedida = response;
                        Utilities.navigator.Navigate(UserControlView.DetailsPagoMedida, transaction);
                    }
                });

                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
                Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Timer"
        private void ActivateTimer()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                tbTimer.Text = Utilities.GetConfiguration("TimerGenerico");
                timer = new TimerGeneric(tbTimer.Text);
                timer.CallBackClose = response =>
                {
                    Utilities.navigator.Navigate(UserControlView.Main);
                };
                timer.CallBackTimer = response =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        tbTimer.Text = response;
                    });
                };
            });
            GC.Collect();
        }

        private void SetCallBacksNull()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                timer.CallBackClose = null;
                timer.CallBackTimer = null;
            });
            GC.Collect();
        }
        #endregion
    }
}
