using MauiApp1.Models;
using SQLite;

namespace MauiApp1.Services;

public sealed class StockDatabase
{
    private readonly SQLiteAsyncConnection _database;
    private bool _initialized;
    private readonly Random _random = new();

    public StockDatabase()
    {
        SQLitePCL.Batteries_V2.Init();

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "stocks.db3");
        _database = new SQLiteAsyncConnection(databasePath);
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        await _database.CreateTableAsync<StockCandle>();
        var count = await _database.Table<StockCandle>().CountAsync();

        if (count == 0)
            await SeedAsync();

        _initialized = true;
    }

    public async Task<List<StockCandle>> GetLatestAsync(int take = 80)
    {
        await InitializeAsync();

        var rows = await _database.Table<StockCandle>()
            .OrderByDescending(x => x.Timestamp)
            .Take(take)
            .ToListAsync();

        rows.Reverse();
        return rows;
    }

    public async Task<StockCandle> AddRandomCandleAsync()
    {
        await InitializeAsync();

        var last = await _database.Table<StockCandle>()
            .OrderByDescending(x => x.Timestamp)
            .FirstOrDefaultAsync();

        var previousClose = last?.Close ?? 189m;
        var open = previousClose;
        var delta = (decimal)(_random.NextDouble() * 4.0 - 2.0);
        var close = Math.Max(1m, open + delta);
        var high = Math.Max(open, close) + (decimal)(_random.NextDouble() * 1.8);
        var low = Math.Min(open, close) - (decimal)(_random.NextDouble() * 1.8);

        var candle = new StockCandle
        {
            Timestamp = (last?.Timestamp ?? DateTime.Now.AddMinutes(-1)).AddMinutes(1),
            Open = decimal.Round(open, 2),
            High = decimal.Round(high, 2),
            Low = decimal.Round(low, 2),
            Close = decimal.Round(close, 2),
            Volume = _random.Next(5_000, 80_000)
        };

        await _database.InsertAsync(candle);
        return candle;
    }

    public async Task ResetAsync()
    {
        await InitializeAsync();
        await _database.DeleteAllAsync<StockCandle>();
        await SeedAsync();
    }

    private async Task SeedAsync()
    {
        var candles = new List<StockCandle>();
        var price = 185m;
        var start = DateTime.Now.Date.AddHours(9);

        for (var i = 0; i < 80; i++)
        {
            var open = price;
            var close = Math.Max(1m, open + (decimal)(_random.NextDouble() * 3.6 - 1.6));
            var high = Math.Max(open, close) + (decimal)(_random.NextDouble() * 1.4);
            var low = Math.Min(open, close) - (decimal)(_random.NextDouble() * 1.4);

            candles.Add(new StockCandle
            {
                Timestamp = start.AddMinutes(i * 5),
                Open = decimal.Round(open, 2),
                High = decimal.Round(high, 2),
                Low = decimal.Round(low, 2),
                Close = decimal.Round(close, 2),
                Volume = _random.Next(5_000, 80_000)
            });

            price = close;
        }

        await _database.InsertAllAsync(candles);
    }
}
