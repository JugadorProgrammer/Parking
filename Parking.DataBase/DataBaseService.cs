using Parking.Core.DataBase;

namespace Parking.DataBase
{
    public class DataBaseService : IDataBaseService
    {
        private readonly string _connectionString;
        public DataBaseService(string connectionString) 
        {
            _connectionString = connectionString;
        }
    }
}
