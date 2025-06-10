using Crm.Klassen;
using Crm.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Crm.ViewModels
{
    public class AnUnternehmenViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ObservableCollection<UnternehmenModel> UnternehmenListe { get; set; }

        private UnternehmenModel _ausgewaehltesUnternehmen;

        public UnternehmenModel AusgewaehltesUnternehmen
        {
            get => _ausgewaehltesUnternehmen;
            set
            {
                if (_ausgewaehltesUnternehmen != value)
                {
                    _ausgewaehltesUnternehmen = value;
                    OnChanged(nameof(AusgewaehltesUnternehmen));
                }
            }
        }

        public AnUnternehmenViewModel()
        {
            // Alle Unternehmen inkl. Abteilungen laden
            UnternehmenListe = new ObservableCollection<UnternehmenModel>(
                DatenbankService.LadeAlleUnternehmenMitAbteilungen()
            );
        }
    }
}