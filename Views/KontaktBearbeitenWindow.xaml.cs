using System.Windows;
using Crm.Views;
using Crm.ViewModels;

namespace Crm.Views
{
    /// <summary>
    /// Interaktionslogik für KontaktBearbeitenWindow.xaml
    /// </summary>
    public partial class KontaktBearbeitenWindow : Window
    {
        public KontaktBearbeitenWindow(KontaktWizardViewModel viewModel)
        {
            InitializeComponent();

            var page = new KontaktErfassenPage();     // UserControl
            page.DataContext = viewModel;             // <-- viewModel hier korrekt verwendet

            MainFrame.Content = page;
        }
    }
}