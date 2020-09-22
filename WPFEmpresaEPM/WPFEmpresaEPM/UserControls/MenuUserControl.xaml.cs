using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WPFEmpresaEPM.Classes;
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
        #endregion

        #region "Constructor"
        public MenuUserControl()
        {
            InitializeComponent();
            transaction = new Transaction();
        }
        #endregion

        #region "Eventos"
        private void btnOption_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                string option = (sender as Image).Tag.ToString();

                switch (option)
                {
                    case "1":
                        transaction.typeTransaction = ETypeTransaction.PagoFactura;
                        break;
                    case "2":
                        transaction.typeTransaction = ETypeTransaction.PagoMedida;
                        break;
                    case "3":
                        transaction.typeTransaction = ETypeTransaction.FacturaPrepago;
                        break;
                }

                Utilities.navigator.Navigate(UserControlView.ConsultPagoFactura,transaction);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion
    }
}
