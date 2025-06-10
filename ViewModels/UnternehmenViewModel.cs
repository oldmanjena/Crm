using Crm.Klassen;      // Für RelayCommand
using Crm.Models;       // Für UnternehmenModel
using Crm.Views;        // Für Fenster wie UnternehmenWizard, AbteilungErfassen
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

                    // Command-Status aktualisieren
                    if (OpenAbteilungErfassenCommand is RelayCommand rc1)
                    {
                        rc1.RaiseCanExecuteChanged();
                    }
                    if (BearbeiteUnternehmenCommand is RelayCommand rc2)
                    {
                        rc2.RaiseCanExecuteChanged();
                    }
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

        private void BearbeiteUnternehmen()
        {
            if (AusgewaehltesUnternehmen == null)
            {
                MessageBox.Show("Bitte wählen Sie ein Unternehmen aus.");
                return;
            }

            var dialog = new UnternehmenBearbeiten(AusgewaehltesUnternehmen);
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                // Nach Bearbeitung: Liste neu laden
                UnternehmenListe.Clear();
                foreach (var u in DatenbankService.LadeAlleUnternehmenMitAbteilungen())
                    UnternehmenListe.Add(u);
            }
        }

        private void OpenUnternehmenWizard()
        {
            var fenster = new UnternehmenErfassenFenster();
            fenster.Owner = Application.Current.MainWindow;
            fenster.ShowDialog();

            // Liste neu laden
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
        }
    }
}