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

namespace Crm.Views
{
    /// <summary>
    /// Interaktionslogik für KontaktWizard.xaml
    /// </summary>
    public partial class KontaktWizard : Window
    {
        public KontaktWizard()
        {
            InitializeComponent();
            wizardMain.Finish += wizardMain_Finish;
        }

        private void wizardMain_Finish(object sender, Xceed.Wpf.Toolkit.Core.CancelRoutedEventArgs e)
        {
            var vm = (KontaktWizardViewModel)this.DataContext;
            try
            {
                vm.SaveKontaktToDatabase();
                MessageBox.Show("Kontakt erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
                e.Handled = true; // verhindert Schließen
            }
        }
    }
}