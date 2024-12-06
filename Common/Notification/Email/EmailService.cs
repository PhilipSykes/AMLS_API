using System.Net;
using System.Net.Mail;
using static Common.Models.Shared;

namespace Common.Notification.Email;

/// <summary>
/// Interface for email notification services
/// </summary>
public interface IEmailService
{
    Task SendReserveEmailAsync(EmailDetails data);
}


/// <summary>
/// Service for sending emails using SMTP
/// </summary>
public class EmailService : IEmailService
{
    private readonly string _appPassword = "dktpdtqizcxwjerg";
    private readonly int _port = 587;
    private readonly string _senderEmail = "hallam.amls@gmail.com";
    private readonly string _smtpServer = "smtp.gmail.com";

    /// <summary>
    /// Sends a reservation confirmation email using a template
    /// </summary>
    /// <param name="data">Email details containing recipient addresses and template parameters</param>
    /// <returns>A task representing the asynchronous email operation</returns>
    /// <exception cref="Exception">Thrown when email sending fails</exception>
    public async Task SendReserveEmailAsync(EmailDetails data)
    {
        var htmlBody =
            "<!DOCTYPE html>\n<html>\n<head>\n    <style>\n        body { font-family: Arial, sans-serif; }\n    </style>\n</head>\n<body>\n\n<h1>Hello {UserName}!</h1>\n<h3>This is to confirm your booking of {Media}</h3>\n\n<p>You can pick up your item(s) at {Time} from {Location}</p>\n\n</body>\n</html>";

        try
        {
            foreach (var item in data.EmailBody) htmlBody = htmlBody.Replace($"{{{item.Key}}}", item.Value);

            await SendEmailAsync(data.RecipientAddresses, "Reserving subject line", htmlBody);
        }
        catch (Exception e)
        {
            throw new Exception($"Error sending reserve email: {e.Message}");
        }
    }

    //TODO: Remove inline HTML for E-mail Content

    /// <summary>
    /// Sends an email to multiple recipients using SMTP
    /// </summary>
    /// <param name="recipients">List of recipient email addresses</param>
    /// <param name="subject">Email subject line</param>
    /// <param name="body">Email content body</param>
    /// <param name="isBodyHtml">Indicates if the body contains HTML content</param>
    /// <returns>A task representing the asynchronous email operation</returns>
    /// <exception cref="Exception">Thrown when email sending fails</exception>
    private async Task SendEmailAsync(List<string> recipients, string subject, string body, bool isBodyHtml = true)
    {
        try
        {
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(_senderEmail);
                foreach (var recipient in recipients) message.To.Add(recipient);

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
}