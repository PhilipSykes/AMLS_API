using Common.Constants;

namespace Services.NotificationService.Handlers;

public class NotificationHandler
{
    public static async Task HandleNotification(string type, string message)
    {
        switch (type)
        {
            case MessageTypes.EmailNotifications.Login:
                Console.WriteLine($"New login: {message}");
                break;
            case MessageTypes.EmailNotifications.PasswordReset:
                Console.WriteLine($"Password reset requested: {message}");
                break;
            case MessageTypes.EmailNotifications.ProfileUpdate:
                Console.WriteLine($"Profile updated: {message}");
                break;
            case MessageTypes.EmailNotifications.BorrowMedia:
                Console.WriteLine($"Profile updated: {message}");
                break;
            case MessageTypes.EmailNotifications.ReserveMedia:
                Console.WriteLine($"Profile updated: {message}");
                break;
            
        }
        await Task.CompletedTask;
    }
}