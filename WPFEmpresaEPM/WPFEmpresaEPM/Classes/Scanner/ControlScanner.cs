using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEmpresaEPM.Classes.Scanner
{
   public class ControlScanner
    {
        #region Serial ports
        private SerialPort _BarcodeReader;
        #endregion

        #region Callbacks
        public Action<string> callbackScanner;
        public Action<string> callbackErrorScanner;
        #endregion

        #region Variables
        public int flagScanner = 0;
        #endregion

        public ControlScanner()
        {
            if (_BarcodeReader == null)
            {
                _BarcodeReader = new SerialPort();
            }
        }

        #region Methods
        /// <summary>
        ///  Método para inciar el puerto del scanner
        /// </summary>
        public void InitializePortScanner(string portName, int baudrate)
        {
            try
            {
                if (!_BarcodeReader.IsOpen)
                {
                    _BarcodeReader.PortName = portName;
                    _BarcodeReader.BaudRate = baudrate;
                    _BarcodeReader.Open();
                    _BarcodeReader.ReadTimeout = 200;
                    //_BarcodeReader.DtrEnable = true;
                    //_BarcodeReader.RtsEnable = true;
                    _BarcodeReader.DataReceived += new SerialDataReceivedEventHandler(Scanner_DataReceived);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void ClosePortScanner()
        {
            if (_BarcodeReader.IsOpen)
            {
                _BarcodeReader.Close();
            }
        }
        #endregion

        #region Listeners

        /// <summary>
        /// Método que escucha la respuesta del puerto del scanner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scanner_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (flagScanner == 0)
                {
                    flagScanner = 1;
                    var data = _BarcodeReader.ReadExisting();
                    proccessResponseScanner(data);
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Proccess Responses
        private void proccessResponseScanner(string response)
        {
            try
            {
                string referente = response.Substring(20, 12);
                ulong trueResult;
                if (ulong.TryParse(referente, out trueResult))
                {
                    flagScanner = 1;
                    callbackScanner?.Invoke(trueResult.ToString());
                    ClosePortScanner();
                }
                else
                {
                    flagScanner = 1;
                    ClosePortScanner();
                    callbackErrorScanner?.Invoke("Por favor, escanee el código de barras ocultando el código QR que se encuentra al lado.");
                }

            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}
