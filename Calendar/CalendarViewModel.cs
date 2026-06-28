using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using SQLite;

namespace PigulaSchedule.Calendar;

public class CalendarViewModel : INotifyPropertyChanged
{

    private ObservableCollection<DateTime> _datesColor1 = new();
    private ObservableCollection<DateTime> _datesColor2 = new();
    private ObservableCollection<DateTime> _datesColor3 = new();
    private ObservableCollection<DateTime> _datesColor4 = new();
    public ObservableCollection<DateTime> DatesColor1
    {
        get => _datesColor1;
        set { _datesColor1 = value; OnPropertyChanged(); }
    }

    public ObservableCollection<DateTime> DatesColor2
    {
        get => _datesColor2;
        set { _datesColor2 = value; OnPropertyChanged(); }
    }

    public ObservableCollection<DateTime> DatesColor3
    {
        get => _datesColor3;
        set { _datesColor3 = value; OnPropertyChanged(); }
    }

    public ObservableCollection<DateTime> DatesColor4
    {
        get => _datesColor4;
        set { _datesColor4 = value; OnPropertyChanged(); }
    }


    private readonly SQLiteAsyncConnection _db;
    private const string DbName = "calendar.db3";

    public CalendarViewModel()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbName);
        _db = new SQLiteAsyncConnection(dbPath);
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        await _db.CreateTableAsync<MarkedDayEntity>();
        await LoadFromDbAsync();
    }


    private async Task LoadFromDbAsync()
    {
        var all = await _db.Table<MarkedDayEntity>().ToListAsync();

        var c1 = all
            .Where(e => e.ColorIndex == 1)
            .Select(e => e.Date)
            .OrderBy(d => d);

        var c2 = all
            .Where(e => e.ColorIndex == 2)
            .Select(e => e.Date)
            .OrderBy(d => d);

        DatesColor1 = new ObservableCollection<DateTime>(c1);
        DatesColor2 = new ObservableCollection<DateTime>(c2);

        DatesColor3 = new ObservableCollection<DateTime>(all
            .Where(e => e.ColorIndex == 3)
            .Select(e => e.Date)
            .OrderBy(d => d));


        DatesColor4 = new ObservableCollection<DateTime>(all
           .Where(e => e.ColorIndex == 4)
           .Select(e => e.Date)
           .OrderBy(d => d));
    }

    public async Task MarkDateAsync(DateTime date, int colorIndex)
    {
        var normalized = date.Date;

   
        await UnmarkDateAsync(normalized);

        var entity = new MarkedDayEntity { Date = normalized, ColorIndex = colorIndex };
        await _db.InsertAsync(entity);

        if (colorIndex == 1)
            DatesColor1.Add(normalized);
        else
            DatesColor2.Add(normalized);
    }


    public async Task UnmarkDateAsync(DateTime date)
    {
        var normalized = date.Date;

        await _db.ExecuteAsync(
            "DELETE FROM MarkedDayEntity WHERE Date = ?",
            normalized.Ticks);

        var inC1 = DatesColor1.FirstOrDefault(d => d.Date == normalized);
        if (inC1 != default) DatesColor1.Remove(inC1);

        var inC2 = DatesColor2.FirstOrDefault(d => d.Date == normalized);
        if (inC2 != default) DatesColor2.Remove(inC2);

        var inC3 = DatesColor3.FirstOrDefault(d => d.Date == normalized);
        if (inC3 != default) DatesColor3.Remove(inC3);
    
        var inC4 = DatesColor4.FirstOrDefault(d => d.Date == normalized);   
        if(inC4 != default) DatesColor4.Remove(inC4);
    }


    public async Task ClearAllAsync()
    {
        await _db.DeleteAllAsync<MarkedDayEntity>();
        DatesColor1.Clear();
        DatesColor2.Clear();
        DatesColor3.Clear();
        DatesColor4.Clear();
    }



    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}


[Table("MarkedDayEntity")]
public class MarkedDayEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public long DateTicks
    {
        get => Date.Ticks;
        set => Date = new DateTime(value);
    }

    [Ignore]
    public DateTime Date { get; set; }

    public int ColorIndex { get; set; }

}
