using System.Net;
using System.Net.Mail;
using MongoDB.Bson;

namespace Common.Notification.Email;


public interface IEmailService
{
    Task SendReserveEmailAsync(Dictionary<string, string> data);
}

public class EmailService : IEmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _port = 587;
    private readonly string _senderEmail = "hallam.amls@gmail.com";
    private readonly string _appPassword = "dktpdtqizcxwjerg";

    //TODO: Remove inline HTML for E-mail Content

    private async Task SendEmailAsync(string recipient, string subject, string body, bool isBodyHtml = true)
    {
        try
        {
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(_senderEmail);
                message.To.Add(recipient);
                message.Subject = subject;
                message.IsBodyHtml = isBodyHtml;
                message.Body = body;

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = _smtpServer;
                    smtpClient.Port = _port;
                    smtpClient.Credentials = new NetworkCredential(_senderEmail, _appPassword);
                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(message);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error sending email: {ex.Message}");
        }
    }

    public async Task SendReserveEmailAsync(Dictionary<string, string> data)
    {
        string recipient = data["recipient"];
        data.Remove("recipient");
        string htmlBody = "<!DOCTYPE html>\n<html>\n<head>\n    <style>\n        body { font-family: Arial, sans-serif; }\n    </style>\n</head>\n<body>\n\n<h1>Hello {UserName}!</h1>\n<h3>This is to confirm your booking of {Media}</h3>\n\n<p>You can pick up your item(s) at {Time} from {Location}</p>\n\n</body>\n</html>";
            
        try
        {
            foreach (var item in data)
            {
                htmlBody = htmlBody.Replace($"{{{item.Key}}}", item.Value);
            }
            
            await SendEmailAsync(recipient, subject:"Reserving subject line" , htmlBody, true);
            
        }
        catch (Exception e)
        {
            throw new Exception($"Error sending reserve email: {e.Message}");
        }
    }
    
}