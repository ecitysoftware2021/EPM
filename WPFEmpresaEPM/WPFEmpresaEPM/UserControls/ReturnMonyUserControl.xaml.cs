using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Resources;
using WPFEmpresaEPM.Services.Object;
using WPFEmpresaEPM.ViewModel;

namespace WPFEmpresaEPM.UserControls
{
    /// <summary>
    /// Lógica de interacción para CancelUserControl.xaml
    /// </summary>
    public partial class ReturnMonyUserControl : UserControl
    {
        #region "Referencias"
        private Transaction transaction;
        private decimal ValueReturn;
        #endregion

        #region "Constructor"
        public ReturnMonyUserControl(Transaction transaction)
        {
            InitializeComponent();

            this.transaction = transaction;

            ValueReturn = 0;

            ReturnMoney();
        }
        #endregion

        #region "Metodos"
        private void ReturnMoney()
        {
            try
            {
                ValueReturn = transaction.Payment.ValorIngresado - transaction.Payment.ValorDispensado;

                txtValueReturn.Text = string.Format("{0:C0}", ValueReturn);
                Task.Run(() =>
                {
                    AdminPayPlus.ControlPeripherals.callbackTotalOut = totalOut =>
                    {
                        transaction.StateReturnMoney = true;

                        transaction.Payment.ValorDispensado = totalOut;
                        transaction.Payment.ValorSobrante = transaction.Payment.ValorIngresado;
                        transaction.State = ETransactionState.Cancel;
                        FinishCancelPay();
                    };

                    AdminPayPlus.ControlPeripherals.callbackError = error =>
                    {
                        AdminPayPlus.SaveLog(new RequestLogDevice
                        {
                            Code = error.Item1,
                            Date = DateTime.Now,
                            Description = error.Item2,
                            Level = ELevelError.Medium,
                            TransactionId = transaction.IdTransactionAPi
                        }, ELogType.Device);
                    };

                    AdminPayPlus.ControlPeripherals.callbackOut = valueOut =>
                    {
                        AdminPayPlus.ControlPeripherals.callbackOut = null;
                        transaction.Payment.ValorDispensado = valueOut;

                        transaction.StateReturnMoney = false;

                        if (!transaction.statePaySuccess && transaction.Payment.ValorDispensado != transaction.Payment.ValorIngresado)
                        {
                            transaction.State = ETransactionState.CancelError;
                            transaction.Observation += MessageResource.IncompleteMony + " " + "Devolvio: " + valueOut.ToString();
                        }
                        else
                        {
                            transaction.StateReturnMoney = true;
                        }
                        FinishCancelPay();
                    };

                    AdminPayPlus.ControlPeripherals.callbackLog = log =>
                    {
                        AdminPayPlus.SaveDetailsTransaction(transaction.IdTransactionAPi, 0, 0, 0, string.Empty, log);
                    };

                    AdminPayPlus.ControlPeripherals.StartDispenser(ValueReturn);
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void FinishCancelPay()
        {
            try
            {
                AdminPayPlus.ControlPeripherals.ClearValues();

                if (!string.IsNullOrEmpty(transaction.Observation))
                {
                    AdminPayPlus.SaveErrorControl(transaction.Observation, "", EError.Device, ELevelError.Medium);
                }

                transaction.State = ETransactionState.Cancel;

                transaction.StatePay = "Cancelada";

                AdminPayPlus.UpdateTransaction(transaction);

                //Utilities.PrintVoucher(transaction);

                Thread.Sleep(5000);

                AdminPayPlus.ControlPeripherals.ClearValues();

                Utilities.navigator.Navigate(UserControlView.Main);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion
    }
}
