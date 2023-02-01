using SSI_Holder.Interfaces;
using SSI_Holder.Services;
using SSI_Holder.ViewModels;
using SSI_Holder.Views;
using ZXing.Net.Maui;

namespace SSI_Holder;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        _ = builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                _ = fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                _ = fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                _ = fonts.AddFont("fa-solid-900.ttf", "FontAwesome");
            })
            .ConfigureMauiHandlers(h =>
            {
                h.AddHandler(typeof(ZXing.Net.Maui.Controls.CameraBarcodeReaderView), typeof(CameraBarcodeReaderViewHandler));
                h.AddHandler(typeof(ZXing.Net.Maui.Controls.CameraView), typeof(CameraViewHandler));
                h.AddHandler(typeof(ZXing.Net.Maui.Controls.BarcodeGeneratorView), typeof(BarcodeGeneratorViewHandler));
            });

        RegisterServices(builder);
        RegisterViews(builder);
        RegisterViewModels(builder);

        return builder.Build();
    }

    private static void RegisterServices(MauiAppBuilder builder)
    {
        _ = builder.Services.AddSingleton<IConnectivityService, DefaultConnectivityService>();
        _ = builder.Services.AddSingleton<INavigationService, DefaultNavigationService>();
        _ = builder.Services.AddSingleton<IHyperledgerService, DefaultHyperledgerService>();
        _ = builder.Services.AddSingleton<HttpClient>();
    }

    private static void RegisterViews(MauiAppBuilder builder)
    {
        _ = builder.Services.AddSingleton<WalletPage>();
        _ = builder.Services.AddSingleton<ScanPage>();
    }

    private static void RegisterViewModels(MauiAppBuilder builder)
    {
        _ = builder.Services.AddSingleton<WalletViewModel>();
        _ = builder.Services.AddSingleton<ScanViewModel>();
    }
}
