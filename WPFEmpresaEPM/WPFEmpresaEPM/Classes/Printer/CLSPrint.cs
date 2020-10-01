using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WPFEmpresaEPM.Classes.Printer
{
    public class CLSPrint
    {
        private Utilities utilities;
        private SolidBrush sb;
        private Font fTitles;
        private Font fGIBTitles;
        private Font fContent;

        public CLSPrint()
        {
            utilities = new Utilities();
            sb = new SolidBrush(Color.Black);
            fTitles = new Font("Arial", 8, FontStyle.Bold);
            fGIBTitles = new Font("Arial", 12, FontStyle.Bold);
            fContent = new Font("Arial", 8, FontStyle.Regular);
        }

        #region "Referencias"
        //______________PREPAGADA_____________________//
        public string PIN { get; set; }
        public string VR { get; set; }
        public string APL_DEU { get; set; }
        public string SALD { get; set; }
        public string APL_SA { get; set; }
        public string SALDO_SA { get; set; }
        public string VR_NETO { get; set; }
        public string KWH { get; set; }
        public string TARIFA { get; set; }
        public string SUB_SID { get; set; }
        public string CONTRIB { get; set; }
        public string OTROS { get; set; }
        public string SUBCATEGORIA { get; set; }
        public string OPSA { get; set; }
        public string TEL { get; set; }
        public string MED { get; set; }
        public string FECHA { get; set; }
        public string PUNTO_DE_VENTA { get; set; }
        //_____________FIN_PREPAGADA__________________//
        //--------------------------------------------//
        //_______________ MEDIDA_____________________//
        public string REFERENTE_DE_PAGO { get; set; }
        public string FECHA_DE_PAGO { get; set; }
        public string CONTRATO { get; set; }
        public string IDENTIFICACION_C { get; set; }
        public string VALOR_TOTAL_FACT { get; set; }
        public string SALDO_ANTERIOR_VENCIDO { get; set; }
        public string SALDO_ANTERIOR_VIGENTE { get; set; }
        public string SALDO_ANTERIOR_TOTAL { get; set; }
        public string VALOR_PAGO { get; set; }
        public string NUEVO_SALDO { get; set; }
        public string SALDO_FAVOR { get; set; }
        public string NUM_REST_PAGOS { get; set; }
        public string FECHA_DE_VENCI_FACT { get; set; }
        public string SERVICIO_A_SUSPENDER { get; set; }
        //_______________FIN_MEDIDA__________________//
        //--------------------------------------------//
        //________________FACTURA_____________________//
        public string VALOR { get; set; }
        public string FACTURA { get; set; }
        public string FECHA_FACTURA { get; set; }
        public string HORA_FACTURA { get; set; }
        //_______________FIN_FACTURA__________________//
        #endregion

        #region "Métodos"
        public void ImprimirComprobante(ETypeTransaction type)
        {
            try
            {
                PrintController printcc = new StandardPrintController();
                PrintDocument pd = new PrintDocument();
                pd.PrintController = printcc;
                PaperSize ps = new PaperSize("Recibo Pago", 475, 470);

                switch (type)
                {
                    case ETypeTransaction.PagoFactura:
                        pd.PrintPage += new PrintPageEventHandler(PrintPagePagoFactura);
                        break;
                    case ETypeTransaction.FacturaPrepago:
                        pd.PrintPage += new PrintPageEventHandler(PrintPageEnergiaPrepagada);
                        break;
                    case ETypeTransaction.PagoMedida:
                        pd.PrintPage += new PrintPageEventHandler(PrintPageAtuMedida);
                        break;
                }
                
                pd.Print();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "CLSPrint", ex, ex.ToString());
            }
        }

        private void PrintPageEnergiaPrepagada(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                int y = 0;
                int sum = 30;
                int x = 150;

                //string RutaIMG = Utilities.GetConfiguration("ImageBoucher");
                //g.DrawImage(Image.FromFile(RutaIMG), y += sum + 20, 0);

                g.DrawString("COMPROBANTE DE VENTA", fGIBTitles, sb, 25, y += sum);
                g.DrawString("Nit 890.904.996-1", fContent, sb, 95, y += sum);
                g.DrawString("ENERGIA PREPAGO MERCADO REG", fContent, sb, 35, y += sum - 10);
                g.DrawString("PIN:", fGIBTitles, sb, 120, y += sum);
                g.DrawString(PIN, fGIBTitles, sb, 30, y += sum - 10);
                g.DrawString("VR:" + VR, fGIBTitles, sb, 95, y += sum);
                g.DrawString("APL.DEU:" + APL_DEU + "   " + "APL.SA:" + APL_SA, fContent, sb, 65, y += sum);
                g.DrawString("VR NETO:" + VR_NETO + "    KWH:" + KWH, fContent, sb, 50, y += sum - 10);
                g.DrawString("TARIFA:" + TARIFA + "    SUB SID:" + SUB_SID, fContent, sb, 65, y += sum - 10);
                g.DrawString("CONTRIB:" + CONTRIB + "    OTROS:" + OTROS, fContent, sb, 65, y += sum - 10);
                g.DrawString("SUBCATEGORIA:" + SUBCATEGORIA, fContent, sb, 80, y += sum - 10);
                g.DrawString("MED:" + MED, fGIBTitles, sb, 60, y += sum);
                g.DrawString("FECHA  " + FECHA, fContent, sb, 60, y += sum);
                g.DrawString("PUNTO DE VENTA:" + PUNTO_DE_VENTA, fContent, sb, 80, y += sum - 10);
                g.DrawString("========================================", fContent, sb, 10, y += sum);
                g.DrawString("MAYOR INFORMACIÓN LLAMAR A  LINEA", fTitles, sb, 10, y += sum + 10);
                g.DrawString("GRATUITA DE ATENCIÓN EMP: 01 8000 415115", fTitles, sb, 10, y += 20);
                g.DrawString("RECUERDE SIEMPRE ESPERAR LA TIRILLA DE ", fTitles, sb, 10, y += sum - 10);
                g.DrawString("SOPORTE DE PAGO, ES EL ÚNICO DOCUMENTO", fTitles, sb, 10, y += 20);
                g.DrawString("QUE LO RESPALDA.", fTitles, sb, 10, y += 20);

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "CLSPrint", ex, ex.ToString());
            }
        }

        private void PrintPageAtuMedida(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                int y = 0;
                int sum = 30;
                int x = 150;

                //string RutaIMG = Utilities.GetConfiguration("ImageBoucher");
                //g.DrawImage(Image.FromFile(RutaIMG), y += sum + 20, 0);

                g.DrawString("COMPROBANTE DE VENTA", fGIBTitles, sb, 25, y += sum);
                g.DrawString("Nit 890.904.996-1", fTitles, sb, 95, y += sum);
                g.DrawString("PAGA A TU MEDIDA", fGIBTitles, sb, 60, y += sum - 10);
                g.DrawString("Referente de pago:", fTitles, sb, 10, y += sum);
                g.DrawString(REFERENTE_DE_PAGO, fContent, sb, x, y);
                g.DrawString("Fecha pago:", fTitles, sb, 10, y += sum);
                g.DrawString(FECHA_DE_PAGO, fContent, sb, x, y);
                g.DrawString("Contrato:", fTitles, sb, 10, y += sum);
                g.DrawString(CONTRATO, fContent, sb, x, y);
                g.DrawString("Identifiación cliente:", fTitles, sb, 10, y += sum);
                g.DrawString(IDENTIFICACION_C, fContent, sb, x, y);
                g.DrawString("Valor total fact:", fTitles, sb, 10, y += sum);
                g.DrawString(VALOR_TOTAL_FACT, fContent, sb, x, y);
                g.DrawString("Saldo anterior vencido:", fTitles, sb, 10, y += sum);
                g.DrawString(SALDO_ANTERIOR_VENCIDO, fContent, sb, x, y);
                g.DrawString("Saldo anterior vigente:", fTitles, sb, 10, y += sum);
                g.DrawString(SALDO_ANTERIOR_VIGENTE, fContent, sb, x, y);
                g.DrawString("Saldo anterior total:", fTitles, sb, 10, y += sum);
                g.DrawString(SALDO_ANTERIOR_TOTAL, fContent, sb, x, y);
                g.DrawString("Valor pago:", fTitles, sb, 10, y += sum);
                g.DrawString(VALOR_PAGO, fContent, sb, x, y);
                g.DrawString("Nuevo saldo:", fTitles, sb, 10, y += sum);
                g.DrawString(NUEVO_SALDO, fContent, sb, x, y);
                g.DrawString("Saldo a favor:", fTitles, sb, 10, y += sum);
                g.DrawString(SALDO_FAVOR, fContent, sb, x, y);
                g.DrawString("Num restante pagos:", fTitles, sb, 10, y += sum);
                g.DrawString(NUM_REST_PAGOS, fContent, sb, x, y);
                g.DrawString("Fecha vencimiento fact:", fTitles, sb, 10, y += sum);
                g.DrawString(FECHA_DE_VENCI_FACT, fContent, sb, x, y);
                g.DrawString("Servicios a suspender:", fTitles, sb, 10, y += sum);
                g.DrawString(SERVICIO_A_SUSPENDER, fContent, sb, x, y);
                g.DrawString("Punto de venta:", fTitles, sb, 10, y += sum);
                g.DrawString("1", fContent, sb, x, y);
                g.DrawString("========================================", fContent, sb, 10, y += sum);
                g.DrawString("MAYOR INFORMACIÓN LLAMAR A  LINEA", fTitles, sb, 10, y += sum + 10);
                g.DrawString("GRATUITA DE ATENCIÓN EMP: 01 8000 415115", fTitles, sb, 10, y += 20);
                g.DrawString("RECUERDE SIEMPRE ESPERAR LA TIRILLA DE ", fTitles, sb, 10, y += sum - 10);
                g.DrawString("SOPORTE DE PAGO, ES EL ÚNICO DOCUMENTO", fTitles, sb, 10, y += 20);
                g.DrawString("QUE LO RESPALDA.", fTitles, sb, 10, y += 20);

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "CLSPrint", ex, ex.ToString());
            }
        }

        private void PrintPagePagoFactura(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                int y = 0;
                int sum = 30;
                int x = 150;

                //string RutaIMG = Utilities.GetConfiguration("ImageBoucher");
                //g.DrawImage(Image.FromFile(RutaIMG), y += sum + 20, 0);

                g.DrawString("COMPROBANTE DE VENTA", fGIBTitles, sb, 25, y += sum);
                g.DrawString("Nit 890.904.996-1", fTitles, sb, 95, y += sum);
                g.DrawString("PAGO DE FACTURA", fGIBTitles, sb, 60, y += sum - 10);
                g.DrawString("Número medido:", fTitles, sb, 10, y += sum + 20);
                g.DrawString(FACTURA, fContent, sb, x, y);
                g.DrawString("Fecha de pago:", fTitles, sb, 10, y += sum);
                g.DrawString(FECHA_FACTURA, fContent, sb, x, y);
                g.DrawString("Hora de pago:", fTitles, sb, 10, y += sum);
                g.DrawString(HORA_FACTURA, fContent, sb, x, y);
                g.DrawString("Punto de venta:", fTitles, sb, 10, y += sum);
                g.DrawString("1", fContent, sb, x, y);
                g.DrawString("Estado:", fTitles, sb, 10, y += sum);
                g.DrawString("Aprobado", fContent, sb, x, y);
                g.DrawString("Valor:", fTitles, sb, 10, y += sum);
                g.DrawString(VALOR, fContent, sb, x, y);
                g.DrawString("========================================", fContent, sb, 10, y += sum);
                g.DrawString("MAYOR INFORMACIÓN LLAMAR A  LINEA", fTitles, sb, 10, y += sum + 10);
                g.DrawString("GRATUITA DE ATENCIÓN EMP: 01 8000 415115", fTitles, sb, 10, y += 20);
                g.DrawString("RECUERDE SIEMPRE ESPERAR LA TIRILLA DE ", fTitles, sb, 10, y += sum - 10);
                g.DrawString("SOPORTE DE PAGO, ES EL ÚNICO DOCUMENTO", fTitles, sb, 10, y += 20);
                g.DrawString("QUE LO RESPALDA.", fTitles, sb, 10, y += 20);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "CLSPrint", ex, ex.ToString());
            }
        }
        #endregion
    }
}
