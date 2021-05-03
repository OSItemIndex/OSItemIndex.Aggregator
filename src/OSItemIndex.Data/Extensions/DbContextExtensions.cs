using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace OSItemIndex.Data.Extensions
{
    public static class DbContextExtensions
    {
        public static IDbContextTransaction? EnsureOrStartTransaction(this DbContext context, IsolationLevel isolation)
        {
            if (context.Database.CurrentTransaction != null)
                return null;

            if (System.Transactions.Transaction.Current != null)
                return null;

            return context.Database.BeginTransaction(isolation);
        }
    }
}
