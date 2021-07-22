using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSItemIndex.Data.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync(EventSource? source = null, EventType? type = null);
        Task<Event> GetMostRecentAsync(EventSource? source = null, EventType? type = null);
    }
}
