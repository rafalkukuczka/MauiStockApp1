using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    private readonly StockDatabase _database;
    private readonly StockChartDrawable _chartDrawable = new();

    public MainPage(StockDatabase database)
    {
        InitializeComponent();
        _database = database;
        ChartView.Drawable = _chartDrawable;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        var rows = await _database.GetLatestAsync(80);
        UpdateUi(rows);
    }

    private async void OnAddTickClicked(object sender, EventArgs e)
    {
        await _database.AddRandomCandleAsync();
        await LoadAsync();
    }

    private async void OnResetClicked(object sender, EventArgs e)
    {
        await _database.ResetAsync();
        await LoadAsync();
    }

    private async void OnModal(object sender, EventArgs e)
    {
        //await Navigation.PushModalAsync(new SettingsPage());
        SettingsOverlay.IsVisible = true;
    }

    private void UpdateUi(List<StockCandle> rows)
    {
        _chartDrawable.Candles = rows;
        ChartView.Invalidate();

        RowsView.ItemsSource = rows.TakeLast(10).Reverse().ToList();
        RowsLabel.Text = rows.Count.ToString();

        if (rows.Count == 0)
            return;

        var last = rows[^1];
        var first = rows[0];
        var change = last.Close - first.Open;
        var percent = first.Open == 0 ? 0 : change / first.Open * 100m;

        LastPriceLabel.Text = $"{last.Close:0.00} USD";
        ChangeLabel.Text = $"{change:+0.00;-0.00;0.00} ({percent:+0.00;-0.00;0.00}%)";
        ChangeLabel.TextColor = change >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");
    }

    private void OnClose(object sender, EventArgs e)
    {
        SettingsOverlay.IsVisible = false;
    }

    private void OnSave(object sender, EventArgs e)
    {
        SettingsOverlay.IsVisible = false;
    }
}
