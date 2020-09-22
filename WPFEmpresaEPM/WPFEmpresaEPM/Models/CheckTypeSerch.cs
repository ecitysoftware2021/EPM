using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEmpresaEPM.Models
{
    public class CheckTypeSerch : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        private string _Referencia;

        public string Referencia
        {
            get
            {
                return _Referencia;
            }
            set
            {
                _Referencia = value;
                OnPropertyRaised("Referencia");
            }
        }

        private string _Numero;

        public string Numero
        {
            get
            {
                return _Numero;
            }
            set
            {
                _Numero = value;
                OnPropertyRaised("Numero");
            }
        }
    }
}