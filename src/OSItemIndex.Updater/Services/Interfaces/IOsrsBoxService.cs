using System.Threading.Tasks;

namespace OSItemIndex.Updater.Services
{
    public interface IOsrsBoxService : IStatefulService
    {
        Task AggregateAsync();
    }
}
