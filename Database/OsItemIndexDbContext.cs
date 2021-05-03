using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace OSItemIndex.Data.Database
{
    public class OsItemIndexDbContext : DbContext
    {
        public DbSet<OsrsBoxItem> Items { get; set; }
        public DbSet<RealtimeItemPrice> PricesRealtime { get; set; }

        public OsItemIndexDbContext(DbContextOptions<OsItemIndexDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entity in builder.Model.GetEntityTypes())
            {
                entity.SetTableName(ToSnakeCase(entity.GetTableName()));

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName($"p_k_{ToSnakeCase(key.GetName())}");
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.PrincipalKey.SetName($"f_k_{ToSnakeCase(key.PrincipalKey.GetName())}");
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName($"i_x_{ToSnakeCase(index.GetDatabaseName())}");
                }
            }
        }

        private static string ToSnakeCase(string str)
        {
            Regex pattern = new(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
            return string.Join("_", pattern.Matches(str)).ToLower();
        }
    }
}
