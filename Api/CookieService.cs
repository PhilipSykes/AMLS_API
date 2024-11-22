namespace Common.Utils;

public class CookieService
{
    private const string CookieName = "user_id";
    
    public void SetUserCookie(HttpContext context, string userId)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddDays(7)
        };
        
        context.Response.Cookies.Append(CookieName, userId, cookieOptions);
    }
    
    public string? GetUserId(HttpContext context)
    {
        return context.Request.Cookies[CookieName];
    }
    
    public void RemoveCookie(HttpContext context)
    {
        context.Response.Cookies.Delete(CookieName);
    }
}