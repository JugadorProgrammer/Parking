

namespace Parking.Core.DataBase.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string? Guid { get; set; }
    }
}
