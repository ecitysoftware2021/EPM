using DLL_Conexion_Caja_Redebam;
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

namespace WPFEmpresaEPM.UserControls
{
    /// <summary>
    /// Interaction logic for DatafonoUserControl.xaml
    /// </summary>
    public partial class DatafonoUserControl : UserControl
    {
        public DatafonoUserControl()
        {
            InitializeComponent();
        }

        public static string Operacion { get; } = "0";
        public static string Monto { get; } = "100";
        public static string IVA { get; } = "0";
        public static string factura { get; } = "1234";
        public static string base_dev { get; } = "0";
        public static string imp_consu { get; } = "0";
        public static string cod_cajero { get; } = "58";

        public static void ExeDatafono()
        {

            var result = BoxConnection.InitPay(new RequestRedebam
            {
                base_dev = base_dev,
                CajaPath = @"C:\Redebam\Cajas5.2.3.exe",
                cod_cajero = cod_cajero,
                factura = factura,
                imp_consu = imp_consu,
                InFilePath = @"C:\Redebam\IN.txt",
                IVA = IVA,
                Monto = Monto,
                Operacion = Operacion,
                OutFilePath = @"C:\Redebam\OUT.txt"
            });

            if (result._Status)
            {
                Console.WriteLine("Transacción Exitosa");
                Console.WriteLine("Código Autorización: " + result._AutorizationCode);
                Console.WriteLine("RRN: " + result._Rrn);
                Console.WriteLine("Franquicia: " + result._Franchise);
                Console.WriteLine("Tarjeta: " + result._LastNumbers);
                Console.WriteLine("Cuotas: " + result._Quotas);
                Console.WriteLine("Recibo: " + result._ReceiptNumber);
            }
            else
            {
                Console.WriteLine("Recibo: " + result._Message);
            }

            Console.ReadKey();

        }
    }
}
