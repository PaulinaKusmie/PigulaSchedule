using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PigulaSchedule.Calendar;

public partial class CalendarView : ContentView
{
    // ──────────────────────────────────────────────
    // BindableProperties
    // ──────────────────────────────────────────────

    public static readonly BindableProperty HighlightedDatesColor1Property =
        BindableProperty.Create(
            nameof(HighlightedDatesColor1),
            typeof(ObservableCollection<DateTime>),
            typeof(CalendarView),
            defaultValue: null,
            propertyChanged: OnDatesChanged);

    public static readonly BindableProperty HighlightedDatesColor2Property =
        BindableProperty.Create(
            nameof(HighlightedDatesColor2),
            typeof(ObservableCollection<DateTime>),
            typeof(CalendarView),
            defaultValue: null,
            propertyChanged: OnDatesChanged);

    public static readonly BindableProperty Color1Property =
        BindableProperty.Create(
            nameof(Color1),
            typeof(Color),
            typeof(CalendarView),
            defaultValue: Color.FromArgb("#378ADD"),
            propertyChanged: (b, o, n) => ((CalendarView)b).BuildCalendar());

    public static readonly BindableProperty Color2Property =
        BindableProperty.Create(
            nameof(Color2),
            typeof(Color),
            typeof(CalendarView),
            defaultValue: Color.FromArgb("#D4537E"),
            propertyChanged: (b, o, n) => ((CalendarView)b).BuildCalendar());

    public ObservableCollection<DateTime> HighlightedDatesColor1
    {
        get => (ObservableCollection<DateTime>)GetValue(HighlightedDatesColor1Property);
        set => SetValue(HighlightedDatesColor1Property, value);
    }

    public ObservableCollection<DateTime> HighlightedDatesColor2
    {
        get => (ObservableCollection<DateTime>)GetValue(HighlightedDatesColor2Property);
        set => SetValue(HighlightedDatesColor2Property, value);
    }

    public Color Color1
    {
        get => (Color)GetValue(Color1Property);
        set => SetValue(Color1Property, value);
    }

    public Color Color2
    {
        get => (Color)GetValue(Color2Property);
        set => SetValue(Color2Property, value);
    }

    // ──────────────────────────────────────────────
    // Stan wewnętrzny
    // ──────────────────────────────────────────────

    private int _currentYear;
    private int _currentMonth;

    private static readonly string[] DayNames = { "Pon", "Wt", "Śr", "Czw", "Pt", "Sob", "Nd" };
    private static readonly string[] MonthNames =
    {
        "Styczeń", "Luty", "Marzec", "Kwiecień", "Maj", "Czerwiec",
        "Lipiec", "Sierpień", "Wrzesień", "Październik", "Listopad", "Grudzień"
    };

    // ──────────────────────────────────────────────
    // Konstruktor
    // ──────────────────────────────────────────────

    public CalendarView()
    {
        InitializeComponent();

        var now = DateTime.Today;
        _currentYear = now.Year;
        _currentMonth = now.Month;

        BuildDayNameHeaders();
        BuildCalendar();
    }

    // ──────────────────────────────────────────────
    // Obsługa zmian kolekcji
    // ──────────────────────────────────────────────

    private static void OnDatesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var calendar = (CalendarView)bindable;

        if (oldValue is ObservableCollection<DateTime> oldCollection)
            oldCollection.CollectionChanged -= calendar.OnCollectionChanged;

        if (newValue is ObservableCollection<DateTime> newCollection)
            newCollection.CollectionChanged += calendar.OnCollectionChanged;

        calendar.BuildCalendar();
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => BuildCalendar();

    // ──────────────────────────────────────────────
    // Nawigacja
    // ──────────────────────────────────────────────

    private void OnPrevMonth(object sender, EventArgs e)
    {
        if (_currentMonth == 1) { _currentMonth = 12; _currentYear--; }
        else _currentMonth--;
        BuildCalendar();
    }

    private void OnNextMonth(object sender, EventArgs e)
    {
        if (_currentMonth == 12) { _currentMonth = 1; _currentYear++; }
        else _currentMonth++;
        BuildCalendar();
    }

    // ──────────────────────────────────────────────
    // Budowanie widoku
    // ──────────────────────────────────────────────

    private void BuildDayNameHeaders()
    {
        DayNamesGrid.Children.Clear();
        for (int i = 0; i < 7; i++)
        {
            var label = new Label
            {
                Text = DayNames[i],
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 12,
                TextColor = Colors.Gray
            };
            Grid.SetColumn(label, i);
            DayNamesGrid.Children.Add(label);
        }
    }

    private void BuildCalendar()
    {
        MonthYearLabel.Text = $"{MonthNames[_currentMonth - 1]} {_currentYear}";

        DaysGrid.Children.Clear();
        DaysGrid.RowDefinitions.Clear();

        var dates1 = HighlightedDatesColor1?.Select(d => d.Date).ToHashSet() ?? new HashSet<DateTime>();
        var dates2 = HighlightedDatesColor2?.Select(d => d.Date).ToHashSet() ?? new HashSet<DateTime>();

        var firstDay = new DateTime(_currentYear, _currentMonth, 1);
        int daysInMonth = DateTime.DaysInMonth(_currentYear, _currentMonth);

        // Poniedziałek = 0, ..., Niedziela = 6
        int startOffset = ((int)firstDay.DayOfWeek + 6) % 7;

        int totalCells = startOffset + daysInMonth;
        int rows = (int)Math.Ceiling(totalCells / 7.0);

        for (int r = 0; r < rows; r++)
            DaysGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var today = DateTime.Today;

        for (int cell = 0; cell < rows * 7; cell++)
        {
            int dayNumber = cell - startOffset + 1;
            int row = cell / 7;
            int col = cell % 7;

            if (dayNumber < 1 || dayNumber > daysInMonth)
            {
                var empty = new BoxView { IsVisible = false };
                Grid.SetRow(empty, row);
                Grid.SetColumn(empty, col);
                DaysGrid.Children.Add(empty);
                continue;
            }

            var date = new DateTime(_currentYear, _currentMonth, dayNumber);
            bool isColor1 = dates1.Contains(date.Date);
            bool isColor2 = dates2.Contains(date.Date);
            bool isToday = date.Date == today;

            Color bgColor = Colors.Transparent;
            Color textColor = Application.Current?.RequestedTheme == AppTheme.Dark
                ? Colors.White : Colors.Black;

            if (isColor1) { bgColor = Color1; textColor = Colors.White; }
            else if (isColor2) { bgColor = Color2; textColor = Colors.White; }

            var frame = new Border
            {
                HeightRequest = 36,
                WidthRequest = 36,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = bgColor,
                StrokeShape = new RoundRectangle { CornerRadius = 8 },
                Stroke = isToday
                    ? (Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black)
                    : Colors.Transparent,
                StrokeThickness = isToday ? 1.5 : 0,
                Content = new Label
                {
                    Text = dayNumber.ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 14,
                    TextColor = textColor
                }
            };

            Grid.SetRow(frame, row);
            Grid.SetColumn(frame, col);
            DaysGrid.Children.Add(frame);
        }
    }
}
