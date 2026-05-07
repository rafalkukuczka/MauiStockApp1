using MauiApp1.Models;
using Microsoft.Maui.Graphics;

namespace MauiApp1.Services;

public sealed class StockChartDrawable : IDrawable
{
    public IReadOnlyList<StockCandle> Candles { get; set; } = Array.Empty<StockCandle>();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Color.FromArgb("#0B1220");
        canvas.FillRectangle(dirtyRect);

        if (Candles.Count < 2)
            return;

        var paddingLeft = 60f;
        var paddingRight = 28f;
        var paddingTop = 24f;
        var paddingBottom = 48f;

        var chart = new RectF(
            dirtyRect.X + paddingLeft,
            dirtyRect.Y + paddingTop,
            dirtyRect.Width - paddingLeft - paddingRight,
            dirtyRect.Height - paddingTop - paddingBottom);

        var max = (float)Candles.Max(x => x.High);
        var min = (float)Candles.Min(x => x.Low);
        var range = Math.Max(1f, max - min);

        canvas.StrokeColor = Color.FromArgb("#223047");
        canvas.StrokeSize = 1;

        for (var i = 0; i <= 5; i++)
        {
            var y = chart.Top + chart.Height * i / 5f;
            canvas.DrawLine(chart.Left, y, chart.Right, y);

            var value = max - range * i / 5f;
            canvas.FontColor = Color.FromArgb("#94A3B8");
            canvas.FontSize = 12;
            canvas.DrawString(value.ToString("0.00"), 6, y - 8, 50, 18, HorizontalAlignment.Right, VerticalAlignment.Center);
        }

        var step = chart.Width / Candles.Count;
        var bodyWidth = Math.Max(4f, step * 0.62f);

        float MapY(decimal value)
        {
            var v = (float)value;
            return chart.Bottom - ((v - min) / range * chart.Height);
        }

        for (var i = 0; i < Candles.Count; i++)
        {
            var candle = Candles[i];
            var x = chart.Left + i * step + step / 2f;

            var openY = MapY(candle.Open);
            var closeY = MapY(candle.Close);
            var highY = MapY(candle.High);
            var lowY = MapY(candle.Low);

            var isUp = candle.Close >= candle.Open;
            var color = isUp ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");

            canvas.StrokeColor = color;
            canvas.StrokeSize = 2;
            canvas.DrawLine(x, highY, x, lowY);

            var top = Math.Min(openY, closeY);
            var height = Math.Max(3f, Math.Abs(closeY - openY));

            canvas.FillColor = color;
            canvas.FillRectangle(x - bodyWidth / 2f, top, bodyWidth, height);
        }

        canvas.FontColor = Color.FromArgb("#CBD5E1");
        canvas.FontSize = 13;
        canvas.DrawString("SQLite candlestick data", chart.Left, dirtyRect.Bottom - 32, chart.Width, 20, HorizontalAlignment.Left, VerticalAlignment.Center);
    }
}
