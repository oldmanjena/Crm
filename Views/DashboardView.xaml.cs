using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;

namespace Crm.Views
{
    public partial class DashboardView : UserControl
    {
        public RelayCommand MyCommand { get; }

        public DashboardView()
        {
            InitializeComponent();
            MyCommand = new RelayCommand(() => DoSomething());
        }

        private void DoSomething()
        {
            // Deine Logik hier
        }
    }
}