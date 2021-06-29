using System.Net.Http;
using System.Threading.Tasks;

namespace OSItemIndex.Aggregator.Services
{
    public interface IOsrsBoxClient
    {
        Task<ReleaseMonitoringProject> GetProjectDetailsAsync();
        Task<HttpResponseMessage> GetOsrsBoxItemsAsync();
    }
}
