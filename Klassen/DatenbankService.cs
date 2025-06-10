using Crm.Models;
using Crm.ViewModels;
using Crm.Views;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;

namespace Crm.Klassen
{
    public static class DatenbankService
    {
        public static List<UnternehmenModel> LadeAlleUnternehmenMitAbteilungen()
        {
            var unternehmenListe = new List<UnternehmenModel>();

            var setting = ConfigurationManager.ConnectionStrings["CrmDatabase"];
            if (setting == null)
                throw new System.Exception("ConnectionString 'CrmDatabase' wurde nicht gefunden.");

            string connString = setting.ConnectionString;

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();

                string sqlUnternehmen = "SELECT unternehmen_id, firmenname, webseite, telefon, plz FROM unternehmen order by firmenname ASC";
                using (var cmdU = new SqlCommand(sqlUnternehmen, conn))
                using (var readerU = cmdU.ExecuteReader())
                {
                    while (readerU.Read())
                    {
                        var unternehmen = new UnternehmenModel
                        {
                            unternehmen_id = readerU.GetInt32(0),
                            Firmenname = readerU.GetString(1),
                            Webseite = readerU.IsDBNull(2) ? null : readerU.GetString(2),
                            Telefon = readerU.IsDBNull(3) ? null : readerU.GetString(3),
                            PLZ = readerU.IsDBNull(4) ? null : readerU.GetString(4),
                            Abteilungen = new ObservableCollection<AbteilungModel>()
                        };
                        unternehmenListe.Add(unternehmen);
                    }
                }

                string sqlAbteilungen = "SELECT abteilung_id, unternehmen_id, name, strasse, plz, ort FROM abteilungen order by name ASC";
                using (var cmdA = new SqlCommand(sqlAbteilungen, conn))
                using (var readerA = cmdA.ExecuteReader())
                {
                    while (readerA.Read())
                    {
                        int abtId = readerA.GetInt32(0);
                        int unternehmenId = readerA.GetInt32(1);
                        string name = readerA.GetString(2);
                        string strasse = readerA.IsDBNull(3) ? null : readerA.GetString(3);
                        string plz = readerA.IsDBNull(4) ? null : readerA.GetString(4);
                        string ort = readerA.IsDBNull(5) ? null : readerA.GetString(5);

                        var abteilung = new AbteilungModel
                        {
                            AbteilungId = abtId,
                            UnternehmenId = unternehmenId,
                            Name = name,
                            Strasse = strasse ?? "",
                            PLZ = plz,
                            Ort = ort
                        };

                        var unternehmen = unternehmenListe.Find(u => u.unternehmen_id == unternehmenId);
                        if (unternehmen != null)
                        {
                            unternehmen.Abteilungen.Add(abteilung);
                        }
                    }
                }
            }

            return unternehmenListe;
        }

        public static void AktualisiereUnternehmen(UnternehmenModel unternehmen)
        {
            var setting = ConfigurationManager.ConnectionStrings["CrmDatabase"];
            string connString = setting.ConnectionString;

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Unternehmen SET Firmenname = @Name, telefon = @Tel, Webseite = @Web WHERE unternehmen_id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", unternehmen.Firmenname ?? "");
                    cmd.Parameters.AddWithValue("@Tel", unternehmen.Telefon ?? "");
                    cmd.Parameters.AddWithValue("@Web", unternehmen.Webseite ?? "");
                    cmd.Parameters.AddWithValue("@Id", unternehmen.unternehmen_id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static KontaktModel LadeKontaktNachId(int kontaktId)
        {
            var setting = ConfigurationManager.ConnectionStrings["CrmDatabase"];
            string connString = setting.ConnectionString;

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"
                    SELECT k.kontakt_id, k.vorname, k.nachname, k.geburtstag, k.geschlecht,
                           k.telefon_geschaeftlich, k.telefon_privat, k.mobiltelefon, k.email,
                           k.strasse, k.plz, k.ort, k.land,
                           k.unternehmen_id, k.AbteilungId
                    FROM kontakte k
                    WHERE k.kontakt_id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", kontaktId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new KontaktModel
                            {
                                KontaktId = reader.GetInt32(0),
                                Vorname = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Nachname = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Geburtstag = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                Geschlecht = reader.IsDBNull(4) ? null : reader.GetString(4),
                                TelefonGeschaeftlich = reader.IsDBNull(5) ? null : reader.GetString(5),
                                TelefonPrivat = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Mobiltelefon = reader.IsDBNull(7) ? null : reader.GetString(7),
                                Email = reader.IsDBNull(8) ? null : reader.GetString(8),
                                Strasse = reader.IsDBNull(9) ? null : reader.GetString(9),
                                PLZ = reader.IsDBNull(10) ? null : reader.GetString(10),
                                Ort = reader.IsDBNull(11) ? null : reader.GetString(11),
                                Land = reader.IsDBNull(12) ? null : reader.GetString(12),
                                UnternehmenId = reader.IsDBNull(13) ? (int?)null : reader.GetInt32(13),
                                AbteilungId = reader.IsDBNull(14) ? (int?)null : reader.GetInt32(14)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public static void UpdateKontakt(KontaktModel kontakt)
        {
            var setting = ConfigurationManager.ConnectionStrings["CrmDatabase"];
            string connString = setting.ConnectionString;

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                string sql = @"
                    UPDATE kontakte SET
                        vorname = @Vorname,
                        nachname = @Nachname,
                        geburtstag = @Geburtstag,
                        geschlecht = @Geschlecht,
                        telefon_geschaeftlich = @TelG,
                        telefon_privat = @TelP,
                        mobiltelefon = @Mobil,
                        email = @Email,
                        strasse = @Strasse,
                        plz = @PLZ,
                        ort = @Ort,
                        land = @Land,
                        unternehmen_id = @UnternehmenId,
                        abteilung_id = @AbteilungId
                    WHERE kontakt_id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Vorname", kontakt.Vorname ?? "");
                    cmd.Parameters.AddWithValue("@Nachname", kontakt.Nachname ?? "");
                    cmd.Parameters.AddWithValue("@Geburtstag", (object?)kontakt.Geburtstag ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Geschlecht", kontakt.Geschlecht ?? "");
                    cmd.Parameters.AddWithValue("@TelG", kontakt.TelefonGeschaeftlich ?? "");
                    cmd.Parameters.AddWithValue("@TelP", kontakt.TelefonPrivat ?? "");
                    cmd.Parameters.AddWithValue("@Mobil", kontakt.Mobiltelefon ?? "");
                    cmd.Parameters.AddWithValue("@Email", kontakt.Email ?? "");
                    cmd.Parameters.AddWithValue("@Strasse", kontakt.Strasse ?? "");
                    cmd.Parameters.AddWithValue("@PLZ", kontakt.PLZ ?? "");
                    cmd.Parameters.AddWithValue("@Ort", kontakt.Ort ?? "");
                    cmd.Parameters.AddWithValue("@Land", kontakt.Land ?? "");
                    cmd.Parameters.AddWithValue("@UnternehmenId", (object?)kontakt.UnternehmenId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AbteilungId", (object?)kontakt.AbteilungId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", kontakt.KontaktId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<KontaktModel> LadeAlleKontakte()
        {
            var kontakte = new List<KontaktModel>();

            var setting = ConfigurationManager.ConnectionStrings["CrmDatabase"];
            string connString = setting.ConnectionString;

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"
            SELECT kontakt_id, vorname, nachname, email
            FROM kontakte";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kontakte.Add(new KontaktModel
                        {
                            KontaktId = reader.GetInt32(0),
                            Vorname = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Nachname = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Email = reader.IsDBNull(3) ? null : reader.GetString(3)
                        });
                    }
                }
            }

            return kontakte;
        }
    }
}