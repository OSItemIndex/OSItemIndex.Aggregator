using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OSItemIndex.Data;
using Serilog;

namespace OSItemIndex.Aggregator.Services
{
    public class EntityRepoClient<T> : IEntityRepoClient<T> where T : ItemEntity
    {
        private HttpClient Client { get; }

        public EntityRepoClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", Constants.ObserverUserAgent);
            Client = client;
        }

        public async Task<HttpResponseMessage> PostEntitiesAsync(EntityRepoUpdateRequest<T> request, string uri)
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

        public async Task<HttpResponseMessage> PostEntitiesAsync(IEnumerable<T> entities, EntityRepoVersion version,
                                                                 string uri)
        {
            return await PostEntitiesAsync(new EntityRepoUpdateRequest<T>
                                               { Entities = entities, Version = version }, uri);
        }
    }
}
