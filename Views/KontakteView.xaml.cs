using Crm.Models;
using Crm.ViewModels;
using Crm.Klassen;
using Crm.Views;
using System.Windows.Controls;
using System.Windows.Input;

namespace Crm.Views
{
    /// <summary>
    /// Interaktionslogik für KontakteView.xaml
    /// </summary>
    public partial class KontakteView : UserControl
    {
        public KontakteView()
        {
            InitializeComponent();
            this.DataContext = new KontakteViewModel();
        }

        private void KontakteListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (KontakteListView.SelectedItem is KontaktModel kontaktAusListe)
            {
                // Lade vollständige Daten aus der DB
                var vollerKontakt = DatenbankService.LadeKontaktNachId(kontaktAusListe.kontakt_id);

                if (vollerKontakt != null)
                {
                    var viewModel = new KontaktWizardViewModel(vollerKontakt);
                    var window = new KontaktBearbeitenWindow(viewModel);  // <- Übergib das ViewModel

                    window.ShowDialog(); // Zeige Fenster
                }
            }
        }
    }
}