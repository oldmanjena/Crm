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
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Crm.Views
{
    /// <summary>
    /// Interaktionslogik für UnternehmenWizard.xaml
    /// </summary>
    public partial class UnternehmenWizard : Window
    {
        public UnternehmenWizard()
        {
            InitializeComponent();
            this.DataContext = new UnternehmenWizardViewModel();
        }

        private void Weiter_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = wizard.Items.IndexOf(wizard.CurrentPage);
            if (currentIndex < wizard.Items.Count - 1)
            {
                var nextPage = wizard.Items[currentIndex + 1] as WizardPage;
                if (nextPage != null)
                {
                    wizard.CurrentPage = nextPage;
                }
            }
        }

        private void Zurueck_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = wizard.Items.IndexOf(wizard.CurrentPage);
            if (currentIndex > 0)
            {
                var prevPage = wizard.Items[currentIndex - 1] as WizardPage;
                if (prevPage != null)
                {
                    wizard.CurrentPage = prevPage;
                }
            }
        }

        private void Abbrechen_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Speichern_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button wurde geklickt"); // DEBUG TEST
            if (DataContext is UnternehmenWizardViewModel vm)
            {
                if (vm.SaveCommand.CanExecute(null))
                {
                    vm.SaveCommand.Execute(null);
                    MessageBox.Show("Unternehmen erfolgreich gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
        }
    }
}