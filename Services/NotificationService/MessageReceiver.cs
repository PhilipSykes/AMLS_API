using Microsoft.Extensions.Options;
using Common.Constants;
using Common;
using Services.NotificationService.Handlers;

namespace Services.NotificationService;
public class NotificationMessageReceiver : BaseMessageReceiver
{
    private static readonly string[] NotificationTypes =
    [
        MessageTypes.Notifications.Login,
        MessageTypes.Notifications.Logout,
        MessageTypes.Notifications.PasswordReset,
        MessageTypes.Notifications.ProfileUpdate
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