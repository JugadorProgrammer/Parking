using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Parking.Core.Account;
using Parking.Core.DataBase;
using Parking.Core.DataBase.Models;
using Parking.Core.Email;

namespace Parking.Account
{
    public class AccountService : IAccountService
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;
        public AccountService(IDataBaseService dataBaseService, PasswordHasher passwordHasher, IEmailService emailService)
        {
            _dataBaseService = dataBaseService;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
        }

        public async Task DropPassword(User user)
        {
            
        }

        public async Task SetPassword(User user)
        {

            await _dataBaseService.UpdateUser(user);
        }

        public Task SetPassword(string userName, string newUserPassword, string userGuidPassword)
        {

        }

        public Task SingUpUser(User user, HttpContext httpContext)
        {

        }
    }
}
