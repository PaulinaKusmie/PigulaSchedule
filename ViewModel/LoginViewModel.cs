using Android.Icu.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.OCR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

    [ObservableProperty] private string ocrResult = string.Empty;


    public LoginViewModel()
    {
            Day = DateTime.Now.Day.ToString();
            MonthYear = $"{IntToNameMonth(DateTime.Now.Month)}   {DateTime.Now.Year}";
    }

    [RelayCommand] 
    public async Task ScanScheduleAsync()
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();

            if (photo == null)
                return;

            using var stream = await photo.OpenReadAsync();

            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            var result = await OcrPlugin.Default
                .RecognizeTextAsync(ms.ToArray());

            OcrResult = result.AllText;

           var dupa =  ScheduleParser.Parse(OcrResult);


        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd OCR: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }


    private string IntToNameMonth(int numberOfMonth)
    {
        return numberOfMonth >= 1 && numberOfMonth <= 12
            ? CultureInfo.GetCultureInfo("pl-PL")
                .DateTimeFormat
                .GetMonthName(numberOfMonth)
            : "Nieprawidłowy numer miesiąca";

    }
}
