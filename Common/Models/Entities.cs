using Common.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Models
{
   public static class Entities
    {
        public record Login
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectID { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Login.User)]
            public string UserID { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Login.Email)]
            public string Email { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Login.PasswordHash)]
            public string PasswordHash { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Login.Role)]
            public string Role { get; init; } = string.Empty;
            
            [BsonElement(DbFieldNames.Login.Branches)]
            public string[] Branches { get; init; } = [];
        }

        public record Branch
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectId { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Branch.Name)]
            public string Name { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Branch.Address)]
            public Address Address { get; init; } = new Address();
        }

        public record Address
        {
            [BsonElement(DbFieldNames.Address.Street)]
            public string Street { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Address.Town)]
            public string Town { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Address.Postcode)]
            public string Postcode { get; init; } = string.Empty;
        }

        public record MediaInfo
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectId { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Title)]
            public string Title { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Language)]
            public string Language { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Description)]
            public string Description { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Rating)]
            [BsonRepresentation(BsonType.Double)]
            public double Rating { get; init; } = 0.0;

            [BsonElement(DbFieldNames.MediaInfo.ReleaseDate)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime ReleaseDate { get; init; } = DateTime.UtcNow;

            [BsonElement(DbFieldNames.MediaInfo.Type)]
            public string Type { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Genres)]
            public string[] Genres { get; init; } = [];

            //Book attributes
            [BsonElement(DbFieldNames.MediaInfo.Isbn)]
            public string Isbn { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Author)]
            public string Author { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Publisher)]
            public string Publisher { get; init; } = string.Empty;

            //Film attributes
            [BsonElement(DbFieldNames.MediaInfo.Director)]
            public string Director { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Studio)]
            public string Studio { get; init; } = string.Empty;

            //TV series attributes
            [BsonElement(DbFieldNames.MediaInfo.Creator)]
            public string Creator { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Network)]
            public string Network { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Season)]
            [BsonRepresentation(BsonType.Int32)]
            public int Season { get; init; } = 0;

            [BsonElement(DbFieldNames.MediaInfo.Episodes)]
            [BsonRepresentation(BsonType.Int32)]
            public int Episodes { get; init; } = 0;

            [BsonElement(DbFieldNames.MediaInfo.EndDate)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime? EndDate { get; init; } = null;

            //Aggregates
            [BsonElement(DbFieldNames.MediaInfo.PhysicalCopies)]
            public List<PhysicalCopy>? PhysicalCopies { get; init; } = new();
            
        }

        public record PhysicalCopy
        {
            [BsonElement(DbFieldNames.PhysicalMedia.Branch)]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Branch { get; init; } = string.Empty;
            
            [BsonElement(DbFieldNames.PhysicalMedia.Status)]
            public string Status { get; init; } = "Unknown";
        }

        public record PhysicalMedia
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; init; } = string.Empty;
            
            [BsonElement(DbFieldNames.PhysicalMedia.Info)]
            [BsonRepresentation(BsonType.ObjectId)]
            public string InfoRef = string.Empty;
            
            [BsonElement(DbFieldNames.PhysicalMedia.Branch)]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Location { get; init; } = string.Empty;
            
            [BsonElement(DbFieldNames.PhysicalMedia.Status)]
            public string Status = "Unknown";
        }

        public record Members
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectId { get; init; }

            [BsonElement(DbFieldNames.Members.FirstName)]
            public string FirstName { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Members.LastName)]
            public string LastName { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Members.DateOfBirth)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime DateOfBirth { get; init; } = DateTime.UtcNow;

            [BsonElement(DbFieldNames.Members.Email)]
            public string Email { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Members.PhoneNumber)]
            public string PhoneNumber { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Members.Settings)]
            public List<Setting> Settings { get; init; } = new();

            [BsonElement(DbFieldNames.Members.Favourites)]
            public string[] Favourites { get; init; } = [];

            [BsonElement(DbFieldNames.Members.History)]
            public string[] History { get; init; } = [];

            [BsonElement(DbFieldNames.Members.NearestBranch)]
            public string NearestBranch { get; init; } = string.Empty;
        }
        
        public class Setting
        {
            [BsonElement(DbFieldNames.Settings.Name)]
            public string Name { get; init; } = string.Empty;
    
            [BsonElement(DbFieldNames.Settings.Value)]
            public string Value { get; init; } = string.Empty;
        }
        
        public record Staff
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectId { get; init; } = string.Empty;
            
            [BsonElement(DbFieldNames.Staff.FirstName)]
            public string FirstName { get; init; } = string.Empty;
            
            [BsonElement(DbFieldNames.Staff.LastName)]
            public string LastName { get; init; } = string.Empty;
            
            [BsonElement(DbFieldNames.Staff.DateOfBirth)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime DateOfBirth { get; init; } = DateTime.UtcNow;
            
            [BsonElement(DbFieldNames.Staff.Role)]
            public string Role { get; init; } = string.Empty;
            
            [BsonElement(DbFieldNames.Staff.Email)]
            public string Email { get; init; } = string.Empty;
            
            [BsonElement(DbFieldNames.Staff.PhoneNumber)]
            public string PhoneNumber { get; init; } = string.Empty;
            
            [BsonElement(DbFieldNames.Staff.Branches)]
            public string[] Branches { get; init; } = [];
        }

        public record Reservation
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectId { get; init; }

            [BsonRepresentation(BsonType.ObjectId)]
            [BsonElement(DbFieldNames.Reservations.Member)]
            public string Member { get; init; }

            [BsonRepresentation(BsonType.ObjectId)]
            [BsonElement(DbFieldNames.Reservations.Item)]
            public string Item { get; init; }

            [BsonElement(DbFieldNames.Reservations.StartDate)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime StartDate { get; init; } = DateTime.UtcNow;

            [BsonElement(DbFieldNames.Reservations.EndDate)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime EndDate { get; init; }
        }
        
        
        public record ReservableItem
        {
            public string Item { get; init; } = string.Empty;
            public string BranchName { get; init; } = string.Empty;
            public List<Timeslot> Timeslots { get; init; } = new();
            public DateTime LastEnd { get; init; }
        }

        public class Timeslot
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }

            public Timeslot(DateTime start, DateTime end)
            {
                Start = start;
                End = end;
            }
        }

        //Aggregates
        public record PhysicalInventory
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectId { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.PhysicalMedia.Status)]
            public string Status { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.PhysicalMedia.Branch)]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Branch { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Aggregates.MediaInfo)]
            public MediaInfo MediaInfo { get; init; } = new();

            [BsonElement(DbFieldNames.Aggregates.BranchDetails)]
            public Branch BranchDetails { get; init; } = new();

            [BsonElement(DbFieldNames.Aggregates.Reservations)]
            public List<Reservation> Reservations { get; init; } = new();
        }
    }
}