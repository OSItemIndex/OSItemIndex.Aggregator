using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using OSItemIndex.Data;

namespace OSItemIndex.Aggregator.Services
{
    public interface IEntityRepoClient<T> where T : ItemEntity
    {
        Task<HttpResponseMessage> PostEntitiesAsync(EntityRepoUpdateRequest<T> request, string uri);
        Task<HttpResponseMessage> PostEntitiesAsync(IEnumerable<T> entities, EntityRepoVersion version, string uri);
    }
}
