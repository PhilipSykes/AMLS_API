namespace Common.Constants
{
    public static class DbFieldNames
    {
        // Common fields
        public const string Id = "_id";
        public const string ObjectId = "objectId";
        
        public static class Login
        {
            public const string User = "user";
            public const string Email = "email";
            public const string PasswordHash = "password";
            public const string Role = "role";
            public const string Branches = "branches";
        }
        
        public static class Branch
        {
            public const string Name = "name";
            public const string Address = "address";
        }

        public static class Address
        {
            public const string Street = "address";
            public const string Town = "town";
            public const string Postcode = "postcode";
        }
        
        public static class MediaInfo
        {
            public const string Title = "title";
            public const string Publisher = "publisher";
            public const string Language = "language";
            public const string Description = "description";
            public const string Isbn = "isbn";
            public const string Author = "author";       
            public const string Rating = "rating";
            public const string ReleaseDate = "releaseDate";
            public const string Type = "type";
            public const string Genres = "genres";
            public const string PhysicalCopies = "physicalCopies";
            
            public const string Director = "director";   
            public const string Studio = "studio";       
            public const string Creator = "creator";    
            public const string Network = "network";    
            public const string Season = "season";     
            public const string Episodes = "episodes";  
            public const string EndDate = "endDate";    
        }
        public static class Users
        {
            public const string FirstName = "firstName";
            public const string LastName = "lastName";
            public const string DateOfBirth = "dateOfBirth";
            public const string Email = "email";
            public const string PhoneNumber = "phoneNumber";
            public const string Settings = "settings";
            public const string Favourites = "favourites";
            public const string History = "history";
            public const string NearestBranch = "nearestBranch";
        }
        
        public static class Staff
        {
            public const string FirstName = "firstName";
            public const string LastName = "lastName";
            public const string DateOfBirth = "dateOfBirth";
            public const string Email = "email";
            public const string PhoneNumber = "phoneNumber";
            public const string Branches = "branches";
        }

        public static class PhysicalCopies
        {
            public const string Branch = "branch";
            public const string Status = "status";
        }

        public static class Reservations
        {
            public const string Member = "member";
            public const string Item = "item";
            public const string StartDate = "startDate";
            public const string EndDate = "endDate";
        }
    }
}