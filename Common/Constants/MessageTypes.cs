namespace Common.Constants;

public static class MessageTypes 
{
    public static class Notifications
    {
        public const string Login = "login";
        public const string Logout = "logout";
        public const string PasswordReset = "password_reset";
        public const string ProfileUpdate = "profile_update";
    }

    public static class User
    {
        public const string Create = "user_create";
        public const string Update = "user_update";
        public const string Delete = "user_delete";
    }
}