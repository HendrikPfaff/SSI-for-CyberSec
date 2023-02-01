using SSI_Holder.ViewModels;
using ZXing.Net.Maui;

namespace SSI_Holder.Views;

public partial class ScanPage : ContentPage
{
    private readonly ScanViewModel _viewModel;

    public ScanPage(ScanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        barcodeReader.Options = new BarcodeReaderOptions()
        {
            AutoRotate = true,
            Formats = BarcodeFormats.All
        };
    }

    private void CameraBarcodeReaderView_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        _ = Dispatcher.Dispatch(async () =>
        {
            // Use dispatcher so our Main-thread does not crash.
            await _viewModel.ScanInvitation(e.Results[0].Value);
        });
    }
}