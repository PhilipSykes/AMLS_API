using System.Net;
using System.Net.Mail;
using MongoDB.Bson;

namespace Common.Notification.Email;

public class EmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _port = 587;
    private readonly string _senderEmail = "hallam.amls@gmail.com";
    private readonly string _appPassword = "dktpdtqizcxwjerg";

    private async Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = true)
    {
        try
        {
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(_senderEmail);
                message.To.Add(toEmail);
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

    public async Task SendBorrowEmailAsync(string toEmail, Dictionary<string, string> data)
    {
        try
        {
            string htmlBody = await File.ReadAllTextAsync("Content/borrow-email.html");

            foreach (var item in data)
            {
                htmlBody = htmlBody.Replace($"{{{item.Key}}}", item.Value);
            }
            
            await SendEmailAsync(toEmail, subject:"Borrowing subject line" , htmlBody, true);
            
        }
        catch (Exception e)
        {
            throw new Exception($"Error sending borrow email: {e.Message}");
        }
    }
    
}