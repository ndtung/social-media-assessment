using System ;
using System.Collections.Generic ;
using System.Configuration;
using System.Data ;
using System.Linq ;
using System.Threading.Tasks ;
using System.Windows;
using MCIFramework.ViewModels;
namespace MCIFramework
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow app = new MainWindow();
            MainWindowModel context = new MainWindowModel();
            app.DataContext = context;
            app.Show();
        }
    }
}
