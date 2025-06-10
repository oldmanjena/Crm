using Crm.ViewModels;
using Fluent.Localization.Languages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crm.Models;

namespace Crm.ViewModels
{
    public class KalenderViewModel : ViewModelBase
    {
        private DateTime? _selectedDate;

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    LoadTermineForDate(value);
                }
            }
        }

        public ObservableCollection<Termin> Termine { get; } = new();

        private void LoadTermineForDate(DateTime? date)
        {
            Termine.Clear();
            if (date == null)
                return;

            // Beispiel-Daten
            Termine.Add(new Termin { Titel = "Meeting", Uhrzeit = "09:00" });
            Termine.Add(new Termin { Titel = "Geburtstag Max", Uhrzeit = "Ganztägig" });
        }
    }

    public class Termin
    {
        public string Titel { get; set; }
        public string Uhrzeit { get; set; }
    }
}