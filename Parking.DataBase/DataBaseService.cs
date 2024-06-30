using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Parking.Core.DataBase;
using Parking.Core.DataBase.Models;

namespace Parking.DataBase
{
    public class DataBaseService : IDataBaseService
    {
        private readonly string _connectionString;
        public DataBaseService(IConfiguration configuration)
        {
            _connectionString = configuration["DatabaseSettings:ConnectionString"]!;
        }

        #region User
        public async Task<User?> GetUser(int userId)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                return (await db.QueryAsync<User>("SELECT * FROM \"User\" WHERE \"Id\"=@userId",
                    new { userId })).FirstOrDefault();
            }
        }
        public async Task<User?> GetUser(string name)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                return (await db.QueryAsync<User>("SELECT * FROM \"User\" WHERE \"Name\"=@name",
                    new { name })).FirstOrDefault();
            }
        }

        public async Task<User?> GetUser(string name, string password)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                return (await db.QueryAsync<User>("SELECT * FROM \"User\" WHERE \"Name\"=@name AND \"Password\"=@password",
                    new { name, password })).FirstOrDefault();
            }
        }

        public async Task<User?> GetUserByGuid(string name, string guid)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                return (await db.QueryAsync<User>("SELECT * FROM \"User\" WHERE \"Name\"=@name AND \"Guid\"=@Guid ",
                    new { name, guid })).FirstOrDefault();
            }
        }

        public async Task UpdateUser(User user)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                await db.QueryAsync<User>("UPDATE \"User\" SET \"Name\"=@Name, \"Email\"=@Email, \"Password\"=@Password, \"IsEmailConfirmed\"=@IsEmailConfirmed, \"Guid\"=@Guid WHERE \"Id\"=@Id", user);
            }
        }

        public async Task DeleteUser(int userId)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                await db.QueryAsync<User>("DELETE FROM \"User\" WHERE \"Id\"=@userId", new { userId });
            }
        }

        public async Task<User> CreateUser(User user)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                user.Id = (await db.QueryAsync<int>("INSERT INTO \"User\"(\"Name\", \"Password\", \"Email\", \"Guid\") VALUES(@Name, @Password, @Email, @Guid) RETURNING \"Id\"", user)).FirstOrDefault();
                return user;
            }
        }
        #endregion
    }
}
