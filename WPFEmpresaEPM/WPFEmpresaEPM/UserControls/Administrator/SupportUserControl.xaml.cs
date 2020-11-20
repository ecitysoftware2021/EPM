using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            txtNumberPhone.Text = Utilities.GetConfiguration("NumbersPhone");
        }
    }
}
