using BC = BCrypt.Net.BCrypt;


namespace Common.Utils;

public class PasswordService
{
    private const int WorkFactor = 12;  // Adjustable up to 31
    
    public string HashPassword(string password)
    {
        return BC.EnhancedHashPassword(password, WorkFactor);
    }
    
    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return BC.EnhancedVerify(providedPassword, hashedPassword);
    }
}