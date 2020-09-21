using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using WPFEmpresaEPM.Classes;

namespace WPFEmpresaEPM.UserControls
{
    /// <summary>
    /// Interaction logic for ConsultUserControl.xaml
    /// </summary>
    public partial class ConsultUserControl : UserControl
    {
        #region "Constructor"
        public ConsultUserControl()
        {
            InitializeComponent();
        }
        #endregion

        #region "Eventos"
        private void btnConsult_TouchDown(object sender, TouchEventArgs e)
        {
            Consult();
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
        #endregion

        #region "Métodos"
        private void Consult()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        #endregion

        private void TxtIdentification_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TxtIdentification.Text.Length > 15)
                {
                    TxtIdentification.Text = TxtIdentification.Text.Remove(15, 1);
                    return;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
    }
}
