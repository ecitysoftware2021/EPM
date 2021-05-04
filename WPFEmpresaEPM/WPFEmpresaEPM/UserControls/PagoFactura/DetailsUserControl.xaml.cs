using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Classes.UseFull;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Resources;

namespace WPFEmpresaEPM.UserControls.PagoFactura
{
    /// <summary>
    /// Interaction logic for DetailsUserControl.xaml
    /// </summary>
    public partial class DetailsUserControl : UserControl
    {
        #region "Referencias"
        private Transaction transaction;
        private TimerGeneric timer;
        #endregion

        #region "Constructor"
        public DetailsUserControl(Transaction ts)
        {
            InitializeComponent();

            try
            {
                transaction = ts;
                grvSupport.Content = Utilities.UCSupport;
                DataContext = transaction.detailsPagoFactura;
                ActivateTimer();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        #region "Eventos"
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (!AdminPayPlus.DataPayPlus.PayPadConfiguration.enablE_CARD)
                {
                    btnTarjeta.Visibility = System.Windows.Visibility.Hidden;
                }

                if (!AdminPayPlus.DataPayPlus.PayPadConfiguration.enablE_VALIDATE_PERIPHERALS)
                {
                    btnEfectivo.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void BtnBack_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.InvoiceListPagoFactura, transaction);
        }

        private void BtnExit_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Main);
        }

        private void btnPay_TouchDown(object sender, TouchEventArgs e)
        {
            SaveTransaction(int.Parse((sender as Image).Tag.ToString()));
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
                    transaction.PaymentType = type == 1 ? EPaymentType.Cash : EPaymentType.Card;
                    transaction.payer = null;
                    
                    transaction.Amount = Utilities.RoundValue(transaction.detailsPagoFactura.ValorPagar,true);
                    transaction.RealAmount = transaction.detailsPagoFactura.ValorPagar;

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
        #endregion

        #region "Timer"
        private void ActivateTimer()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                tbTimer.Text = AdminPayPlus.DataPayPlus.PayPadConfiguration.generiC_TIMER;
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
