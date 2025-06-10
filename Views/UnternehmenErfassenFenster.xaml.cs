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
    /// Interaktionslogik für UnternehmenErfassenFenster.xaml
    /// </summary>
    public partial class UnternehmenErfassenFenster : Window
    {
        public UnternehmenErfassenFenster()
        {
            InitializeComponent();
            this.Content = new UnternehmenErfassenControl();
        }
    }
}