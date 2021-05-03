using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;

namespace OSItemIndex.Data.Extensions
{
    public static class DatabaseFacadeExtensions
    {
        public static NpgsqlConnection GetNpgsqlConnection(this DatabaseFacade database)
        {
            return (NpgsqlConnection) database.GetDbConnection();
        }
    }
}
