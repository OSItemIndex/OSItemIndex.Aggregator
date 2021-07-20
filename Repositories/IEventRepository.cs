using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSItemIndex.Data.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event> GetMostRecentAsync();
        Task SubmitAsync(Event e);
    }
}
