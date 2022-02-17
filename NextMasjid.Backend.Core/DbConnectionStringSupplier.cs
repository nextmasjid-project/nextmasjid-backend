namespace NextMasjid.Backend.Core
{
    public class DbConnectionStringSupplier
    {
        public readonly string _connectionString;

        public DbConnectionStringSupplier(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
