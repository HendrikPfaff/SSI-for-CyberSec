using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSI_Holder.Interfaces;
using SSI_Holder.Models;
using System.Diagnostics;
using System.Net.Http.Json;

namespace SSI_Holder.Services
{
    public class DefaultHyperledgerService : IHyperledgerService
    {
        private readonly string _holderEndpoint = "http://38.242.248.237:8091";
        private readonly IConnectivityService _connectivityService;
        private readonly HttpClient _httpClient;

        public DefaultHyperledgerService(IConnectivityService connectivityService, HttpClient httpClient)
        {
            _connectivityService = connectivityService;
            _httpClient = httpClient;
        }

        public async Task<string> ReceiveInvitationAsync(string invitationJson)
        {
            string result = string.Empty;
            if (_connectivityService.IsConnected())
            {
                try
                {
                    string tmp = invitationJson[1..^1];
                    HttpRequestMessage httpRequest = new(HttpMethod.Post, $"{_holderEndpoint}/connections/receive-invitation");
                    JsonContent tmpContent = JsonContent.Create(tmp);
                    httpRequest.Content = tmpContent;
                    HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest);
                    result = await httpResponse.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            else
            {
                Debug.WriteLine("No internet connection");
            }
            return result;
        }

        public async Task<List<Credential>> ReceiveCredentialsAsync()
        {
            List<Credential> result = new();
            try
            {
                string response = await _httpClient.GetStringAsync($"{_holderEndpoint}/credentials");
                if(response != "{\"results\": []}")
                {
                    JObject tmp = JsonConvert.DeserializeObject<JObject>(response);
                    result = JsonConvert.DeserializeObject<List<Credential>>(tmp["results"].ToString());
                }                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return result;
        }

        public async Task<List<Connection>> ReceiveConnectionsAsync(ConnectionState state = ConnectionState.all)
        {
            List<Connection> result = new();
            try
            {
                string parameter = "";
                if(state != ConnectionState.all)
                {
                    parameter = $"?state={Enum.GetName(state)}";
                }

                string response = await _httpClient.GetStringAsync($"{_holderEndpoint}/connections{parameter}");
                if (response != string.Empty)
                {
                    JObject tmp = JsonConvert.DeserializeObject<JObject>(response);
                    result = JsonConvert.DeserializeObject<List<Connection>>(tmp["results"].ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return result;
        }

        public async Task<List<CredentialRecord>> ReceiveRecordsAsync()
        {
            List<CredentialRecord> result = new();
            try
            {
                string response = await _httpClient.GetStringAsync($"{_holderEndpoint}/issue-credential/records");
                if(response != "{\"results\": []}")
                {
                    JObject tmp = JsonConvert.DeserializeObject<JObject>(response);
                    result = JsonConvert.DeserializeObject<List<CredentialRecord>>(tmp["results"].ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return result;
        }
    }
}
