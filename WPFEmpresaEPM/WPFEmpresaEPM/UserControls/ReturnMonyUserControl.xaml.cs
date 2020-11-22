﻿using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Resources;
using WPFEmpresaEPM.Services.Object;

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

            grvSupport.Content = Utilities.UCSupport;

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
                        AdminPayPlus.ControlPeripherals.callbackOut = null;
                        AdminPayPlus.ControlPeripherals.callbackTotalOut = null;

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
                        AdminPayPlus.ControlPeripherals.callbackTotalOut = null;

                        transaction.Payment.ValorDispensado = valueOut;
                        transaction.StateReturnMoney = false;

                        if (!transaction.statePaySuccess && transaction.Payment.ValorDispensado != transaction.Payment.ValorIngresado)
                        {
                            transaction.State = ETransactionState.CancelError;
                            transaction.Observation += MessageResource.IncompleteMony + " " + "Devolvio: " + valueOut.ToString();
                        }
                        else
                        {
                            transaction.State = ETransactionState.Cancel;
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

                transaction.StatePay = "Cancelada";

                AdminPayPlus.UpdateTransaction(transaction);

                Utilities.PrintVoucher(transaction);

                Thread.Sleep(3000);

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
