using Crm.Klassen;
using Crm.Models;
using Crm.Views;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Windows;

namespace Crm.ViewModels
{
    public class KontakteViewModel : INotifyPropertyChanged
    {
        private string _anrede;
        private string _vorname;
        private string _nachname;
        private string _email;

        // Unternehmen-Auswahl
        private Unternehmen _ausgewaehltesUnternehmen;

        private Abteilung _ausgewaehlteAbteilung;

        public RelayCommand LadeKontakteCommand { get; }

        public RelayCommand KontaktLoeschenCommand { get; }

        public bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

        private void LadeAlleKontakte()
        {
            KontaktListe.Clear();
            foreach (var kontakt in DatenbankService.LadeAlleKontakte())
                KontaktListe.Add(kontakt);
        }

        public KontakteViewModel()
        {
            if (!IsInDesignMode)
            {
                KontaktListe = new ObservableCollection<KontaktModel>();
                LadeKontakteCommand = new RelayCommand(_ => LadeKontakte());

                LadeKontakte(); // initialer Aufruf

                // 👉 Hierhin verschieben
                KontaktLoeschenCommand = new RelayCommand(KontaktLoeschen, param => IstKontaktAusgewählt);
            }
        }

        public ObservableCollection<KontaktModel> KontaktListe { get; set; } = new();

        private KontaktModel _ausgewählterKontakt;

        public KontaktModel AusgewählterKontakt
        {
            get => _ausgewählterKontakt;
            set
            {
                _ausgewählterKontakt = value;
                OnPropertyChanged(nameof(AusgewählterKontakt));
                OnPropertyChanged(nameof(IstKontaktAusgewählt));
                KontaktLoeschenCommand?.RaiseCanExecuteChanged();
            }
        }

        public bool IstKontaktAusgewählt => AusgewählterKontakt != null;

        private void KontaktLoeschen(object? parameter)
        {
            if (AusgewählterKontakt == null)
                return;

            if (MessageBox.Show($"Kontakt \"{AusgewählterKontakt.Vorname} {AusgewählterKontakt.Nachname}\" wirklich löschen?",
                                "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            // 👉 ConnectionString aus App.config lesen
            var setting = ConfigurationManager.ConnectionStrings["CrmDatabase"];
            if (setting == null)
            {
                MessageBox.Show("ConnectionString 'CrmDatabase' wurde nicht gefunden.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using var connection = new SqlConnection(setting.ConnectionString);
                connection.Open();

                var command = new SqlCommand("DELETE FROM Kontakte WHERE kontakt_id = @KontaktID", connection);
                command.Parameters.AddWithValue("@KontaktID", AusgewählterKontakt.KontaktId);
                command.ExecuteNonQuery();

                KontaktListe.Remove(AusgewählterKontakt);
                AusgewählterKontakt = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Löschen des Kontakts: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string Anrede
        {
            get => _anrede;
            set { _anrede = value; OnPropertyChanged(nameof(Anrede)); }
        }

        public string Vorname
        {
            get => _vorname;
            set { _vorname = value; OnPropertyChanged(nameof(Vorname)); }
        }

        public string Nachname
        {
            get => _nachname;
            set { _nachname = value; OnPropertyChanged(nameof(Nachname)); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        // UnternehmenId für DB Speicherung
        private int? _unternehmenId;

        public int? UnternehmenId
        {
            get => _unternehmenId;
            set
            {
                if (_unternehmenId != value)
                {
                    _unternehmenId = value;
                    OnPropertyChanged(nameof(UnternehmenId));
                    LadeAbteilungen();
                }
            }
        }

        // AbteilungId für DB Speicherung
        private int? _abteilungId;

        public int? AbteilungId
        {
            get => _abteilungId;
            set
            {
                if (_abteilungId != value)
                {
                    _abteilungId = value;
                    OnPropertyChanged(nameof(AbteilungId));
                }
            }
        }

        // ObservableCollection für Unternehmen - wird z.B. im ViewModel oder beim Laden befüllt
        public ObservableCollection<Unternehmen> UnternehmenListe { get; set; } = new();

        // ObservableCollection für Abteilungen - abhängig vom ausgewählten Unternehmen
        public ObservableCollection<Abteilung> AbteilungenListe { get; set; } = new();

        // Property für aktuell ausgewähltes Unternehmen (ComboBox Binding)
        public Unternehmen AusgewaehltesUnternehmen
        {
            get => _ausgewaehltesUnternehmen;
            set
            {
                if (_ausgewaehltesUnternehmen != value)
                {
                    _ausgewaehltesUnternehmen = value;
                    OnPropertyChanged(nameof(AusgewaehltesUnternehmen));
                    UnternehmenId = _ausgewaehltesUnternehmen?.Id;

                    // Beim Unternehmenwechsel Abteilungen neu laden
                    LadeAbteilungen();
                }
            }
        }

        // Property für aktuell ausgewählte Abteilung (ComboBox Binding)
        public Abteilung AusgewaehlteAbteilung
        {
            get => _ausgewaehlteAbteilung;
            set
            {
                if (_ausgewaehlteAbteilung != value)
                {
                    _ausgewaehlteAbteilung = value;
                    OnPropertyChanged(nameof(AusgewaehlteAbteilung));
                    AbteilungId = _ausgewaehlteAbteilung?.Id;
                }
            }
        }

        private void LadeKontakte()
        {
            var kontakte = DatenbankService.LadeAlleKontakte(); // deine Methode
            KontaktListe.Clear();
            foreach (var kontakt in kontakte)
                KontaktListe.Add(kontakt);
        }

        private void BearbeiteKontakt()
        {
            if (AusgewählterKontakt == null)
            {
                MessageBox.Show("Bitte zuerst einen Kontakt auswählen.");
                return;
            }

            var viewModel = new KontaktWizardViewModel(AusgewählterKontakt);
            var bearbeitungsFenster = new KontaktBearbeitenWindow(viewModel);
            bearbeitungsFenster.ShowDialog();

            // Liste ggf. aktualisieren
            LadeKontakte();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void LadeAbteilungen()
        {
            AbteilungenListe.Clear();
            AbteilungId = null;
            AusgewaehlteAbteilung = null;

            if (UnternehmenId == null)
                return;

            // Hier Datenbankverbindung und Abfragen der Abteilungen für das ausgewählte Unternehmen
            var setting = ConfigurationManager.ConnectionStrings["CrmDatabase"];
            if (setting == null) return;

            string connString = setting.ConnectionString;

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT Id, Name FROM Abteilungen WHERE UnternehmenId = @unternehmenId", conn))
                {
                    cmd.Parameters.AddWithValue("@unternehmenId", UnternehmenId.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AbteilungenListe.Add(new Abteilung
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
        }
    }

    // Vereinfachte Klassen für Unternehmen und Abteilung
    public class Unternehmen
    {
        public int Id { get; set; }
        public string Firmenname { get; set; }
    }

    public class Abteilung
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}