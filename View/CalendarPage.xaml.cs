using PigulaSchedule.Interface;
using PigulaSchedule.ViewModel;

namespace PigulaSchedule.View;

public partial class CalendarPage : ContentPage
{
	public CalendarPage(CalendarViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;


    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CalendarViewModel vm)
            await vm.InitializeAsync();
    }
}