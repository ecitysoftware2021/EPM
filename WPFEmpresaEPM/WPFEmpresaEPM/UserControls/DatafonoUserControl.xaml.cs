using DLL_Conexion_Caja_Redebam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Services;
using WPFEmpresaEPM.Services.Object;
using WPFEmpresaEPM.ViewModel;
using WPFEmpresaEPM.Windows.Alerts;

namespace WPFEmpresaEPM.UserControls
{
    /// <summary>
    /// Interaction logic for DatafonoUserControl.xaml
    /// </summary>
    public partial class DatafonoUserControl : UserControl
    {
        #region "Referencias"
        private Transaction transaction;
        private PaymentViewModel paymentViewModel;
        private int Intentos = 1;
        private ModalLoadWindow loading;
        public string Operacion { get; } = "0";
        public string Monto { get; } = "100";
        public string IVA { get; } = "0";
        public string factura { get; } = "1234";
        public string base_dev { get; } = "0";
        public string imp_consu { get; } = "0";
        public string cod_cajero { get; } = "58";
        #endregion

        #region "Constructor"
        public DatafonoUserControl(Transaction ts)
        {
            InitializeComponent();

            this.transaction = ts;

            OrganizeValues();
        }
        #endregion

        #region "Métodos"
        private void OrganizeValues()
        {
            try
            {
                var paymentViewModel = new PaymentViewModel
                {
                    PayValue = transaction.Amount,
                    ValorFaltante = transaction.Amount,
                    ImgContinue = Visibility.Hidden,
                    ImgCancel = Visibility.Visible,
                    ImgCambio = Visibility.Hidden,
                    ValorSobrante = 0,
                    ValorIngresado = transaction.Amount,
                    viewList = new CollectionViewSource(),
                    Denominations = new List<DenominationMoney>(),
                    ValorDispensado = 0
                };

                transaction.Payment = paymentViewModel;

                Activar();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        public void Activar()
        {
            try
            {
                var result = BoxConnection.InitPay(new RequestRedebam
                {
                    base_dev = base_dev,
                    CajaPath = @"C:\Redebam\Cajas5.2.3.exe",
                    cod_cajero = cod_cajero,
                    factura = factura,
                    imp_consu = imp_consu,
                    InFilePath = @"C:\Redebam\IN.txt",
                    IVA = IVA,
                    Monto = Monto,
                    Operacion = Operacion,
                    OutFilePath = @"C:\Redebam\OUT.txt"
                });

                if (result._Status)
                {
                    string ms = string.Concat("Código Autorización: " + result._AutorizationCode+" RRN: " + result._Rrn+" Franquicia: " + result._Franchise+" Tarjeta: " + result._LastNumbers+" Cuotas: " + result._Quotas+" Recibo: " + result._ReceiptNumber);

                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Reference = "",
                        Description = string.Concat("Respuesta exitosa del tafáfono: ", ms),
                        State = 1,
                        Date = DateTime.Now
                    }, ELogType.General);

                    SavePay();
                }
                else
                {
                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Reference = "",
                        Description = string.Concat("Respuesta mala del tafáfono: ", result._Message),
                        State = 1,
                        Date = DateTime.Now
                    }, ELogType.General);

                    Cancelled(string.Concat("Transacción cancelada, por favor intenta de nuevo.", Environment.NewLine, result._Message));
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void Cancelled(string ms)
        {
            try
            {
                Utilities.ShowModal(ms, EModalType.Error, true);

                Utilities.navigator.Navigate(UserControlView.Main);
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

                    Task.Run(async () =>
                    {
                        Thread.Sleep(1000);

                        object dataTransaction = null;

                        switch (transaction.typeTransaction)
                        {
                            case ETypeTransaction.PagoFactura:
                                dataTransaction = new InvoicePayRequest
                                {
                                    payValue = transaction.RealAmount,
                                    reference = transaction.detailsPagoFactura.Referencia
                                };
                                break;
                            case ETypeTransaction.FacturaPrepago:
                                dataTransaction = new PrepaidPayRequest
                                {
                                    payValue = transaction.Amount,
                                    reference = transaction.NumeroMedidor,
                                    transactionID = transaction.ConsecutivoId
                                };
                                break;
                            case ETypeTransaction.PagoMedida:
                                dataTransaction = new PayRequest
                                {
                                    contract = transaction.NumeroContrato,
                                    document = transaction.Document,
                                    transactionID = transaction.ConsecutivoId,
                                    payValue = transaction.Amount
                                };
                                break;
                        }

                        bool response = await ApiIntegration.ReportPay(transaction, dataTransaction);

                        Thread.Sleep(1000);

                        if (!response && Intentos < 5)
                        {
                            Intentos++;
                            AdminPayPlus.CreateConsecutivoDashboard(transaction);
                            this.paymentViewModel.StatePay = false;
                            SavePay();
                            return;
                        }

                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            this.Opacity = 1;
                            loading.Close();
                        });
                        GC.Collect();

                        if (response)
                        {
                            this.transaction.statePaySuccess = true;
                            transaction.State = ETransactionState.Success;
                            Utilities.navigator.Navigate(UserControlView.PaySuccess, transaction);
                        }
                        else
                        {
                            //CancelTransaction();
                        }
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
            }
        }
        #endregion
    }
}
