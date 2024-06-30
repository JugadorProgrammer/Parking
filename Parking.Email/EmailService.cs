using System.Net.Mail;
using System.Net;
using System.Text;
using Parking.Core.Email;
using Microsoft.Extensions.Configuration;

namespace Parking.Email
{
    public class EmailService : IEmailService
    {
        private readonly string _senderName;
        private readonly string _sourceEmailAdress;
        private readonly string _sourceEmailPassword;
        public EmailService(IConfiguration configuration)
        {
            _senderName = configuration["EmailSettings:SenderName"]!;
            _sourceEmailAdress = configuration["EmailSettings:SenderEmailAdress"]!;
            _sourceEmailPassword = configuration["EmailSettings:SenderEmailPassword"]!;
        }

        public async Task SendEmail(string distanationEmailAdress, string themeMessage, string message, bool IsBodyHtml = true)
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress(_sourceEmailAdress, _senderName, Encoding.UTF8);
            // кому отправляем
            MailAddress to = new MailAddress(distanationEmailAdress);

            // создаем объект сообщения
            using (var m = new MailMessage(from, to))
            {
                // тема письма
                m.Subject = themeMessage;
                // текст письма
                m.Body = message;
                // письмо представляет код html
                m.IsBodyHtml = IsBodyHtml;
                // адрес smtp-сервера и порт, с которого будем отправлять письмо
                using (var smtp = new SmtpClient("smtp.mail.ru", 2525))
                {
                    // логин и пароль
                    smtp.Credentials = new NetworkCredential(_sourceEmailAdress, _sourceEmailPassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(m);
                }
            }
        }
    }
}
