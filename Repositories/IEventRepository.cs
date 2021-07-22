using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OSItemIndex.Data.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<IEnumerable<Event>> GetAllAsync(Expression<Func<Event, Event>> select);
        Task<IEnumerable<Event>> GetAllAsync(
            Expression<Func<Event, bool>> filter,
            Func<IQueryable<Event>, IOrderedQueryable<Event>>? orderBy = null,
            Expression<Func<Event, Event>>? select = null);
        Task<Event> GetMostRecentAsync(Expression<Func<Event, bool>>? filter = null);
        Task AddAsync(Event e);
    }
}
