using System;
using System.Collections.Generic;
using System.Linq;
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
                return await dbContext.Set<Event>().OrderByDescending(entity => entity.Id).ToListAsync();
            }
        }

        public async Task<Event> GetMostRecentAsync()
        {
            await using (var factory = _context.GetFactory())
            {
                var dbContext = factory.GetDbContext();
                return await dbContext.Set<Event>().OrderByDescending(entity => entity.Timestamp).FirstOrDefaultAsync();
            }
        }

        public async Task SubmitAsync(Event e)
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
