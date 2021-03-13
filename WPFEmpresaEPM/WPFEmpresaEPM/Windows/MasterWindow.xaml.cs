using System;
using System.Reflection;
using System.Windows;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Models;
using WPFEmpresaEPM.UserControls.Administrator;

namespace WPFEmpresaEPM.Windows
{
    /// <summary>
    /// Lógica de interacción para MasterWindow.xaml
    /// </summary>
    public partial class MasterWindow : Window
    {
        #region "Constructor"
        public MasterWindow()
        {
            InitializeComponent();

            SetUserControl();
        }
        #endregion

        #region "Métodos"
        private void SetUserControl()
        {
            try
            {
                if (Utilities.navigator == null)
                {
                    Utilities.navigator = new Navigation();
                }

                string a = Utilities.EncryptorData("usrapli",true, "WPFEmpresaEPM");
                string b = Utilities.EncryptorData("1Cero12019$/*", true,"WPFEmpresaEPM");
                string c = Utilities.EncryptorData("Pay+ EPM Punto Facil Rionegro", true,"WPFEmpresaEPM");
                string d = Utilities.EncryptorData("EmpresaPublicadeMedellin2021%", true,"WPFEmpresaEPM");

                WPKeyboard.Keyboard.ConsttrucKeyyboard(WPKeyboard.Keyboard.EStyle.style_2);

                Utilities.navigator.Navigate(UserControlView.Config);

                DataContext = Utilities.navigator;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion
    }
}
