using Microsoft.Extensions.Options;
using Common.Configuration;
using Common.Constants;
using Common;
using Common.Models;
using Common.Notification.Email;

namespace Services.NotificationService;
public class NotificationMessageReceiver : BaseMessageReceiver<EmailDetails>
{
    private readonly IEmailService _emailService;
    private static readonly string[] NotificationTypes =
    [
        MessageTypes.EmailNotifications.Login,
        MessageTypes.EmailNotifications.PasswordReset,
        MessageTypes.EmailNotifications.ProfileUpdate,
        MessageTypes.EmailNotifications.BorrowMedia,
        MessageTypes.EmailNotifications.ReserveMedia,
    ];

    public NotificationMessageReceiver(IOptions<RabbitMQConfig> options, IEmailService emailService) 
        : base(options, NotificationTypes)
    {
        _emailService = emailService;
    }

    protected override async Task HandleMessage(string messageType, EmailDetails data)
    {
        switch (messageType)
        {
            case MessageTypes.EmailNotifications.Login:
                //TODO create new login email 
                break;
            case MessageTypes.EmailNotifications.PasswordReset:
                //TODO create password reset email 
                break;
            case MessageTypes.EmailNotifications.ProfileUpdate:
                //TODO create profile update email 
                break;
            case MessageTypes.EmailNotifications.BorrowMedia:
                //TODO create borrow media email 
                break;
            case MessageTypes.EmailNotifications.ReserveMedia:
                await _emailService.SendReserveEmailAsync(data);
                break;
            
        }
        await Task.CompletedTask;
    }
}