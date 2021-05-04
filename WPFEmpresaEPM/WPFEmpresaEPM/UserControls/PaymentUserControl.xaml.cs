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
using WPFEmpresaEPM.Windows.Alerts;

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
        private int Intentos = 1;
        private ModalLoadWindow loading;
        #endregion

        #region "Constructor"
        public PaymentUserControl(Transaction transaction)
        {
            InitializeComponent();

            this.transaction = transaction;

            this.transaction.statePaySuccess = false;

            grvSupport.Content = Utilities.UCSupport;
        }
        #endregion

        #region "Eventos"
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            OrganizeValues();
        }

        private void BtnCancell_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            CancellPay();
        }
        #endregion

        #region Métodos
        private void OrganizeValues()
        {
            try
            {   //TODO:aqui
                this.transaction.Amount = 200;

                this.paymentViewModel = new PaymentViewModel
                {
                    PayValue = this.transaction.Amount,
                    ValorFaltante = this.transaction.Amount,
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

                string moreMs = string.Empty;

                if (transaction.Amount > transaction.RealAmount)
                {
                    moreMs = $"¿ Desea asumir el ajuste de {String.Format("{0:C0}", transaction.RealAmount)} a {String.Format("{0:C0}", transaction.Amount)} ?";

                    if (Utilities.ShowModal(string.Concat("Este dispositivo no devuelve cantidades inferiores a $100. ",Environment.NewLine,moreMs), EModalType.Information ,false))
                    {
                        ActivateWallet();
                    }
                    else
                    {
                        Utilities.navigator.Navigate(UserControlView.Main);
                    }
                }
                else
                {
                    ActivateWallet();
                }
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

                                ReturnMoney(paymentViewModel.ValorSobrante);
                            }
                            else
                            {
                                transaction.StateReturnMoney = true;
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

        private void ReturnMoney(decimal returnValue)
        {
            try
            {
                AdminPayPlus.ControlPeripherals.callbackTotalOut = totalOut =>
                {
                    AdminPayPlus.ControlPeripherals.callbackOut = null;
                    AdminPayPlus.ControlPeripherals.callbackTotalOut = null;

                    transaction.StateReturnMoney = true;
                    paymentViewModel.ValorDispensado = totalOut;
                    SavePay();
                };

                AdminPayPlus.ControlPeripherals.callbackLog = log =>
                {
                    paymentViewModel.SplitDenomination(log);
                    AdminPayPlus.SaveDetailsTransaction(transaction.IdTransactionAPi, 0, 0, 0, string.Empty, log);
                };

                AdminPayPlus.ControlPeripherals.callbackOut = valueOut =>
                {
                    AdminPayPlus.ControlPeripherals.callbackOut = null;
                    AdminPayPlus.ControlPeripherals.callbackTotalOut = null;

                    paymentViewModel.ValorDispensado = valueOut;
                    transaction.StateReturnMoney = false;

                    if (paymentViewModel.ValorDispensado == paymentViewModel.ValorSobrante)
                    {
                        transaction.StateReturnMoney = true;
                        SavePay();
                    }
                    else
                    {
                        transaction.Observation += MessageResource.IncompleteMony + " Devolvio: " + valueOut.ToString();
                        Utilities.ShowModal("No se pudo entregar la totalidad del dinero. Por favor comunícate con un administrador.", EModalType.Error, false);
                        SavePay(ETransactionState.Error);
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
                        Thread.Sleep(1000);

                        object dataTransaction = null;
                        //TODO:aqui
                        //switch (transaction.typeTransaction)
                        //{
                        //    case ETypeTransaction.PagoFactura:
                        //        dataTransaction = new InvoicePayRequest
                        //        {
                        //            payValue = transaction.RealAmount,
                        //            reference = transaction.detailsPagoFactura.Referencia
                        //        };
                        //        break;
                        //    case ETypeTransaction.FacturaPrepago:
                        //        dataTransaction = new PrepaidPayRequest
                        //        {
                        //            payValue = transaction.Amount,
                        //            reference = transaction.NumeroMedidor,
                        //            transactionID = transaction.ConsecutivoId
                        //        };
                        //        break;
                        //    case ETypeTransaction.PagoMedida:
                        //        dataTransaction = new PayRequest
                        //        {
                        //            contract = transaction.NumeroContrato,
                        //            document = transaction.Document,
                        //            transactionID = transaction.ConsecutivoId,
                        //            payValue = transaction.Amount
                        //        };
                        //        break;
                        //}

                        //bool response = await ApiIntegration.ReportPay(transaction, dataTransaction);

                        //Thread.Sleep(1000);

                        //if (!response && Intentos < 5)
                        //{
                        //    Intentos++;
                        //    AdminPayPlus.CreateConsecutivoDashboard(transaction);
                        //    this.paymentViewModel.StatePay = false;
                        //    SavePay();
                        //    return;
                        //}

                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            this.Opacity = 1;
                            loading.Close();
                        });
                        GC.Collect();

                        //if (response)
                        //{
                            this.transaction.statePaySuccess = true;
                            transaction.State = ETransactionState.Success;
                            Utilities.navigator.Navigate(UserControlView.PaySuccess, transaction);
                        //}
                        //else
                        //{
                        //    CancelTransaction();
                        //}
                    });

                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        if (loading == null)
                        {
                            loading = new ModalLoadWindow();
                            this.Opacity = 0.3;
                            loading.ShowDialog();
                        }
                    });
                    GC.Collect();
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
                Utilities.ShowModal(ms, EModalType.Error , false);
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
