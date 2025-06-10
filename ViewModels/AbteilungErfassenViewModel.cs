using Crm.Models;
using Crm.Klassen;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Crm.ViewModels
{
    public class AbteilungErfassenViewModel : INotifyPropertyChanged
    {
        // Event für PropertyChanged (INotifyPropertyChanged)
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnChanged([CallerMemberName] string? prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // Properties für die Abteilungseingabe
        private string _name = string.Empty;

        private string _strasse = string.Empty;
        private string _plz = string.Empty;
        private string _ort = string.Empty;
        private string _land = string.Empty;
        private string _telefon = string.Empty;
        private string _email = string.Empty;

        public string Name
        {
            get => _name;
            set { _name = value; OnChanged(); }
        }

        public string Strasse
        {
            get => _strasse;
            set { _strasse = value; OnChanged(); }
        }

        public string PLZ
        {
            get => _plz;
            set { _plz = value; OnChanged(); }
        }

        public string Ort
        {
            get => _ort;
            set { _ort = value; OnChanged(); }
        }

        public string Land
        {
            get => _land;
            set { _land = value; OnChanged(); }
        }

        public string Telefon
        {
            get => _telefon;
            set { _telefon = value; OnChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnChanged(); }
        }

        // Liste aller Unternehmen für die Auswahl
        public ObservableCollection<UnternehmenModel> UnternehmenListe { get; set; } = new();

        // Ausgewähltes Unternehmen
        private UnternehmenModel? _ausgewaehltesUnternehmen;

        public UnternehmenModel? AusgewaehltesUnternehmen
        {
            get => _ausgewaehltesUnternehmen;
            set { _ausgewaehltesUnternehmen = value; OnChanged(); }
        }

        // Command zum Speichern
        public ICommand SpeichernCommand { get; }

        // Standardkonstruktor
        public AbteilungErfassenViewModel() : this(null)
        {
        }

        // Konstruktor mit Vorauswahl eines Unternehmens
        public AbteilungErfassenViewModel(UnternehmenModel? unternehmen)
        {
            SpeichernCommand = new RelayCommand(_ => SpeichereAbteilung());

            LadeUnternehmen();

            if (unternehmen != null)
            {
                AusgewaehltesUnternehmen = UnternehmenListe
                    .FirstOrDefault(u => u.unternehmen_id == unternehmen.unternehmen_id);
            }
        }

        // Unternehmen aus Datenbank laden
        private void LadeUnternehmen()
        {
            try
            {
                UnternehmenListe.Clear();
                var setting = ConfigurationManager.ConnectionStrings["CrmDatabase"];
                if (setting == null)
                {
                    MessageBox.Show("ConnectionString 'CrmDatabase' wurde nicht gefunden.");
                    return;
                }

                using var conn = new SqlConnection(setting.ConnectionString);
                conn.Open();

                string sql = "SELECT unternehmen_id, firmenname FROM unternehmen ORDER BY firmenname";

                using var cmd = new SqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    UnternehmenListe.Add(new UnternehmenModel
                    {
                        unternehmen_id = reader.GetInt32(0),
                        Firmenname = reader.GetString(1)
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Unternehmen: " + ex.Message);
            }
        }

        // Abteilung speichern
        private void SpeichereAbteilung()
        {
            if (AusgewaehltesUnternehmen == null)
            {
                MessageBox.Show("Bitte wählen Sie ein Unternehmen aus.");
                return;
            }

            try
            {
                var setting = ConfigurationManager.ConnectionStrings["CrmDatabase"];
                if (setting == null)
                {
                    MessageBox.Show("ConnectionString 'CrmDatabase' wurde nicht gefunden.");
                    return;
                }

                using var conn = new SqlConnection(setting.ConnectionString);
                conn.Open();

                string sql = @"
                    INSERT INTO abteilungen
                    (unternehmen_id, name, strasse, plz, ort, land, telefon, email)
                    VALUES
                    (@unternehmen_id, @name, @strasse, @plz, @ort, @land, @telefon, @email);";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@unternehmen_id", AusgewaehltesUnternehmen.unternehmen_id);
                cmd.Parameters.AddWithValue("@name", (object)Name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@strasse", (object)Strasse ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@plz", (object)PLZ ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ort", (object)Ort ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@land", (object)Land ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@telefon", (object)Telefon ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", (object)Email ?? DBNull.Value);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Abteilung wurde erfolgreich gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);

                // Optional: Eingabefelder nach Speichern zurücksetzen
                Name = Strasse = PLZ = Ort = Land = Telefon = Email = string.Empty;
                AusgewaehltesUnternehmen = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern: " + ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}