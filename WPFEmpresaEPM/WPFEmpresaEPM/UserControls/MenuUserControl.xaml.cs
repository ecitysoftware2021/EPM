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
                string option = (sender as TextBlock).Tag.ToString();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion
    }
}
