
using System.Collections.Generic;
using System.Threading.Tasks;
using OSItemIndex.Data;

namespace OSItemIndex.Updater.Services
{
    public interface IOsrsBoxClient
    {
        Task<ReleaseMonitoringProject?> GetProjectDetailsAsync();
        Task<IEnumerable<OsrsBoxItem>?> GetOsrsBoxItemsAsync();
    }
}
