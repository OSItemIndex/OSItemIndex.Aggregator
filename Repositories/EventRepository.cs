using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OSItemIndex.Data.Database;
using Serilog;

namespace OSItemIndex.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IDbContextHelper _context;

        public EventRepository(IDbContextHelper context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            await using (var factory = _context.GetFactory())
            {
                var dbContext = factory.GetDbContext();
                return await dbContext.Set<Event>().OrderBy(e => e.Id).ToListAsync();
            }
        }

        public async Task<IEnumerable<Event>> GetAllAsync(Expression<Func<Event, Event>> select)
        {
            await using (var factory = _context.GetFactory())
            {
                var dbContext = factory.GetDbContext();
                return await dbContext.Set<Event>().Select(select).OrderBy(e => e.Id).ToListAsync();
            }
        }

        public async Task<IEnumerable<Event>> GetAllAsync(Expression<Func<Event, bool>> filter, Func<IQueryable<Event>, IOrderedQueryable<Event>>? orderBy = null, Expression<Func<Event, Event>>? select = null)
        {
            await using (var factory = _context.GetFactory())
            {
                var dbContext = factory.GetDbContext();
                var query = dbContext.Set<Event>().Where(filter);

                if (select != null)
                {
                    query = query.Select(select);
                }

                return orderBy != null ? await orderBy(query).ToListAsync() : await query.ToListAsync();
            }
        }

        public async Task<Event> GetMostRecentAsync(Expression<Func<Event, bool>>? filter)
        {
            await using (var factory = _context.GetFactory())
            {
                var dbContext = factory.GetDbContext();
                var query = dbContext.Set<Event>().AsQueryable();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                return await query.OrderByDescending(entity => entity.Timestamp).FirstOrDefaultAsync();
            }
        }

        public async Task AddAsync(Event e)
        {
            await using (var factory = _context.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                e.Timestamp = DateTime.UtcNow;

                await dbContext.Set<Event>().AddAsync(e);
                await dbContext.SaveChangesAsync();

                Log.Information("Event submitted > {@Event}", e);
            }
        }
    }
}
