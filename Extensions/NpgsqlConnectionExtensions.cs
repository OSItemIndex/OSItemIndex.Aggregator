using System.Threading.Tasks;
using Npgsql;

namespace OSItemIndex.Data.Extensions
{
    public static class NpgsqlConnectionExtensions
    {
        public static async Task ExecuteNonQueryAsync(this NpgsqlConnection connection, string command)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = command;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async ValueTask<bool> EnsureConnectedAsync(this NpgsqlConnection conn)
        {
            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync();
                return true;
            }
            return false;
        }

    }
}
