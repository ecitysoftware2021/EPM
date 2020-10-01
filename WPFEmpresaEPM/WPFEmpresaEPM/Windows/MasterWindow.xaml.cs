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

                string a = Encryptor.Encrypt("usrapli");
                string b = Encryptor.Encrypt("1Cero12019$/*");
                string c = Encryptor.Encrypt("Ecity.Software");
                string d = Encryptor.Encrypt("Ecitysoftware2019#");
                string e = Encryptor.Encrypt("http://181.143.126.126:41400/");
                string es = Encryptor.Encrypt("Pay+ EPM Ed. Inteligente 1");
                string ess = Encryptor.Encrypt("EmpresasPublicasdeMedellin2020/");
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
