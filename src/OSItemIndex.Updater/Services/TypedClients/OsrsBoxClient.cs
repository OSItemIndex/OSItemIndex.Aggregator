using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using OSItemIndex.Data;
using Serilog;

namespace OSItemIndex.Updater.Services
{
    public class OsrsBoxClient : IOsrsBoxClient
    {
        private readonly HttpClient _client;

        public OsrsBoxClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", Constants.ObserverUserAgent);
            _client = client;
        }

        public async Task<ReleaseMonitoringProject?> GetProjectDetailsAsync()
        {
            using var response = await _client.GetAsync(Endpoints.OsrsBox.Project);
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadFromJsonAsync<ReleaseMonitoringProject>();
                    return json;
                }
                catch (HttpRequestException e)
                {
                    Log.Error(e, "Response status not OK");
                }
                catch (JsonException e)
                {
                    Log.Error(e, "JSON exception caught");
                }
            }
            return null;
        }

        public async Task<IEnumerable<OsrsBoxItem>?> GetOsrsBoxItemsAsync()
        {
            using var response = await _client.GetAsync(Endpoints.OsrsBox.ItemsComplete);
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadFromJsonAsync<IDictionary<string, OsrsBoxItem>>();
                    return json?.Values;
                }
                catch (HttpRequestException e)
                {
                    Log.Error(e, "Response status not OK");
                }
                catch (JsonException e)
                {
                    Log.Error(e, "JSON exception caught");
                }
            }
            return null;
        }
    }
}
