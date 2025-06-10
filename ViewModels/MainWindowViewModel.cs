using Crm.Klassen;
using Crm.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Crm.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private object _aktuellesView;

        public object AktuellesView
        {
            get => _aktuellesView;
            set { _aktuellesView = value; OnPropertyChanged(); }
        }

        // Konstruktor, Befüllung etc.

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}