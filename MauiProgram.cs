using Microsoft.Extensions.Logging;
using PigulaSchedule.Calendar;
using PigulaSchedule.Interface;
using PigulaSchedule.Repository;
using PigulaSchedule.Services;
using PigulaSchedule.View;
using PigulaSchedule.ViewModel;
using PigulaSchedule.ViewModels;


namespace PigulaSchedule
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            builder.Services.AddHttpClient("GeminiClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(60); 
            });


            
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<SchedulePage>();
            builder.Services.AddTransient<ScheduleViewModel>();
            builder.Services.AddSingleton<IShiftRepository, ShiftRepository>();
            builder.Services.AddTransient<IGeminiOcrService, GeminiOcrService>();
            builder.Services.AddTransient<AddSchedule>();
     

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine(e.ExceptionObject);
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine(e.Exception);
            };
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
