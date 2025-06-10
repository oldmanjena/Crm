using ModernWpf;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Crm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Theme (Light/Dark) setzen
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;

            // Akzentfarbe setzen (optional)
            ThemeManager.Current.AccentColor = System.Windows.Media.Colors.Teal;

            // Weitere Initialisierungen...
        }
    }
}