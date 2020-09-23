using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.ViewModel;
using System.Reflection;
using WPFEmpresaEPM.Services.Object;
using System.Windows.Data;
using System.Collections.Generic;
using WPFEmpresaEPM.Resources;
using System.Threading;
using WPFEmpresaEPM.Services;
using Newtonsoft.Json;
using WPFEmpresaEPM.Services.ObjectIntegration;

namespace WPFEmpresaEPM.UserControls
{
    /// <summary>
    /// Lógica de interacción para PaymentUserControl.xaml
    /// </summary>
    public partial class PaymentUserControl : UserControl
    {
        #region "Referencias"
        private Transaction transaction;
        private PaymentViewModel paymentViewModel;
        #endregion

        #region "Constructor"
        public PaymentUserControl(Transaction transaction)
        {
            InitializeComponent();

            this.transaction = transaction;

            this.transaction.statePaySuccess = false;

            OrganizeValues();
        }
        #endregion

        #region "Eventos"
        private void BtnCancell_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            CancellPay();
        }
        #endregion

        #region Métodos
        private void OrganizeValues()
        {
            try
            {
                this.paymentViewModel = new PaymentViewModel
                {
                    PayValue = 100,
                    ValorFaltante = 100,
                    ImgContinue = Visibility.Hidden,
                    ImgCancel = Visibility.Visible,
                    ImgCambio = Visibility.Hidden,
                    ValorSobrante = 0,
                    ValorIngresado = 0,
                    viewList = new CollectionViewSource(),
                    Denominations = new List<DenominationMoney>(),
                    ValorDispensado = 0
                };

                this.DataContext = this.paymentViewModel;

                ActivateWallet();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void ActivateWallet()
        {
            try
            {
                Task.Run(() =>
                {
                    AdminPayPlus.ControlPeripherals.callbackValueIn = enterValue =>
                    {
                        if (enterValue.Item1 > 0)
                        {
                            if (!this.paymentViewModel.StatePay)
                            {
                                paymentViewModel.ValorIngresado += enterValue.Item1;

                                paymentViewModel.RefreshListDenomination(int.Parse(enterValue.Item1.ToString()), 1, enterValue.Item2);

                                AdminPayPlus.SaveDetailsTransaction(transaction.IdTransactionAPi, enterValue.Item1, 2, 1, enterValue.Item2, string.Empty);
                                LoadView();
                            }
                        }
                    };

                    AdminPayPlus.ControlPeripherals.callbackTotalIn = enterTotal =>
                    {
                        if (!this.paymentViewModel.StatePay)
                        {
                            this.paymentViewModel.ImgCancel = Visibility.Hidden;

                            AdminPayPlus.ControlPeripherals.StopAceptance();

                            if (enterTotal > 0 && paymentViewModel.ValorSobrante > 0)
                            {
                                this.paymentViewModel.ImgCambio = Visibility.Visible;

                                ReturnMoney(paymentViewModel.ValorSobrante, true);
                            }
                            else
                            {
                                SavePay();
                            }
                        }
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

                    AdminPayPlus.ControlPeripherals.StartAceptance(paymentViewModel.PayValue);
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void ReturnMoney(decimal returnValue, bool state)
        {
            try
            {
                AdminPayPlus.ControlPeripherals.callbackTotalOut = totalOut =>
                {
                    if (state)
                    {
                        transaction.StateReturnMoney = true;
                        paymentViewModel.ValorDispensado = totalOut;
                        SavePay();
                    }
                };

                AdminPayPlus.ControlPeripherals.callbackLog = log =>
                {
                    paymentViewModel.SplitDenomination(log);
                    AdminPayPlus.SaveDetailsTransaction(transaction.IdTransactionAPi, 0, 0, 0, string.Empty, log);
                };

                AdminPayPlus.ControlPeripherals.callbackOut = valueOut =>
                {
                    AdminPayPlus.ControlPeripherals.callbackOut = null;
                    if (state)
                    {
                        paymentViewModel.ValorDispensado = valueOut;
                        transaction.StateReturnMoney = false;

                        if (paymentViewModel.ValorDispensado == paymentViewModel.ValorSobrante)
                        {
                            transaction.StateReturnMoney = true;
                            SavePay();
                        }
                        else
                        {
                            transaction.Observation += MessageResource.IncompleteMony + " " + "Devolvio: " + valueOut.ToString();
                            Utilities.ShowModal(MessageResource.IncompleteMony, EModalType.Error);
                            SavePay(ETransactionState.Error);
                        }
                    }
                };

                AdminPayPlus.ControlPeripherals.StartDispenser(returnValue);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void LoadView()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    paymentViewModel.viewList.Source = paymentViewModel.Denominations;
                    lv_denominations.DataContext = paymentViewModel.viewList;
                    lv_denominations.Items.Refresh();
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void SavePay(ETransactionState statePay = ETransactionState.Initial)
        {
            try
            {
                if (!this.paymentViewModel.StatePay)
                {
                    this.paymentViewModel.StatePay = true;
                    transaction.Payment = paymentViewModel;
                    transaction.State = statePay;

                    AdminPayPlus.ControlPeripherals.ClearValues();

                    Task.Run(async () =>
                    {
                        string url = string.Empty;

                        switch (transaction.typeTransaction)
                        {
                            case ETypeTransaction.PagoFactura:
                                url = string.Format("{0}?referencia={1}&valor={2}",
                                      Utilities.GetConfiguration("ReportPagoDeFactura"),
                                      transaction.detailsPagoFactura.Referencia, transaction.Amount);
                                break;
                            case ETypeTransaction.FacturaPrepago:
                                url = string.Format("{0}?medidor={1}&valorCompra={2}&idTransaccion={3}", 
                                      Utilities.GetConfiguration("ReportFacturaPrepago"), 
                                      transaction.NumeroMedidor, transaction.Amount, 
                                      transaction.IdTransactionAPi);
                                break;
                            case ETypeTransaction.PagoMedida:
                                url = string.Format("{0}?Contrato={1}&IdCliente={2}&idTransaccion={3}&valorPagado={4}", 
                                      Utilities.GetConfiguration("ReportPagoMedida"),
                                      transaction.NumeroContrato, transaction.Document,
                                      transaction.IdTransactionAPi,transaction.Amount);
                                break;
                        }

                        bool response = await ApiIntegration.ReportPay(transaction, url);

                        if (response)
                        {
                            this.transaction.statePaySuccess = true;
                            transaction.State = ETransactionState.Success;
                            Utilities.navigator.Navigate(UserControlView.PaySuccess, transaction);
                        }
                        else
                        {
                            CancelTransaction();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                CancelTransaction();
            }
        }

        private void CancelTransaction()
        {
            try
            {
                string ms = "Estimado usuario, no se pudo notificar el pago. Se le hará la devolución del dinero ingresado.";
                Utilities.ShowModal(ms, EModalType.Error);
                Utilities.navigator.Navigate(UserControlView.ReturnMony, transaction);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void CancellPay()
        {
            try
            {
                this.paymentViewModel.ImgContinue = Visibility.Hidden;

                this.paymentViewModel.ImgCancel = Visibility.Hidden;

                if (Utilities.ShowModal(MessageResource.CancelTransaction, EModalType.Information))
                {
                    AdminPayPlus.ControlPeripherals.StopAceptance();
                    AdminPayPlus.ControlPeripherals.callbackLog = null;
                    if (!this.paymentViewModel.StatePay)
                    {
                        if (paymentViewModel.ValorIngresado > 0)
                        {
                            transaction.Payment = paymentViewModel;
                            Utilities.navigator.Navigate(UserControlView.ReturnMony, transaction);
                        }
                        else
                        {
                            Utilities.navigator.Navigate(UserControlView.Main);
                        }
                    }
                }
                else
                {
                    if (paymentViewModel.ValorIngresado > 0)
                    {
                        this.paymentViewModel.ImgContinue = Visibility.Visible;
                    }

                    this.paymentViewModel.ImgCancel = Visibility.Visible;
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
