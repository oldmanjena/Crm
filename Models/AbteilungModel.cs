using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.Models
{
    public class AbteilungModel
    {
        public int? AbteilungId { get; set; }
        public int UnternehmenId { get; set; } // Wird beim Speichern gesetzt
        public string Name { get; set; }
        public string Strasse { get; set; }
        public string PLZ { get; set; }
        public string Ort { get; set; }
        public string Land { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
    }
}