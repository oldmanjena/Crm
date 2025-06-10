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

namespace Crm.ViewModels
{
    public class KontaktWizardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly KontaktModel? _bearbeiteterKontakt;

        public string SpeicherButtonText => _bearbeiteterKontakt == null ? "Kontakt speichern" : "Kontakt aktualisieren";

        private void OnChanged([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

            if (_canSaveRelevantProperties.Contains(prop))
            {
                _finishCommand?.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<UnternehmenModel> UnternehmenListe { get; set; } = new();
        public ObservableCollection<AbteilungModel> Abteilungen { get; set; } = new();

        private UnternehmenModel? _ausgewaehltesUnternehmen;

        public UnternehmenModel? AusgewaehltesUnternehmen
        {
            get => _ausgewaehltesUnternehmen;
            set
            {
                if (_ausgewaehltesUnternehmen != value)
                {
                    _ausgewaehltesUnternehmen = value;
                    OnChanged();
                    LadeAbteilungenFuerUnternehmen();
                }
            }
        }

        private AbteilungModel? _ausgewaehlteAbteilung;

        public AbteilungModel? AusgewaehlteAbteilung
        {
            get => _ausgewaehlteAbteilung;
            set { _ausgewaehlteAbteilung = value; OnChanged(); }
        }

        // Kontaktfelder
        private string _vorname = "";

        public string Vorname
        {
            get => _vorname;
            set { _vorname = value; OnChanged(); }
        }

        private string _nachname = "";

        public string Nachname
        {
            get => _nachname;
            set { _nachname = value; OnChanged(); }
        }

        private DateTime? _geburtstag;

        public DateTime? Geburtstag
        {
            get => _geburtstag;
            set { _geburtstag = value; OnChanged(); }
        }

        private string _geschlecht = "";

        public string Geschlecht
        {
            get => _geschlecht;
            set { _geschlecht = value; OnChanged(); }
        }

        private string _position = "";

        public string Position
        {
            get => _position;
            set { _position = value; OnChanged(); }
        }

        private string _telefonGeschaeftlich = "";

        public string TelefonGeschaeftlich
        {
            get => _telefonGeschaeftlich;
            set { _telefonGeschaeftlich = value; OnChanged(); }
        }

        private string _email = "";

        public string Email
        {
            get => _email;
            set { _email = value; OnChanged(); }
        }

        // NEUE Felder für Adresse
        private string _strasse = "";

        public string Strasse
        {
            get => _strasse;
            set { _strasse = value; OnChanged(); }
        }

        private string _plz = "";

        public string PLZ
        {
            get => _plz;
            set { _plz = value; OnChanged(); }
        }

        private string _ort = "";

        public string Ort
        {
            get => _ort;
            set { _ort = value; OnChanged(); }
        }

        private string _land = "";

        public string Land
        {
            get => _land;
            set { _land = value; OnChanged(); }
        }

        private string _telefon = "";

        public string Telefon
        {
            get => _telefon;
            set { _telefon = value; OnChanged(); }
        }

        public ObservableCollection<string> Geschlechter { get; } = new()
        {
            "männlich", "weiblich", "divers"
        };

        private RelayCommand _finishCommand;
        public RelayCommand FinishCommand => _finishCommand;

        private static readonly string[] _canSaveRelevantProperties =
        {
            nameof(Vorname),
            nameof(Nachname),
            nameof(AusgewaehltesUnternehmen)
        };

        public KontaktWizardViewModel()
        {
            UnternehmenListe = new ObservableCollection<UnternehmenModel>(
                DatenbankService.LadeAlleUnternehmenMitAbteilungen()
            );

            _finishCommand = new RelayCommand(_ => SaveKontaktToDatabase(), _ => CanSave());
        }

        public KontaktWizardViewModel(KontaktModel? kontaktZumBearbeiten) : this()
        {
            _bearbeiteterKontakt = kontaktZumBearbeiten;

            if (_bearbeiteterKontakt != null)
            {
                Vorname = kontaktZumBearbeiten.Vorname;
                Nachname = kontaktZumBearbeiten.Nachname;
                Geburtstag = kontaktZumBearbeiten.Geburtstag;
                Geschlecht = kontaktZumBearbeiten.Geschlecht;
                Position = kontaktZumBearbeiten.Position;
                Telefon = kontaktZumBearbeiten.TelefonGeschaeftlich; // falls Property noch so heißt
                Email = kontaktZumBearbeiten.Email;

                // Adresse laden
                Strasse = kontaktZumBearbeiten.Strasse;
                PLZ = kontaktZumBearbeiten.PLZ;
                Ort = kontaktZumBearbeiten.Ort;
                Land = kontaktZumBearbeiten.Land;

                AusgewaehltesUnternehmen = UnternehmenListe
                    .FirstOrDefault(u => u.unternehmen_id == kontaktZumBearbeiten.UnternehmenId);

                LadeAbteilungenFuerUnternehmen();

                AusgewaehlteAbteilung = Abteilungen
                    .FirstOrDefault(a => a.AbteilungId == kontaktZumBearbeiten.AbteilungId);
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Vorname)
                && !string.IsNullOrWhiteSpace(Nachname)
                && AusgewaehltesUnternehmen != null;
        }

        private void LadeAbteilungenFuerUnternehmen()
        {
            Abteilungen.Clear();
            AusgewaehlteAbteilung = null;

            if (AusgewaehltesUnternehmen?.Abteilungen != null)
            {
                foreach (var abt in AusgewaehltesUnternehmen.Abteilungen)
                    Abteilungen.Add(abt);
            }
        }

        public void SaveKontaktToDatabase()
        {
            var setting = ConfigurationManager.ConnectionStrings["CrmDatabase"];
            if (setting == null)
            {
                MessageBox.Show("ConnectionString 'CrmDatabase' wurde nicht gefunden.");
                return;
            }

            try
            {
                using var conn = new SqlConnection(setting.ConnectionString);
                conn.Open();

                // ✅ Nur bei Neuanlage: Dublettenprüfung durchführen
                if (_bearbeiteterKontakt == null && KontaktExistiertSchon(conn, Vorname, Nachname, Email))
                {
                    MessageBox.Show("Ein Kontakt mit diesen Angaben existiert bereits.", "Duplikat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string sql;
                if (_bearbeiteterKontakt != null)
                {
                    sql = @"
            UPDATE kontakte
            SET vorname = @Vorname,
                nachname = @Nachname,
                geburtstag = @Geburtstag,
                geschlecht = @Geschlecht,
                telefon_geschaeftlich = @Telefon,
                email = @Email,
                position = @Position,
                unternehmen_id = @UnternehmenId,
                AbteilungId = @AbteilungId,
                strasse = @Strasse,
                plz = @PLZ,
                ort = @Ort,
                land = @Land
            WHERE kontakt_id = @KontaktId;";
                }
                else
                {
                    sql = @"
            INSERT INTO kontakte
            (vorname, nachname, geburtstag, geschlecht, telefon, email, position, unternehmen_id, AbteilungId, strasse, plz, ort, land)
            VALUES
            (@Vorname, @Nachname, @Geburtstag, @Geschlecht, @Telefon, @Email, @Position, @UnternehmenId, @AbteilungId, @Strasse, @PLZ, @Ort, @Land);";
                }

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Vorname", (object)Vorname ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Nachname", (object)Nachname ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Geburtstag", (object)Geburtstag ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Geschlecht", (object)Geschlecht ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefon", (object)Telefon ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Position", (object)Position ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UnternehmenId", (object)(AusgewaehltesUnternehmen?.unternehmen_id) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AbteilungId", (object)(AusgewaehlteAbteilung?.AbteilungId) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Strasse", (object)Strasse ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PLZ", (object)PLZ ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ort", (object)Ort ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Land", (object)Land ?? DBNull.Value);

                if (_bearbeiteterKontakt != null)
                    cmd.Parameters.AddWithValue("@KontaktId", _bearbeiteterKontakt.KontaktId);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Kontakt wurde erfolgreich gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);

                Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.DataContext == this)
                    ?.Close();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) // UNIQUE constraint violation
            {
                MessageBox.Show("Ein Kontakt mit diesen Angaben existiert bereits.", "Duplikat", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ✅ Methode außerhalb!
        private bool KontaktExistiertSchon(SqlConnection conn, string vorname, string nachname, string email)
        {
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM kontakte WHERE vorname = @v AND nachname = @n AND email = @e", conn);
            cmd.Parameters.AddWithValue("@v", vorname);
            cmd.Parameters.AddWithValue("@n", nachname);
            cmd.Parameters.AddWithValue("@e", email);
            return (int)cmd.ExecuteScalar() > 0;
        }
    }
}