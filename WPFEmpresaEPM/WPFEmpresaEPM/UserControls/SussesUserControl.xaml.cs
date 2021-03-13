using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Resources;
using WPFEmpresaEPM.Services.Object;

namespace WPFEmpresaEPM.UserControls
{
    /// <summary>
    /// Lógica de interacción para SussesUserControl.xaml
    /// </summary>
    public partial class SussesUserControl : UserControl
    {
        #region "Referencias"
        private Transaction transaction;
        #endregion

        #region "Constructor"
        public SussesUserControl(Transaction transaction)
        {
            InitializeComponent();

            this.transaction = transaction;

            grvSupport.Content = Utilities.UCSupport;

            FinishTrnsaction();
        }
        #endregion

        #region "Métodos"
        private void FinishTrnsaction()
        {
            try
            {
                Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(transaction.Observation))
                    {
                        AdminPayPlus.SaveErrorControl(transaction.Observation, "", EError.Device, ELevelError.Medium);
                    }

                    AdminPayPlus.UpdateTransaction(this.transaction);

                    transaction.StatePay = "Aprobado";

                    Utilities.PrintVoucherSuccess(this.transaction);

                    Thread.Sleep(3000);

                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        if (transaction.State == ETransactionState.Error)
                        {
                            Utilities.RestartApp();
                        }
                        else
                        {
                            Utilities.navigator.Navigate(UserControlView.Main);
                        }
                    });
                    GC.Collect();
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