namespace OSItemIndex.Data.Database
{
    public class DbOptions
    {
        public const string Section = "DbOptions";

        public string? DbUser { get; set; } = "postgres";
        public string? DbPass { get; set; } = "localdev";
        public string DbHost { get; set; } = "localhost";
        public int DbPort { get; set; } = 5432;
        public string DbDatabase { get; set; } = "ositemindex";
        public int PgPoolSize { get; set; } = 100;
    }
}
