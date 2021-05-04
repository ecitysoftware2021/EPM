using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Classes.UseFull;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Services.ObjectIntegration;
using System.Windows.Data;
using System.Windows.Input;

namespace WPFEmpresaEPM.UserControls.PagoFactura
{
    /// <summary>
    /// Interaction logic for InvoiceListUserControl.xaml
    /// </summary>
    public partial class InvoiceListUserControl : UserControl
    {
        #region "Referencias"
        private Transaction transaction;
        private CollectionViewSource view;
        private ObservableCollection<DetailsPagoFactura> lstPager;
        private DetailsPagoFactura ProductsSelected;
        private TimerGeneric timer;
        #endregion

        public InvoiceListUserControl(Transaction ts)
        {
            InitializeComponent();

            try
            {
                transaction = ts;
                grvSupport.Content = Utilities.UCSupport;
                view = new CollectionViewSource();
                lstPager = new ObservableCollection<DetailsPagoFactura>();
                ProductsSelected = new DetailsPagoFactura();
                ActivateTimer();
                InitView();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        #region "Métodos"
        private void InitView()
        {
            try
            {
                foreach (var product in transaction.listFacturas.IFX)
                {
                    lstPager.Add(new DetailsPagoFactura
                    {
                        img = GetImage(false),
                        ValorPagar = Convert.ToDecimal(product.PaySvcRs.PmtAddRs.PmtInfo.RemitInfo.CurAmt.Amt),
                        Referencia = product.PaySvcRs.PmtAddRs.PmtInfo.RemitInfo.BillId,
                        FechaLimite = product.PaySvcRs.PmtAddRs.PmtInfo.RemitInfo.BillDt,
                        NumeroCuenta = product.PaySvcRs.PmtAddRs.PmtInfo.RemitInfo.BillingAcct,
                        Direccion = product.PaySvcRs.PmtAddRs.PmtInfo.RemitInfo.BillRefInfo
                    });
                }

                if (lstPager.Count > 0)
                {
                    view.Source = lstPager;
                    lv_Products.DataContext = view;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private string GetImage(bool flag)
        {
            try
            {
                if (!flag)
                {
                    return "/Images/Others/circle.png";
                }

                return "/Images/Others/ok.png";
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return string.Empty;
        }
        #endregion

        #region "Eventos"
        private void BtnBack_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.ConsultPagoFactura, this.transaction);
        }

        private void BtnExit_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Main);
        }

        private void btnPagar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (transaction.Amount > 0)
                {
                    SetCallBacksNull();
                    timer.CallBackStop?.Invoke(1);
                    Utilities.navigator.Navigate(UserControlView.DetailsPagoFactura, transaction);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void btnCancelar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Main);
        }

        private void ListViewItem_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                    var service = (DetailsPagoFactura)(sender as ListViewItem).Content;

                    ProductsSelected.img = GetImage(false);

                    service.img = GetImage(true);

                    lv_Products.Items.Refresh();

                    ProductsSelected = service;

                    transaction.detailsPagoFactura = service;
                    
                    transaction.Amount = Utilities.RoundValue(service.ValorPagar, true);

                    transaction.RealAmount = service.ValorPagar;
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
