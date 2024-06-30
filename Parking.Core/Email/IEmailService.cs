

namespace Parking.Core.Email
{
    public interface IEmailService
    {
        Task SendEmail(string distanationEmailAdress, string themeMessage, string message, bool IsBodyHtml = true);
    }
}
