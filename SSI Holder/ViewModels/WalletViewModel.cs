using SSI_Holder.Interfaces;
using SSI_Holder.Models;
using SSI_Holder.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SSI_Holder.ViewModels
{
    public class WalletViewModel : BaseViewModel
    {
        #region Private Protperties
        private ObservableCollection<Credential> _credentials = new();
        private ObservableCollection<Connection> _connections = new();
        private readonly INavigationService _navigationService;
        private readonly IHyperledgerService _hyperledgerService;
        #endregion

        #region Public Properties
        public ObservableCollection<Credential> Credentials
        {
            get => _credentials;
            set => SetProperty(ref _credentials, value);
        }
        public ObservableCollection<Connection> Connections
        {
            get => _connections;
            set => SetProperty(ref _connections, value);
        }
        #endregion

        #region Commands
        public Command ScanButtonTappedCommand { get; private set; }
        #endregion

        public WalletViewModel(INavigationService navigationService, IHyperledgerService hyperledgerService)
        {
            _navigationService = navigationService;
            _hyperledgerService = hyperledgerService;

            ScanButtonTappedCommand = new Command(async () => await OnScanButtonTapped());
            
            // Load the Wallet contents.
            _ = LoadConnections().GetAwaiter();
            _ = LoadCredentials().GetAwaiter();            
        }

        public async Task BackgroundPolling()
        {
            // Polling for new records.
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
            while (await timer.WaitForNextTickAsync())
            {
                await LoadRecords();
            }
        }

        private async Task OnScanButtonTapped()
        {
            await _navigationService.GoToAsync(nameof(ScanPage));
        }

        public async Task LoadConnections()
        {
            List<Connection> allConnections = await _hyperledgerService.ReceiveConnectionsAsync();
            Connections.Clear();
            foreach (Connection connection in allConnections)
            {
                if (connection.State is ConnectionState.active or ConnectionState.response)
                {
                    Connections.Add(connection);
                }
            }
        }

        public async Task LoadCredentials()
        {
            Credentials = new ObservableCollection<Credential>(await _hyperledgerService.ReceiveCredentialsAsync());            
        }

        private async Task LoadRecords()
        {
            await _hyperledgerService.ReceiveRecordsAsync();
        }
    }
}
