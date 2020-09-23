using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Classes.UseFull;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Resources;
using WPFEmpresaEPM.Services;

namespace WPFEmpresaEPM.UserControls.PagoPrepago
{
    /// <summary>
    /// Interaction logic for ConsultUserControl.xaml
    /// </summary>
    public partial class ConsultUserControl : UserControl
    {
        #region "Referencias"
        private Transaction transaction;
        private TimerGeneric timer;
        #endregion

        #region "Constructor"
        public ConsultUserControl(Transaction ts)
        {
            InitializeComponent();

            try
            {
                transaction = ts;
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

        private void txtNumeroMedidor_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(true, sender as TextBox, this);
        }

        private void txtNumeroMedidor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                txtErrorMedidor.Visibility = Visibility.Hidden;

                if (txtNumeroMedidor.Text.Length > 15)
                {
                    txtNumeroMedidor.Text = txtNumeroMedidor.Text.Remove(15, 1);
                    return;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void txtValor_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(true, sender as TextBox, this);
        }

        private void txtValor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                txtErrorValor.Visibility = Visibility.Hidden;

                if (txtValor.Text.Length > 15)
                {
                    txtValor.Text = txtValor.Text.Remove(15, 1);
                    return;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
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
        private bool Validar()
        {
            try
            {
                if (string.IsNullOrEmpty(txtNumeroMedidor.Text))
                {
                    txtErrorMedidor.Visibility = Visibility.Visible;
                    return false;
                }

                if (string.IsNullOrEmpty(txtValor.Text))
                {
                    txtErrorValor.Visibility = Visibility.Visible;
                    return false;
                }

                if (Convert.ToDecimal(txtValor.Text) <= 0)
                {
                    txtErrorValor.Visibility = Visibility.Visible;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }

            transaction.Amount = Convert.ToDecimal(txtValor.Text);
            transaction.NumeroMedidor = txtNumeroMedidor.Text;

            return true;
        }

        private void Consult()
        {
            try
            {
                Task.Run(async () =>
                {
                    string data = string.Format("?medidor={0}&valorCompra={1}", transaction.NumeroMedidor, transaction.Amount.ToString());

                    string url = string.Concat(Utilities.GetConfiguration("FacturaPrepago"), data);

                    var response = await ApiIntegration.SearchFacturaPrepago(url);

                    Utilities.CloseModal();

                    if (response == null)
                    {
                        Utilities.ShowModal("No se encontrarón resultados. Por favor vuelve a intentarlo.", EModalType.Error);
                        ActivateTimer();
                    }
                    else
                    {
                        transaction.detailsPrepago = response;
                        Utilities.navigator.Navigate(UserControlView.DetailsPagoPrepago, transaction);
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
