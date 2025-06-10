using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.ViewModels
{
    public class AbteilungViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _strasse;
        private string _plz;
        private string _ort;
        private string _land;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnChanged(nameof(Name));
            }
        }

        public string Strasse
        {
            get => _strasse;
            set
            {
                _strasse = value;
                OnChanged(nameof(Strasse));
            }
        }

        public string PLZ
        {
            get => _plz;
            set
            {
                _plz = value;
                OnChanged(nameof(PLZ));
            }
        }

        public string Ort
        {
            get => _ort;
            set
            {
                _ort = value;
                OnChanged(nameof(Ort));
            }
        }

        public string Land
        {
            get => _land;
            set
            {
                _land = value;
                OnChanged(nameof(Land));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}