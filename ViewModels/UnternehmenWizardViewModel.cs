using Crm.Klassen;
using Crm.Models;
using Crm.ViewModels;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Input;

namespace Crm.ViewModels
{
    public class UnternehmenWizardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _firmenname, _strasse, _ort, _plz, _land, _telefon, _email, _webseite, _notizen;

        public string Firmenname
        {
            get => _firmenname;
            set { _firmenname = value; OnPropertyChanged(); }
        }

        public string Strasse
        {
            get => _strasse;
            set { _strasse = value; OnPropertyChanged(); }
        }

        public string Ort
        {
            get => _ort;
            set { _ort = value; OnPropertyChanged(); }
        }

        public string PLZ
        {
            get => _plz;
            set { _plz = value; OnPropertyChanged(); }
        }

        public string Land
        {
            get => _land;
            set { _land = value; OnPropertyChanged(); }
        }

        public string Telefon
        {
            get => _telefon;
            set { _telefon = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Webseite
        {
            get => _webseite;
            set { _webseite = value; OnPropertyChanged(); }
        }

        public string Notizen
        {
            get => _notizen;
            set { _notizen = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }

        public UnternehmenWizardViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveUnternehmenMitAbteilungen());
        }

        private void SaveUnternehmenMitAbteilungen()
        {
            int neueId = SaveUnternehmen();
            if (neueId > 0)
            {
                SaveAbteilungen(neueId);
                MessageBox.Show("Das Unternehmen und die Abteilungen wurden erfolgreich gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Windows[Application.Current.Windows.Count - 1]?.Close();
            }
        }

        public ObservableCollection<AbteilungModel> Abteilungen { get; set; } = new();

        private AbteilungModel _neueAbteilung = new();

        public AbteilungModel NeueAbteilung
        {
            get => _neueAbteilung;
            set
            {
                _neueAbteilung = value;
                OnPropertyChanged();
            }
        }

        public ICommand AbteilungHinzufuegenCommand => new RelayCommand(_ => HinzufuegenAbteilung());

        private void HinzufuegenAbteilung()
        {
            Abteilungen.Add(new AbteilungModel
            {
                Name = NeueAbteilung.Name,
                Strasse = NeueAbteilung.Strasse,
                PLZ = NeueAbteilung.PLZ,
                Ort = NeueAbteilung.Ort,
                Land = NeueAbteilung.Land,
                Telefon = NeueAbteilung.Telefon,
                Email = NeueAbteilung.Email
            });

            NeueAbteilung = new AbteilungModel(); // Eingabefelder leeren
        }

        private int SaveUnternehmen()
        {
            try
            {
                var setting = ConfigurationManager.ConnectionStrings["CrmDatabase"];
                if (setting == null)
                {
                    MessageBox.Show("ConnectionString 'CrmDatabase' wurde nicht gefunden.");
                    return -1;
                }

                string connString = setting.ConnectionString;

                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string sql = @"
                INSERT INTO unternehmen
                (firmenname, strasse, ort, plz, land, telefon, email, webseite, notizen)
                VALUES
                (@firmenname, @strasse, @ort, @plz, @land, @telefon, @email, @webseite, @notizen);
                SELECT CAST(scope_identity() AS int);";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@firmenname", (object)Firmenname ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@strasse", (object)Strasse ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ort", (object)Ort ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@plz", (object)PLZ ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@land", (object)Land ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@telefon", (object)Telefon ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@email", (object)Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@webseite", (object)Webseite ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@notizen", (object)Notizen ?? DBNull.Value);

                        int neueUnternehmensId = (int)cmd.ExecuteScalar();
                        return neueUnternehmensId;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }

        private void SaveAbteilungen(int unternehmensId)
        {
            if (unternehmensId <= 0)
                return;

            var setting = ConfigurationManager.ConnectionStrings["CrmDatabase"];
            if (setting == null)
            {
                MessageBox.Show("ConnectionString 'CrmDatabase' wurde nicht gefunden.");
                return;
            }

            string connString = setting.ConnectionString;

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();

                foreach (var abteilung in Abteilungen)
                {
                    string sql = @"
                INSERT INTO abteilungen
                (unternehmen_id, name, strasse, plz, ort, land, telefon, email)
                VALUES
                (@unternehmen_id, @name, @strasse, @plz, @ort, @land, @telefon, @email);";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@unternehmen_id", unternehmensId);
                        cmd.Parameters.AddWithValue("@name", (object)abteilung.Name ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@strasse", (object)abteilung.Strasse ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@plz", (object)abteilung.PLZ ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ort", (object)abteilung.Ort ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@land", (object)abteilung.Land ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@telefon", (object)abteilung.Telefon ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@email", (object)abteilung.Email ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}