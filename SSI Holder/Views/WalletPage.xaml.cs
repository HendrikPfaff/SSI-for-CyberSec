using SSI_Holder.ViewModels;

namespace SSI_Holder.Views;

public partial class WalletPage : ContentPage
{
	private readonly WalletViewModel _viewModel;

	public WalletPage(WalletViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_ = _viewModel.BackgroundPolling().GetAwaiter();
	}
}