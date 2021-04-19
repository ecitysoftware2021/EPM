using System;
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

namespace WPFEmpresaEPM.UserControls.PagoMedida
{
    /// <summary>
    /// Interaction logic for DetailsUserControl.xaml
    /// </summary>
    public partial class DetailsUserControl : UserControl
    {
        #region "Referencias"
        private Transaction transaction;
        private TimerGeneric timer; 
        private decimal ValorMin;
        private decimal ValorMax;
        #endregion

        #region "Constructor"
        public DetailsUserControl(Transaction ts)
        {
            InitializeComponent();

            try
            {
                transaction = ts;
                grvSupport.Content = Utilities.UCSupport;
                DataContext = transaction.detailsPagoMedida;
                ValorMin = (decimal)transaction.detailsPagoMedida.ValorMin;
                ValorMax = (decimal)transaction.detailsPagoMedida.ValorTotalFactura;
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
            Utilities.navigator.Navigate(UserControlView.ConsultPagoMedida, transaction);
        }

        private void BtnExit_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Main);
        }

        private void btnPay_TouchDown(object sender, TouchEventArgs e)
        {
            if (Validar())
            {
                SaveTransaction(int.Parse((sender as Image).Tag.ToString()));
            }
        }

        private void txtvalorminimomedida_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(true, sender as TextBox, this);
        }

        private void txtvalorminimomedida_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                txtErrorValor.Visibility = Visibility.Hidden;

                if (txtvalorminimomedida.Text.Length > 8)
                {
                    txtvalorminimomedida.Text = txtvalorminimomedida.Text.Remove(8, 1);
                    return;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Utilities.GetConfiguration("DatafonoIsEnable") == "0")
                {
                    btnTarjeta.Visibility = System.Windows.Visibility.Hidden;
                }

                if (Utilities.GetConfiguration("EfectivoIsEnable") == "0")
                {
                    btnEfectivo.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Métodos"
        private void SaveTransaction(int type)
        {
            try
            {
                Task.Run(async () =>
                {
                    Thread.Sleep(500);

                    transaction.Type = ETransactionType.Payment;
                    transaction.State = ETransactionState.Initial;
                    transaction.payer = null;
                    transaction.PaymentType = type == 1 ? EPaymentType.Cash : EPaymentType.Card;

                    await AdminPayPlus.SaveTransaction(this.transaction);

                    Utilities.CloseModal();

                    if (this.transaction.IdTransactionAPi == 0)
                    {
                        string ms = "Estimado usuario, no se pudo crear la transacción. Por favor intenta de nuevo.";
                        Utilities.ShowModal(ms, EModalType.Error);
                        ActivateTimer();
                    }
                    else
                    {
                        if (type == 1)
                        {
                            Utilities.navigator.Navigate(UserControlView.Pay, transaction);
                        }
                        else
                        {
                            Utilities.navigator.Navigate(UserControlView.Card, transaction);
                        }
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

        private bool Validar()
        {
            try
            {

                if (transaction.PaymentType == EPaymentType.Cash)
                {
                    if (transaction.detailsPagoMedida.ValorMinimoPago % 100 != 0)
                    {
                        txtErrorValor.Text = string.Concat("Esta máquina sólo recibe multiplos de 100",
                        Environment.NewLine, "Ejemplo: $100, $1.000, $10.000... etc.");
                        txtErrorValor.Visibility = Visibility.Visible;
                        return false;
                    }
                }
                

                if (transaction.detailsPagoMedida.ValorMinimoPago < ValorMin || transaction.detailsPagoMedida.ValorMinimoPago > ValorMax)
                {
                    txtErrorValor.Text = string.Concat("Debe ingresar un valor entre",
                    Environment.NewLine, string.Format("{0} y {1}", ValorMin.ToString("C"), ValorMax.ToString("C")));
                    txtErrorValor.Visibility = Visibility.Visible;
                    return false;
                }

                
                transaction.Amount = transaction.detailsPagoMedida.ValorMinimoPago;
                transaction.RealAmount = transaction.detailsPagoMedida.ValorMinimoPago;
                return true;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                return false;
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