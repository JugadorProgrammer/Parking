using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Parking.Core.DataBase;
using Parking.Core.DataBase.Models;
using Parking.Core.Email;
using Parking.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace Parking.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly string _hostName;
        private readonly IDataBaseService _dataBaseService;
        private readonly IEmailService _emailService;
        public AccountController(IServiceProvider serviceProvider)
        {
            _dataBaseService = serviceProvider.GetService<IDataBaseService>()!;
            _emailService = serviceProvider.GetService<IEmailService>()!;
            _hostName = ((IConfiguration)serviceProvider.GetService(typeof(IConfiguration))!)["Host:Name"]!;
        }

        [HttpPost]
        [Authorize]
        [Route("[action]/{userName}")]
        public async Task<IActionResult> DropPassword(string userName)
        {
            var user = await _dataBaseService.GetUser(userName);

            if (user is null)
            {
                return NotFound("User not found!");
            }

            user.Guid = GetNewUserGuid();

            var message = $@"<h3>To recover your password, follow the link - {_hostName}/Account/SetPassword/{user.Guid} </h3> 
<h5>If you do not want to recover your password, ignore this letter.</h5>";

            await _dataBaseService.UpdateUser(user);
            await _emailService.SendEmail(user.Email, "Recover you password!", message);

            return Ok();
        }

        [HttpPost]
        [Route("[action]/{userName}/{guid}/{newUserPassword}")]
        public async Task<IActionResult> SetPassword(string userName, string guid, string newUserPassword)
        {
            var user = await _dataBaseService.GetUserByGuid(userName, guid);

            if (user is null)
            {
                return NotFound("User not found!");
            }

            user.Password = newUserPassword;
            await _dataBaseService.UpdateUser(user);

            return Ok();
        }

        [HttpPost]
        [Route("[action]/{userName}/{userPassword}")]
        public async Task<IActionResult> Login(string userName, string userPassword)
        {
            var user = await _dataBaseService.GetUser(userName, GetHash(userPassword));

            if (user is null)
            {
                return NotFound("User not found!");
            }
            else if (!user.IsEmailConfirmed)
            {
                return BadRequest("Email is not confirmed!");
            }

            await Authenticate(user.Name);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SingUp([ModelBinder] UserDTO userDTO)
        {
            var user = userDTO.GetUser();
            user.Guid = GetNewUserGuid();
            user.Password = GetHash(user.Password);
            await _dataBaseService.CreateUser(user);

            var message = $@"<h3>To confirme your account, follow the link - {_hostName}/Account/ConfirmeEmail/{user.Guid} </h3> 
<h5>If you do not want to confirme your account, ignore this letter.</h5>";
            await _emailService.SendEmail(user.Email, "Confirme your mail!", message);
            return Ok();
        }

        [HttpPost]
        [Route("[action]/{userName}/{guid}")]
        public async Task<IActionResult> ConfirmeEmail(string userName, string guid)
        {
            var user = await _dataBaseService.GetUserByGuid(userName, guid);

            if (user is null)
            {
                return NotFound("User not found");
            }

            user.Guid = null;
            user.IsEmailConfirmed = true;
            await _dataBaseService.UpdateUser(user);

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SendNewConfirmeEmail(string userName)
        {
            var user =  await _dataBaseService.GetUser(userName);

            if (user is null)
            {
                return NotFound("User not found!");
            }
            else if (user.IsEmailConfirmed)
            {
                return BadRequest("Email is not confirmed!");
            }
            var guid = GetNewUserGuid();

            var message = $@"<h3>To confirme your account, follow the link - {_hostName}/Account/ConfirmeEmail/{guid} </h3> 
<h5>If you do not want to confirme your account, ignore this letter.</h5>";
            await _emailService.SendEmail(user.Email, "Confirme your mail!", message);

            user.Guid = guid;
            await _dataBaseService.UpdateUser(user);
            return Ok();
        }

        #region NonAction
        [NonAction]
        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [NonAction]
        private string GetNewUserGuid()
        {
            return Guid.NewGuid().ToString().Replace("/", "").Replace("%", "");
        }
        [NonAction]
        private string GetHash(string stringToHash)
        {
            using (var sha512 = SHA512.Create())
            {
                var hashedBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));
                var builder = new StringBuilder();

                for (int i = 0; i < hashedBytes.Length; ++i)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }

                return builder.ToString().Substring(0, 99);
            }
        }
        #endregion
    }
}
