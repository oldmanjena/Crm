using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.Models
{
    public class KontaktModel
    {
        public int KontaktId { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public DateTime? Geburtstag { get; set; }
        public string Geschlecht { get; set; }
        public string TelefonGeschaeftlich { get; set; }
        public string TelefonPrivat { get; set; }
        public string Mobiltelefon { get; set; }
        public string Email { get; set; }
        public string Strasse { get; set; }
        public string PLZ { get; set; }
        public string Ort { get; set; }
        public string Land { get; set; }

        public string Position { get; set; }

        public int? UnternehmenId { get; set; }
        public int? AbteilungId { get; set; }

        public string VollerName => $"{Vorname} {Nachname}";
    }
}