using PigulaSchedule.ViewModel;

namespace PigulaSchedule.View;

public partial class CalendarPage : ContentPage
{
	public CalendarPage()
	{
		InitializeComponent();
		BindingContext = new CalendarViewModel();

        var screenHeight = DeviceDisplay.MainDisplayInfo.Height
                           / DeviceDisplay.MainDisplayInfo.Density;

        CalendarViewControl.DaysViewHeightRequest = screenHeight - 150;

    }
}