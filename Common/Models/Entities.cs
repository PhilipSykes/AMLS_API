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
            public string User { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Login.Email)]
            public string Email { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Login.PasswordHash)]
            public string PasswordHash { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Login.Role)]
            public string Role { get; init; } = string.Empty;
        }

        public record Branch
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectID { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Branch.Name)]
            public string Name { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Branch.Address)]
            public Address Address { get; init; } = new Address();
        }

        public record Address
        {
            [BsonElement(DbFieldNames.Address.FullAddress)]
            public string FullAddress { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Address.Street)]
            public string Street { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Address.Postcode)]
            public string Postcode { get; init; } = string.Empty;
        }

        public record MediaInfo
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectID { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Title)]
            public string Title { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Publisher)]
            public string Publisher { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Language)]
            public string Language { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Description)]
            public string Description { get; init; } = string.Empty;
            
            [BsonElement(DbFieldNames.MediaInfo.Isbn)]
            public string Isbn { get; init; } = string.Empty;
            
            [BsonElement(DbFieldNames.MediaInfo.Author)]
            public string Author { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Rating)]
            [BsonRepresentation(BsonType.Double)]
            public double Rating { get; init; } = 0.0;

            [BsonElement(DbFieldNames.MediaInfo.PublishDate)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime PublishDate { get; init; } = DateTime.UtcNow;

            [BsonElement(DbFieldNames.MediaInfo.Type)]
            public string Type { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.MediaInfo.Genres)]
            public string[] Genres { get; init; } = [];

            //[BsonElement(DBFieldNames.MediaInfo.FormatSpecificDetails)]
            //public Dictionary<string, string> FormatSpecificDetails { get; init; } = new Dictionary<string, string>();

            [BsonElement(DbFieldNames.MediaInfo.PhysicalCopies)]
            public List<PhysicalCopy> PhysicalCopies { get; init; } = new();
        }

        public record PhysicalCopy
        {
            [BsonId]
            [BsonElement("branch")]
            public ObjectId branch { get; init; } = ObjectId.Empty;
            
            [BsonElement("status")]
            public string Status { get; init; } = "Unknown";
        }
        public record Users
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectID { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Users.FirstName)]
            public string FirstName { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Users.LastName)]
            public string LastName { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Users.DateOfBirth)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime DateOfBirth { get; init; } = DateTime.UtcNow;

            [BsonElement(DbFieldNames.Users.Email)]
            public string Email { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Users.PhoneNumber)]
            public string PhoneNumber { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Users.Settings)]
            public Dictionary<string, string> Settings { get; init; } = new Dictionary<string, string>();

            [BsonElement(DbFieldNames.Users.Favourites)]
            public string[] Favourites { get; init; } = [];

            [BsonElement(DbFieldNames.Users.History)]
            public string[] History { get; init; } = [];

            [BsonElement(DbFieldNames.Users.NearestBranch)]
            public string NearestBranch { get; init; } = string.Empty;
        }

        public record Reservations
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectID { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Reservations.Member)]
            public string Member { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Reservations.Media)]
            public string Media { get; init; } = string.Empty;

            [BsonElement(DbFieldNames.Reservations.StartDate)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime StartDate { get; init; } = DateTime.UtcNow;

            [BsonElement(DbFieldNames.Reservations.EndDate)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime EndDate { get; init; } = DateTime.UtcNow;
        }
    }
}