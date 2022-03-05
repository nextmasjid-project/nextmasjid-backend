using Microsoft.Data.Sqlite;

namespace NextMasjid.Backend.Core
{
    public class MasterConnectionHolder
    {
        public SqliteConnection MasterConnection { get; set; }

        public MasterConnectionHolder()
        {
        }
    }
}
