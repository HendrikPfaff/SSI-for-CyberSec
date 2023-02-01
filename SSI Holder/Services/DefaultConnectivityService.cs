using SSI_Holder.Interfaces;

namespace SSI_Holder.Services
{
    internal class DefaultConnectivityService : IConnectivityService
    {
        public bool IsConnected()
        {
            bool result = false;
            NetworkAccess access = Connectivity.Current.NetworkAccess;
            if(access == NetworkAccess.Internet || access == NetworkAccess.ConstrainedInternet)
            {
                result = true;
            }
            return result;
        }
    }
}
