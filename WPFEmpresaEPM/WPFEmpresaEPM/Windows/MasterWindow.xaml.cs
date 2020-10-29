using System;
using System.Reflection;
using System.Windows;
using WPFEmpresaEPM.Classes;
using WPFEmpresaEPM.Models;

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

                string a = Encryptor.Ecity.Dll.Encryptor.Encrypt("usrapli", "WPFEmpresaEPM");
                string b = Encryptor.Ecity.Dll.Encryptor.Encrypt("1Cero12019$/*", "WPFEmpresaEPM");
                string c = Encryptor.Ecity.Dll.Encryptor.Encrypt("Ecity.Software", "WPFEmpresaEPM");
                string d = Encryptor.Ecity.Dll.Encryptor.Encrypt("Ecitysoftware2019#", "WPFEmpresaEPM");
                string e = Encryptor.Ecity.Dll.Encryptor.Encrypt("http://181.143.126.126:41400/", "WPFEmpresaEPM");
                string es = Encryptor.Ecity.Dll.Encryptor.Encrypt("Pay+ EPM Ed. Inteligente 1", "WPFEmpresaEPM");
                string ess = Encryptor.Ecity.Dll.Encryptor.Encrypt("EmpresasPublicasdeMedellin2020/", "WPFEmpresaEPM");
                //USERNAME: Pay + EPM Ed.Inteligente 1
                //PASSWORD: EmpresasPublicasdeMedellin2020 /

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
