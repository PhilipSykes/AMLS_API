using Microsoft.Extensions.Options;
using Common;
using Common.Constants;
using Common.Configuration;

namespace Services.UserService;

public class UserServiceMessageReceiver : BaseMessageReceiver
{
    private static readonly string[] UserMessageTypes =
    [
        MessageTypes.User.Create,
        MessageTypes.User.Update,
        MessageTypes.User.Delete,
        MessageTypes.User.Signup,
        MessageTypes.Notifications.PasswordReset,
        MessageTypes.Notifications.Login,
    ];

    public UserServiceMessageReceiver(IOptions<RabbitMQConfig> options)
        : base(options, UserMessageTypes)
    {
    }

    protected override async Task HandleMessage(string messageType, string message)
    {
        switch (messageType)
        {
            case MessageTypes.User.Create:
                //await HandleUserCreate(message);
                break;
            // ... other cases
        }
    }
}