using Crm.Klassen;
using Crm.Models;
using System.Windows;

namespace Crm.Views
{
    public partial class UnternehmenBearbeiten : Fluent.RibbonWindow
    {
        private UnternehmenModel _unternehmen;

        public UnternehmenBearbeiten(UnternehmenModel unternehmen)
        {
            InitializeComponent();
            _unternehmen = unternehmen;
            this.DataContext = _unternehmen;
        }

        private void Speichern_Click(object sender, RoutedEventArgs e)
        {
            DatenbankService.AktualisiereUnternehmen(_unternehmen);
            this.DialogResult = true;
            this.Close();
        }

        private void Abbrechen_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}