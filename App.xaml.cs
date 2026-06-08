using PigulaSchedule.View;

namespace PigulaSchedule
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
           // MainPage = new NavigationPage(new LoginPage());
        }


    }
}