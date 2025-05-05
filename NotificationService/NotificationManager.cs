using Microsoft.Extensions.Options;
using Common.Constants;
using Common;
using Common.MessageBroker;
using static Common.Models.Shared;
using Common.Notification.Email;

namespace NotificationService;
public class NotificationManager : BaseMessageReceiver<EmailDetails>
{
    private readonly IEmailService _emailService;
    private readonly Exchange _exchange;
    private static readonly string[] NotificationTypes =
    [
        MessageTypes.EmailNotifications.Login,
        MessageTypes.EmailNotifications.BorrowMedia,
        MessageTypes.EmailNotifications.ReserveMedia,
        MessageTypes.EmailNotifications.TwoFactorCode,
    ];

    public NotificationManager(IOptions<RabbitMQConfig> options, IEmailService emailService,Exchange exchange) 
        : base(options, NotificationTypes)
    {
        _emailService = emailService;
        _exchange = exchange;
    }

    protected override async Task HandleMessage(string messageType, EmailDetails data)
    {
        switch (messageType)
        {
            case MessageTypes.EmailNotifications.Login:
                await _emailService.SendLoginEmailAsync(data); 
                break;
            case MessageTypes.EmailNotifications.BorrowMedia:
                await _emailService.SendBorrowEmailAsync(data); 
                break;
            case MessageTypes.EmailNotifications.ReserveMedia:
                await _emailService.SendReserveEmailAsync(data);
                break;
            case MessageTypes.EmailNotifications.TwoFactorCode:
                await _emailService.SendTwoFactorCodeEmailAsync(data);
                break;
        }
        await Task.CompletedTask;
    }
}