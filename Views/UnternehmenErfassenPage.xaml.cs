using Crm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace Crm.Views
{
    /// <summary>
    /// Interaktionslogik für UnternehmenErfassenPage.xaml
    /// </summary>

    public partial class UnternehmenErfassenControl : UserControl
    {
        public UnternehmenErfassenControl()
        {
            InitializeComponent(); // <- diese Methode muss hier aufgerufen werden
            DataContext = new UnternehmenWizardViewModel();
        }

        private void Speichern_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is UnternehmenWizardViewModel vm)
            {
                vm.SaveCommand.Execute(null);
            }
        }
    }
}