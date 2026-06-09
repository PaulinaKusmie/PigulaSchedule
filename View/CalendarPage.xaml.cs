using PigulaSchedule.ViewModel;

namespace PigulaSchedule.View;

public partial class CalendarPage : ContentPage
{
	public CalendarPage()
	{
		InitializeComponent();
		BindingContext = new CalendarViewModel();


    }
}