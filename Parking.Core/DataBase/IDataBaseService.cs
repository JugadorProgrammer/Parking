using Parking.Core.DataBase.Models;

namespace Parking.Core.DataBase
{
    public interface IDataBaseService
    {
        #region User
        Task<User?> GetUser(int userId);
        Task<User?> GetUser(string name);
        Task<User?> GetUser(string name, string password);
        Task<User?> GetUserByGuid(string name, string guid);
        Task UpdateUser(User user);
        Task DeleteUser(int userId);
        Task<User> CreateUser(User user);
        #endregion
    }
}
