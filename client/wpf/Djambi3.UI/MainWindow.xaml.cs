using System;
using System.Windows;
using System.Windows.Navigation;
using System.Threading;

namespace Djambi.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigateToMainMenu();

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-ES");
            var images = new ImageRepository();
            this.Icon = images.AppIcon;
        }

        private void NavigateToPage(string pageName)
        {
            var nav = frameMain.NavigationService;
            nav.Navigate(new Uri($"/Djambi.UI;component/Pages/{pageName}.xaml", UriKind.Relative));
        }

        public void NavigateToMainMenu()
        {
            NavigateToPage("MainMenuPage");
        }

        public void NavigateToGame()
        {
            NavigateToPage("GamePage");
        }

        public void AutoResizeWindow()
        {
            var x = 1;
        }
    }
}
