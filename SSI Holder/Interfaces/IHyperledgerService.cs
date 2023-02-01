using SSI_Holder.Models;

namespace SSI_Holder.Interfaces
{
    public interface IHyperledgerService
    {
        Task<string> ReceiveInvitationAsync(string invitationJson);
        Task<List<Credential>> ReceiveCredentialsAsync();
        Task<List<Connection>> ReceiveConnectionsAsync(ConnectionState state = ConnectionState.all);
        Task<List<CredentialRecord>> ReceiveRecordsAsync();
    }
}
