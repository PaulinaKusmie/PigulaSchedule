
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PigulaSchedule.Resources;
using PigulaSchedule.View;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace PigulaSchedule.ViewModels;

public partial class MainViewModel : ObservableObject
{

    #region props
    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isBusy;
    [ObservableProperty]
    private string helloText;

    [ObservableProperty]
    private string nextShiftText;

 


    private AddSchedule addSchedule;
    public AddSchedule AddSchedule
    {
        get => addSchedule;
        set
        {
            addSchedule = value;
            OnPropertyChanged();
        }
    }


    [ObservableProperty]
    private string errorMessage = string.Empty;
    #endregion

    public MainViewModel(AddSchedule addSchedule) 
    {
        AddSchedule = addSchedule;
    }

    public async Task InitializeAsync()
    {
        await LoadDataAsync();
    }



    public async Task LoadDataAsync()
    {
        string nextShift = await AddSchedule.LookForNextShift();

        HelloText = $"Witaj! Dziś jest {DateTime.Now.Day} {Utilitis.IntToNameMonth(DateTime.Now.Month)} {DateTime.Now.Year}\n";
        if(!string.IsNullOrEmpty(nextShift)) NextShiftText = $"Następna zmiana {nextShift}";


    }

    [RelayCommand]
    public async Task ScanScheduleAsync()
    {
        try
        {
            IsBusy = true;
            await AddSchedule.AddScheduleAsync();

        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd dodawania harmonogramu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }




    [RelayCommand]
    public async Task OpenCalendarPageAsync()
    {
        try
        {

            IsBusy = true;
            await Shell.Current.GoToAsync(nameof(CalendarPage));


        }
        catch (Exception ex)
        {
            ErrorMessage = $"Bład otwierania strony kalendarza: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteAsync()
    {
        try
        {
            await AddSchedule.DeleteData();

        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd dodawania harmonogramu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

}
