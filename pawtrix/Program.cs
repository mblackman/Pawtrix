using Avalonia;
using System;
using System.Net.Http;
using Meowtrix.Sdk;
using pawtrix.ViewModels;
using pawtrix.Views;

namespace pawtrix;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

    private static readonly MatrixClientFactory ClientFactory = new();
    public static readonly IMatrixClient Client = ClientFactory.Create();

    public static readonly HttpClient HttpClient = new HttpClient();
}