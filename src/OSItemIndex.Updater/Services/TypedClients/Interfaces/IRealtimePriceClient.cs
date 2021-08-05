using System.Collections.Generic;
using System.Threading.Tasks;
using OSItemIndex.Data;

namespace OSItemIndex.Updater.Services
{
    public interface IRealtimePriceClient
    {
        Task<IEnumerable<T>?> GetPricesAsync<T>(string uri) where T: PriceEntity;
    }
}
