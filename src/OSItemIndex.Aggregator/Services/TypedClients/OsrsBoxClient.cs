using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OSItemIndex.Data;
using Serilog;

namespace OSItemIndex.Aggregator.Services
{
    public class OsrsBoxClient : IOsrsBoxClient
    {
        private HttpClient Client { get; }

        public OsrsBoxClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", Constants.ObserverUserAgent);
            Client = client;
        }

        public async Task<ReleaseMonitoringProject> GetProjectDetailsAsync()
        {
            try
            {
                // GetFromJsonAsync will only return null if there's an exception, which we catch
                return (await Client.GetFromJsonAsync<ReleaseMonitoringProject>(Endpoints.OsrsBox.Project))!;
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to retrieve and deserialize project details for osrsbox from release-monitoring project");
                throw;
            }
        }

        public async Task<HttpResponseMessage> GetOsrsBoxItemsAsync()
        {
            try
            {
                return await Client.GetAsync(Endpoints.OsrsBox.ItemsComplete);
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Failed to retrieve complete items from osrsbox static json api");
                throw;
            }
        }

        public async Task<HttpResponseMessage> PostEntitiesAsync(EntityRepoUpdateRequest<OsrsBoxItem> request, string uri)
        {
            try
            {
                return await Client.PostAsJsonAsync(Endpoints.OsItemIndex.Api + uri, request);
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Failed to POST entities to {@Uri}", uri);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PostEntitiesAsync(IEnumerable<OsrsBoxItem> entities, EntityRepoVersion version,
                                                                 string uri)
        {
            return await PostEntitiesAsync(new EntityRepoUpdateRequest<OsrsBoxItem>
                                               { Entities = entities, Version = version }, uri);
        }
    }
}
