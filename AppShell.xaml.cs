using PigulaSchedule.View;

namespace PigulaSchedule
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
         
            Routing.RegisterRoute(nameof(CalendarPage), typeof(CalendarPage));
        }
    }
}
