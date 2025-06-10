using Crm.Klassen;      // RelayCommand
using Crm.Models;       // UnternehmenModel, AbteilungModel, KontaktModel
using Crm.Views;        // Fenster
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Crm.ViewModels
{
    public class UnternehmenViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<UnternehmenModel> UnternehmenListe { get; set; }
        public ObservableCollection<AbteilungModel> GefilterteAbteilungen { get; set; } = new();
        public ObservableCollection<KontaktModel> GefilterteKontakte { get; set; } = new();

        private UnternehmenModel _ausgewaehltesUnternehmen;

        public UnternehmenModel AusgewaehltesUnternehmen
        {
            get => _ausgewaehltesUnternehmen;
            set
            {
                if (_ausgewaehltesUnternehmen != value)
                {
                    _ausgewaehltesUnternehmen = value;
                    OnChanged();
                    LadeAbteilungen();

                    // Commands aktualisieren
                    if (OpenAbteilungErfassenCommand is RelayCommand rc1)
                        rc1.RaiseCanExecuteChanged();
                    if (BearbeiteUnternehmenCommand is RelayCommand rc2)
                        rc2.RaiseCanExecuteChanged();
                }
            }
        }

        private AbteilungModel _ausgewaehlteAbteilung;

        public AbteilungModel AusgewaehlteAbteilung
        {
            get => _ausgewaehlteAbteilung;
            set
            {
                if (_ausgewaehlteAbteilung != value)
                {
                    _ausgewaehlteAbteilung = value;
                    OnChanged();
                    LadeKontakte();
                }
            }
        }

        public ICommand OpenUnternehmenWizardCommand { get; }
        public ICommand OpenAbteilungErfassenCommand { get; }
        public ICommand BearbeiteUnternehmenCommand { get; }

        public UnternehmenViewModel()
        {
            UnternehmenListe = new ObservableCollection<UnternehmenModel>(DatenbankService.LadeAlleUnternehmenMitAbteilungen());

            OpenUnternehmenWizardCommand = new RelayCommand(_ => OpenUnternehmenWizard());
            OpenAbteilungErfassenCommand = new RelayCommand(_ => OpenAbteilungErfassen(), _ => AusgewaehltesUnternehmen != null);
            BearbeiteUnternehmenCommand = new RelayCommand(_ => BearbeiteUnternehmen(), _ => AusgewaehltesUnternehmen != null);
        }

        private void LadeAbteilungen()
        {
            GefilterteAbteilungen.Clear();
            GefilterteKontakte.Clear(); // Auch Kontakte zurücksetzen

            if (AusgewaehltesUnternehmen != null)
            {
                var abteilungen = DatenbankService.LadeAbteilungenZuUnternehmen(AusgewaehltesUnternehmen.unternehmen_id);
                foreach (var abt in abteilungen)
                    GefilterteAbteilungen.Add(abt);
            }
        }

        private void LadeKontakte()
        {
            GefilterteKontakte.Clear();
            if (AusgewaehlteAbteilung != null)
            {
                var kontakte = DatenbankService.LadeKontakteZuAbteilung((int)AusgewaehlteAbteilung.AbteilungId);
                foreach (var kontakt in kontakte)
                    GefilterteKontakte.Add(kontakt);
            }
        }

        private void BearbeiteUnternehmen()
        {
            if (AusgewaehltesUnternehmen == null)
            {
                MessageBox.Show("Bitte wählen Sie ein Unternehmen aus.");
                return;
            }

            var dialog = new UnternehmenBearbeiten(AusgewaehltesUnternehmen)
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                UnternehmenListe.Clear();
                foreach (var u in DatenbankService.LadeAlleUnternehmenMitAbteilungen())
                    UnternehmenListe.Add(u);

                // Abteilungen ggf. neu laden
                LadeAbteilungen();
            }
        }

        private void OpenUnternehmenWizard()
        {
            var fenster = new UnternehmenErfassenFenster
            {
                Owner = Application.Current.MainWindow
            };
            fenster.ShowDialog();

            UnternehmenListe.Clear();
            foreach (var u in DatenbankService.LadeAlleUnternehmenMitAbteilungen())
                UnternehmenListe.Add(u);
        }

        private void OpenAbteilungErfassen()
        {
            if (AusgewaehltesUnternehmen == null)
            {
                MessageBox.Show("Bitte wählen Sie zuerst ein Unternehmen aus.");
                return;
            }

            var abteilungErfassen = new AbteilungErfassen(AusgewaehltesUnternehmen);
            // abteilungErfassen.Owner = Application.Current.MainWindow;
            //  abteilungErfassen.ShowDialog();

            // Optional: Unternehmen inkl. Abteilungen neu laden
            UnternehmenListe.Clear();
            foreach (var u in DatenbankService.LadeAlleUnternehmenMitAbteilungen())
                UnternehmenListe.Add(u);
            LadeAbteilungen();
        }
    }
}