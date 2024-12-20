namespace Common.Constants;

public static class MessageTypes
{
    public static class User
    {
        public const string Create = "user_create";
        public const string Update = "user_update";
        public const string Delete = "user_delete";
        public const string Signup = "user_signup";
        public const string GetProfile = "get_profile";
        public const string UpdateProfile = "update_profile";
        public const string DeleteProfile = "delete_profile";
        public const string ProcessFine = "process_fine";
        public const string GetPurchases = "get_purchases";
        public const string GetHistory = "get_history";
    }

    public static class EmailNotifications
    {
        public const string BorrowMedia = "email_borrow_media";
        public const string ReserveMedia = "email_reserve_media";
        public const string Login = "email_login";
        public const string PasswordReset = "email_password_reset";
        public const string ProfileUpdate = "email_profile_update";
    }

    public static class Subscription
    {
        public const string CancelSubscription = "cancel_subscription";
        public const string Subscribe = "user_subscribe";
        public const string GetSubscriptions = "get_subscriptions";
        public const string UpdateSubscription = "update_subscription";
    }

    public static class Media
    {
        public const string Borrow = "borrow_media";
        public const string Return = "return_media";
        public const string Extend = "extend_media";
        public const string CancelReservation = "cancel_reservation";
        public const string Search = "search_media";
        public const string AcceptReturn = "accept_return";
        public const string RegisterNew = "register_new";
    }

    public static class Favorites
    {
        public const string Set = "set_favorite";
        public const string Get = "get_favorites";
    }

    public static class Inventory
    {
        public const string GenerateManifest = "generate_manifest";
        public const string ConfirmManifest = "confirm_manifest";
        public const string RegisterNew = "register_inventory";
    }
}