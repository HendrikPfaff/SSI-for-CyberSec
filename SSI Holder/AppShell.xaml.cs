using SSI_Holder.Views;

namespace SSI_Holder;

public partial class AppShell : Shell
{
	public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

	public AppShell()
	{
		InitializeComponent();
		RegisterRoutes();
	}

	private void RegisterRoutes()
	{
		Routes.Add(nameof(WalletPage), typeof(WalletPage));
		Routes.Add(nameof(ScanPage), typeof(ScanPage));

        foreach (KeyValuePair<string, Type> item in Routes)
        {
            Routing.RegisterRoute(item.Key, item.Value);
        }
    }
}
