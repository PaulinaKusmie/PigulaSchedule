
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PigulaSchedule.View;
using Plugin.Maui.OCR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PigulaSchedule.Resources;


namespace PigulaSchedule.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    private string day;

    public string Day
    {
        get => day;
        set
        {
            day = value;
            OnPropertyChanged();
        }
    }

    private string monthYear;
    public string MonthYear
    {
        get => monthYear;
        set
        {
            monthYear = value;
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    private string errorMessage = string.Empty;




    public LoginViewModel()
    {
            Day = DateTime.Now.Day.ToString();
            MonthYear = $"{Utiliti.IntToNameMonth(DateTime.Now.Month)}   {DateTime.Now.Year}";
    }

    [RelayCommand] 
    public async Task ScanScheduleAsync()
    {
        try
        {
            AddSchedule addSchedule = new AddSchedule();
            await addSchedule.AddScheduleAsync();

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
            ErrorMessage = $"Error of OpenCalendarPage: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }



}
