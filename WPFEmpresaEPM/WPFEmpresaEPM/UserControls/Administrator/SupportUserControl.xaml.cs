using System.Windows.Controls;
using WPFEmpresaEPM.Classes;

namespace WPFEmpresaEPM.UserControls.Administrator
{
    /// <summary>
    /// Interaction logic for SupportUserControl.xaml
    /// </summary>
    public partial class SupportUserControl : UserControl
    {
        public SupportUserControl()
        {
            InitializeComponent();

            try
            {
                txtNumberPhone.Text = AdminPayPlus.DataPayPlus.PayPadConfiguration.ExtrA_DATA.dataComplementary.NumbersPhone;

                if (AdminPayPlus.DataConfiguration == null)
                {
                    txtNumberMachine.Text = AdminPayPlus.DataPayPlus.PayPadConfiguration.ExtrA_DATA.dataComplementary.NumbersSerial;
                }
                else
                {
                    txtNumberMachine.Text = AdminPayPlus.DataConfiguration.ID_PAYPAD.ToString();
                }
            }
            catch { }
        }
    }
}
