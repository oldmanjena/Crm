using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.Models
{
    public class UnternehmenModel
    {
        public int unternehmen_id { get; set; }  // Wichtig für die Zuordnung im Kontakt
        public string Firmenname { get; set; }
        public string Strasse { get; set; }
        public string Ort { get; set; }
        public string PLZ { get; set; }
        public string Land { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string Webseite { get; set; }
        public string Notizen { get; set; }

        public override string ToString() => Firmenname;

        public ObservableCollection<AbteilungModel> Abteilungen { get; set; } = new();
    }
}