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
using WPFEmpresaEPM.Services;

namespace WPFEmpresaEPM.UserControls
{
    /// <summary>
    /// Lógica de interacción para MenuUserControl.xaml
    /// </summary>
    public partial class MenuUserControl : UserControl
    {
        #region "Referencias"
        private Transaction transaction;
        private TimerGeneric timer;
        #endregion

        #region "Constructor"
        public MenuUserControl()
        {
            InitializeComponent();
            transaction = new Transaction();
            grvSupport.Content = Utilities.UCSupport;
            ActivateTimer();
        }
        #endregion

        #region "Eventos"
        private void btnOption_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);

                string option = (sender as Image).Tag.ToString();

                switch (option)
                {
                    case "1":
                        transaction.typeTransaction = ETypeTransaction.PagoFactura;
                        Utilities.navigator.Navigate(UserControlView.ConsultPagoFactura, transaction);
                        break;
                    case "2":
                        transaction.typeTransaction = ETypeTransaction.PagoMedida;
                        Utilities.navigator.Navigate(UserControlView.ConsultPagoMedida, transaction);
                        break;
                    case "3":
                        transaction.typeTransaction = ETypeTransaction.FacturaPrepago;
                        Utilities.navigator.Navigate(UserControlView.ConsultPagoPrepago, transaction);
                        break;
                }
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
