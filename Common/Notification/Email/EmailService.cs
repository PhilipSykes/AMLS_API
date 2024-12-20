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
    Task SendLoginEmailAsync(EmailDetails data);
    Task SendBorrowEmailAsync(EmailDetails data);
}


/// <summary>
/// Service for sending emails using SMTP
/// </summary>
public class EmailService : IEmailService
{
    private readonly string _appPassword = "app-password";
    private readonly int _port = 587;
    private readonly string _senderEmail = "hallam.amls@gmail.com";
    private readonly string _smtpServer = "smtp.gmail.com";
    
    private string ReplaceTemplateParameters(string template, Dictionary<string, string> parameters)
    {
        var result = template;
        foreach (var param in parameters)
        {
            result = result.Replace($"{{{param.Key}}}", param.Value);
        }
        return result;
    }
    /// <summary>
    /// Sends a reservation confirmation email using a template
    /// </summary>
    /// <param name="data">Email details containing recipient addresses and template parameters</param>
    /// <returns>A task representing the asynchronous email operation</returns>
    /// <exception cref="Exception">Thrown when email sending fails</exception>
    public async Task SendReserveEmailAsync(EmailDetails data)
    {
        try
        {
            var htmlBody = ReplaceTemplateParameters(EmailTemplates.Reservation, data.EmailBody);
            await SendEmailAsync(data.RecipientAddresses, "Reservation Confirmation", htmlBody);
        }
        catch (Exception e)
        {
            throw new Exception($"Error sending reserve email: {e.Message}");
        }
    }
    /// <summary>
    /// Sends a login confirmation email using a template
    /// </summary>
    /// <param name="data">Email details containing recipient addresses and template parameters</param>
    /// <returns>A task representing the asynchronous email operation</returns>
    /// <exception cref="Exception">Thrown when email sending fails</exception>
    public async Task SendLoginEmailAsync(EmailDetails data)
    {
        try
        {
            var htmlBody = ReplaceTemplateParameters(EmailTemplates.Login, data.EmailBody);
            await SendEmailAsync(data.RecipientAddresses, "New Login Detected", htmlBody);
        }
        catch (Exception e)
        {
            throw new Exception($"Error sending login email: {e.Message}");
        }
    }
    /// <summary>
    /// Sends a borrow confirmation email using a template
    /// </summary>
    /// <param name="data">Email details containing recipient addresses and template parameters</param>
    /// <returns>A task representing the asynchronous email operation</returns>
    /// <exception cref="Exception">Thrown when email sending fails</exception>
    public async Task SendBorrowEmailAsync(EmailDetails data)
    {
        try
        {
            var htmlBody = ReplaceTemplateParameters(EmailTemplates.Borrow, data.EmailBody);
            await SendEmailAsync(data.RecipientAddresses, "Borrow Confirmation", htmlBody);
        }
        catch (Exception e)
        {
            throw new Exception($"Error sending borrow email: {e.Message}");
        }
    }
    
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