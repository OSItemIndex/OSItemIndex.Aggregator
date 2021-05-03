using static OSItemIndex.Data.Database.DbContextHelper;

namespace OSItemIndex.Data.Database
{
    public interface IDbContextHelper
    {
        DbContextFactory GetFactory();
    }
}
