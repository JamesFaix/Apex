using System.Windows;
using System.Threading;
using System.Globalization;

namespace Djambi.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
        }
    }
}
