using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;

namespace MauiApp1;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var page = _services.GetRequiredService<MainPage>();
        var window = new Window(page)
        {
            Title = "MauiApp1 Stock SQLite"
        };

        window.Width = 1400;
        window.Height = 850;

#if WINDOWS
        window.Created += (s, e) =>
        {
            var mauiWindow = s as Window;

            var nativeWindow = mauiWindow?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;

            IntPtr hWnd = WindowNative.GetWindowHandle(nativeWindow);

            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

            var appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);

            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Maximize();
            }
        };
#endif

        return window;
    }
}
