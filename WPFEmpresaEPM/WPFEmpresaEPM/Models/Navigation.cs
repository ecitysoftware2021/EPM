using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Services.Object;
using WPFEmpresaEPM.UserControls;
using WPFEmpresaEPM.UserControls.Administrator;

namespace WPFEmpresaEPM.Models
{
    public class Navigation : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private UserControl _view;

        public UserControl View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(View)));
            }
        }

        public void Navigate(UserControlView newWindow, object data = null, object complement = null) => Application.Current.Dispatcher.Invoke((Action)delegate
        {
            try
            {
                switch (newWindow)
                {
                    case UserControlView.Config:
                        View = new ConfigurateUserControl();
                        break;
                    case UserControlView.Main:
                        View = new MainUserControl();
                        break;
                    case UserControlView.Menu:
                        View = new MenuUserControl();
                        break;
                    case UserControlView.ConsultPagoFactura:
                        View = new UserControls.PagoFactura.ConsultUserControl((Transaction)data);
                        break;
                    case UserControlView.InvoiceListPagoFactura:
                        View = new UserControls.PagoFactura.InvoiceListUserControl((Transaction)data);
                        break;
                    case UserControlView.DetailsPagoFactura:
                        View = new UserControls.PagoFactura.DetailsUserControl((Transaction)data);
                        break;
                    case UserControlView.ConsultPagoMedida:
                        View = new UserControls.PagoMedida.ConsultUserControl((Transaction)data);
                        break;
                    case UserControlView.DetailsPagoMedida:
                        View = new UserControls.PagoMedida.DetailsUserControl((Transaction)data);
                        break;
                    case UserControlView.ConsultPagoPrepago:
                        View = new UserControls.PagoPrepago.ConsultUserControl((Transaction)data);
                        break;
                    case UserControlView.DetailsPagoPrepago:
                        View = new UserControls.PagoPrepago.DetailsUserControl((Transaction)data);
                        break;
                    case UserControlView.PaySuccess:
                        View = new SussesUserControl((Transaction)data);
                        break;
                    case UserControlView.Pay:
                        View = new PaymentUserControl((Transaction)data);
                        break;
                    case UserControlView.Card:
                        View = new DatafonoUserControl((Transaction)data);
                        break;
                    case UserControlView.ReturnMony:
                        View = new ReturnMonyUserControl((Transaction)data);
                        break;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Navigate", ex, ex.ToString());
            }
            GC.Collect();
        });
    }
}