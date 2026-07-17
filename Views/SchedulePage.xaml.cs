using PigulaSchedule.Interface;
using PigulaSchedule.ViewModel;

namespace PigulaSchedule.View;

public partial class SchedulePage : ContentPage
{
	public SchedulePage(ScheduleViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;


    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ScheduleViewModel vm)
            await vm.InitializeAsync();
    }
}