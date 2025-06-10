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

namespace Crm.Views
{
    /// <summary>
    /// Interaktionslogik für UnternehmenView.xaml
    /// </summary>
    public partial class UnternehmenView : UserControl
    {
        public UnternehmenView()
        {
            InitializeComponent();
            this.DataContext = new UnternehmenViewModel();
        }

        private void ÖffneAnUnternehmen_Click(object sender, RoutedEventArgs e)
        {
            var fenster = new UnternehmenErfassenFenster();
            fenster.ShowDialog();
        }
    }
}