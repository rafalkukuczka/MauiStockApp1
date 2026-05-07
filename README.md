# MauiApp1 - Stock Chart with SQLite

MAUI Windows desktop application showing a stock-market dashboard with candlestick data stored in local SQLite.

## Run

1. Open `MauiApp1.csproj` in Visual Studio 2022.
2. Select Windows target.
3. Restore NuGet packages.
4. Run.

## What it does

- Creates local SQLite database: `stocks.db3`
- Seeds sample candlestick records
- Draws a candlestick chart using `GraphicsView`
- Adds new random ticks with the `Add tick` button
- Resets the database with the `Reset DB` button

## Important

This project is Windows-only on purpose:

```xml
<TargetFramework>net9.0-windows10.0.19041.0</TargetFramework>
<WindowsPackageType>None</WindowsPackageType>
```

That avoids the broken AppxManifest error you were hitting.

#Below the article illustrating the app
https://pkey.info/knowledge-base/net-maui-stock-trading-app-sqlite/
