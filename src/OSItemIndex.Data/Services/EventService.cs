using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSItemIndex.Data.Repositories;

namespace OSItemIndex.Data.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync(EventSource? source = null, EventType? type = null)
        {
            return await _eventRepository.GetAllAsync(e =>
                                                          (source == null || e.Source.HasFlag(source)) &&
                                                          (type == null || e.Type == type),
                                                      events => events.OrderBy(e => e.Id));
        }

        public async Task<Event> GetMostRecentAsync(EventSource? source = null, EventType? type = null)
        {
            return await _eventRepository.GetMostRecentAsync(e =>
                                                                 (source == null || e.Source.HasFlag(source)) &&
                                                                 (type == null || e.Type == type));
        }
    }
}
