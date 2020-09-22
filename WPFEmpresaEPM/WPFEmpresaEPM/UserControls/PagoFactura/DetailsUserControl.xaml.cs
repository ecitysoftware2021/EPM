using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private void BtnBack_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.ConsultPagoFactura,transaction);
        }

        private void BtnExit_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Main);
        }

        private void BtnPagar_TouchDown(object sender, TouchEventArgs e)
        {
            SaveTransaction();
        }
        #endregion

        #region "Métodos"
        private void SaveTransaction()
        {
            try
            {
                Task.Run(async () =>
                {
                    transaction.Type = ETransactionType.Payment;
                    transaction.State = ETransactionState.Initial;
                    transaction.payer = null;
                    transaction.Amount = transaction.detailsPagoFactura.ValorPagar;

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
                        Utilities.navigator.Navigate(UserControlView.Pay, transaction);
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
