using Crm.ViewModels;
using Crm.Views;
using Fluent;
using System.Windows;
using System.Windows.Controls;

namespace Crm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem selectedItem)
            {
                string selected = selectedItem.Header.ToString();

                switch (selected)
                {
                    case "Firma Anlegen":
                        ContentRegion.Content = new UnternehmenErfassenControl();
                        break;

                    case "Firma Übersicht":
                        ContentRegion.Content = new UnternehmenView();
                        break;

                    case "Kontakte Anlegen":
                        ContentRegion.Content = new KontaktErfassenPage();
                        break;

                    case "Kontakte Übersicht":
                        ContentRegion.Content = new KontakteView();
                        break;

                    case "Abteilung Anlegen":
                        ContentRegion.Content = new AbteilungErfassen();
                        break;

                    default:
                        ContentRegion.Content = null;
                        break;
                }
            }
        }
    }
}