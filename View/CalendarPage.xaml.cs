using PigulaSchedule.ViewModel;

namespace PigulaSchedule.View;

public partial class CalendarPage : ContentPage
{
	public CalendarPage()
	{
		InitializeComponent();
		BindingContext = new CalendarViewModel();


    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CalendarViewModel vm)
            await vm.InitializeAsync();
    }
}