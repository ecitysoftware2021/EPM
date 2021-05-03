using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Models;
using System.Windows;
using WPFEmpresaEPM.Resources;
using System.Threading.Tasks;
using WPFEmpresaEPM.Services;
using WPFEmpresaEPM.Classes.UseFull;
using System.Threading;

namespace WPFEmpresaEPM.UserControls.PagoFactura
{
    /// <summary>
    /// Interaction logic for ConsultUserControl.xaml
    /// </summary>
    public partial class ConsultUserControl : UserControl
    {
        #region "Referencias"
        private Transaction transaction;
        private CheckTypeSerch check;
        private TimerGeneric timer;
        #endregion

        #region "Constructor"
        public ConsultUserControl(Transaction ts)
        {
            InitializeComponent();

            try
            {
                transaction = ts;
                check = new CheckTypeSerch();
                grvSupport.Content = Utilities.UCSupport;
                //check.Numero = GetImage(true);
                //check.Referencia = GetImage(false);
                //this.DataContext = check;
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
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void BtnExit_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Main);
        }

        private void btnConsult_TouchDown(object sender, TouchEventArgs e)
        {
            Validate();
        }

        private void ImgDeleteAll_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                TxtIdentification.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void Keyboard_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                Image image = (Image)sender;
                string Tag = image.Tag.ToString();
                TxtIdentification.Text += Tag;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void ImgDelete_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                string val = TxtIdentification.Text;

                if (val.Length > 0)
                {
                    TxtIdentification.Text = val.Remove(val.Length - 1);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void TxtIdentification_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                txtError.Visibility = Visibility.Hidden;

                if (TxtIdentification.Text.Length > 12)
                {
                    TxtIdentification.Text = TxtIdentification.Text.Remove(12, 1);
                    return;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void btnReferente_TouchDown(object sender, TouchEventArgs e)
        {
            transaction.typeSearch = ETypeSearch.ReferenteDePago;
            txTitleSearch.Text = "Ingrese el referente de pago o escanee la factura";
            check.Numero = GetImage(false);
            check.Referencia = GetImage(true);
        }

        private void btnNumero_TouchDown(object sender, TouchEventArgs e)
        {
            transaction.typeSearch = ETypeSearch.NumeroDeContrato;
            txTitleSearch.Text = "Ingrese el número de contrato";
            check.Numero = GetImage(true);
            check.Referencia = GetImage(false);
        }
        #endregion

        #region "Métodos"
        private void Validate()
        {
            try
            {
                if (string.IsNullOrEmpty(TxtIdentification.Text))
                {
                    txtError.Visibility = Visibility.Visible;
                }
                else
                {
                    txtError.Visibility = Visibility.Hidden;
                    transaction.Document = TxtIdentification.Text;
                    Consult();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void Consult()
        {
            try
            {
                Task.Run(async () =>
                {
                    Thread.Sleep(500);

                    if (transaction.Document.Length > 7)
                    {
                        transaction.typeSearch = ETypeSearch.ReferenteDePago;
                    }
                    else
                    {
                        transaction.typeSearch = ETypeSearch.NumeroDeContrato;
                    }
                    var response = await ApiIntegration.SearchPagoFactura(transaction.Document, transaction.typeSearch);

                    Utilities.CloseModal();

                    if (response == null)
                    {

                        Utilities.ShowModal($"No se encontrarón resultados para {transaction.Document}. Por favor vuelve a intentarlo.", EModalType.Error);
                        ActivateScanner();
                        ActivateTimer();
                    }
                    else
                    {
                        transaction.listFacturas = response;
                        transaction.NumeroContrato = (int)transaction.typeSearch;
                        Utilities.navigator.Navigate(UserControlView.InvoiceListPagoFactura, transaction);
                    }
                });
                AdminPayPlus.ControlScanner.callbackScanner = null;
                AdminPayPlus.ControlScanner.callbackErrorScanner = null;
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
                Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload);
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ActivateScanner();
        }

        private void ActivateScanner()
        {
            AdminPayPlus.ControlScanner.callbackScanner = Reference =>
            {
                if (!string.IsNullOrEmpty(Reference))
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        TxtIdentification.Text = Reference;
                        Validate();
                    });

                }
            };
            AdminPayPlus.ControlScanner.callbackErrorScanner = Error =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    SetCallBacksNull();
                    timer.CallBackStop?.Invoke(1);
                    Utilities.ShowModal(Error, EModalType.Error);
                    ActivateScanner();
                    ActivateTimer();
                });
            };

            AdminPayPlus.ControlScanner.flagScanner = 0;
            AdminPayPlus.ControlScanner.InitializePortScanner(Utilities.GetConfiguration("PortScanner"),
                int.Parse(Utilities.GetConfiguration("BaudRateScanner")));
        }
    }
}
