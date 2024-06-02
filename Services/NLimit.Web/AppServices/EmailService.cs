using MailKit.Net.Smtp;
using MimeKit;

namespace NLimit.Web.AppServices;

public class EmailService
{
    public async Task SendEmailAsync (string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("Тестовый запуск", "r.r.53@bk.ru"));
        emailMessage.To.Add(new MailboxAddress(string.Empty, email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = message
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.mail.ru", 465, true); // 25 - без шифрования, 465 - с шифрованием
            await client.AuthenticateAsync("r.r.53@bk.ru", "UnCK9hUxqLMwm80Urbr7");
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }
    }
}
