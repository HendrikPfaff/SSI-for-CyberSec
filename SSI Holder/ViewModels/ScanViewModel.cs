using SSI_Holder.Interfaces;
using SSI_Holder.Views;

namespace SSI_Holder.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IHyperledgerService _hyperledgerService;
        private readonly WalletViewModel _walletViewModel;

        public ScanViewModel(INavigationService navigationService, IHyperledgerService hyperledgerService, WalletViewModel walletViewModel)
        {
            _navigationService = navigationService;
            _hyperledgerService = hyperledgerService;
            _walletViewModel = walletViewModel;
        }

        public async Task ScanInvitation(string invitationJson)
        {
            // ReceiveInvitation
            _ = await _hyperledgerService.ReceiveInvitationAsync(invitationJson);
            await _walletViewModel.LoadConnections();
            // Go back to Wallet page.
            await _navigationService.GoToAsync("///WalletPage");
        }
    }
}
