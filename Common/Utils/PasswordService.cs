using BC = BCrypt.Net.BCrypt;


namespace Common.Utils;

public static class PasswordService
{
    private const int WorkFactor = 12; // Adjustable up to 31

    public static string HashPassword(string password)
    {
        return BC.EnhancedHashPassword(password, WorkFactor);
    }

    public static bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return BC.EnhancedVerify(providedPassword, hashedPassword);
    }
}