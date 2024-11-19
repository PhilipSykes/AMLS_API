using Microsoft.Extensions.Options;
using Common.Configuration;
using Common.Constants;
using Common;
using Services.NotificationService.Handlers;

namespace Services.NotificationService;
public class NotificationMessageReceiver : BaseMessageReceiver
{
    private static readonly string[] NotificationTypes =
    [
        MessageTypes.EmailNotifications.Login,
        MessageTypes.EmailNotifications.PasswordReset,
        MessageTypes.EmailNotifications.ProfileUpdate,
        MessageTypes.EmailNotifications.BorrowMedia,
        MessageTypes.EmailNotifications.ReserveMedia,
    ];

    public NotificationMessageReceiver(IOptions<RabbitMQConfig> options) 
        : base(options, NotificationTypes)
    {
    }

    protected override async Task HandleMessage(string messageType, string message)
    {
        await NotificationHandler.HandleNotification(messageType, message);
    }
}