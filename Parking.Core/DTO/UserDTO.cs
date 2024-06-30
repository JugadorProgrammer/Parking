using System.ComponentModel.DataAnnotations;
using Parking.Core.DataBase.Models;

namespace Parking.Core.DTO
{
    public class UserDTO
    {
        [Required]
        [StringLength(30, MinimumLength = 6)]
        public required string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(30, MinimumLength = 6)]
        public required string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 6)]
        public required string Password { get; set; }

        public User GetUser() 
        {
            return new User()
            {
                Name = Name, 
                Email = Email,
                Password = Password
            };
        }
    }
}
