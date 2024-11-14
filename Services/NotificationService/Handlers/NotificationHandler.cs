namespace Services.NotificationService.Handlers;

public class NotificationHandler
{
    public static async Task HandleNotification(string type, string message)
    {
        switch (type)
        {
            case "login":
                Console.WriteLine($"New login: {message}");
                break;
            case "logout":
                Console.WriteLine($"User logout: {message}");
                break;
            case "password_reset":
                Console.WriteLine($"Password reset requested: {message}");
                break;
            case "profile_update":
                Console.WriteLine($"Profile updated: {message}");
                break;
        }
        await Task.CompletedTask;
    }
}