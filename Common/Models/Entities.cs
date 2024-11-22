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

            [BsonElement(DBFieldNames.Login.User)]
            public string User { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Login.Email)]
            public string Email { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Login.PasswordHash)]
            public string PasswordHash { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Login.Role)]
            public string Role { get; init; } = string.Empty;
        }

        public record Branch
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectID { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Branch.Name)]
            public string Name { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Branch.Address)]
            public Address Address { get; init; } = new Address();
        }

        public record Address
        {
            [BsonElement(DBFieldNames.Address.FullAddress)]
            public string FullAddress { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Address.Street)]
            public string Street { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Address.Postcode)]
            public string Postcode { get; init; } = string.Empty;
        }

        public record MediaInfo
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectID { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.MediaInfo.Title)]
            public string Title { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.MediaInfo.Publisher)]
            public string Publisher { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.MediaInfo.Language)]
            public string Language { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.MediaInfo.Description)]
            public string Description { get; init; } = string.Empty;
            
            [BsonElement(DBFieldNames.MediaInfo.Isbn)]
            public string Isbn { get; init; } = string.Empty;
            
            [BsonElement(DBFieldNames.MediaInfo.Author)]
            public string Author { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.MediaInfo.Rating)]
            [BsonRepresentation(BsonType.Double)]
            public double Rating { get; init; } = 0.0;

            [BsonElement(DBFieldNames.MediaInfo.PublishDate)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime PublishDate { get; init; } = DateTime.UtcNow;

            [BsonElement(DBFieldNames.MediaInfo.Type)]
            public string Type { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.MediaInfo.Genres)]
            public string[] Genres { get; init; } = [];

            [BsonElement(DBFieldNames.MediaInfo.FormatSpecificDetails)]
            public Dictionary<string, string> FormatSpecificDetails { get; init; } = new Dictionary<string, string>();
        }

        public record Users
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectID { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Users.FirstName)]
            public string FirstName { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Users.LastName)]
            public string LastName { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Users.DateOfBirth)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime DateOfBirth { get; init; } = DateTime.UtcNow;

            [BsonElement(DBFieldNames.Users.Email)]
            public string Email { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Users.PhoneNumber)]
            public string PhoneNumber { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Users.Settings)]
            public Dictionary<string, string> Settings { get; init; } = new Dictionary<string, string>();

            [BsonElement(DBFieldNames.Users.Favourites)]
            public string[] Favourites { get; init; } = [];

            [BsonElement(DBFieldNames.Users.History)]
            public string[] History { get; init; } = [];

            [BsonElement(DBFieldNames.Users.NearestBranch)]
            public string NearestBranch { get; init; } = string.Empty;
        }

        public record Reservations
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string ObjectID { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Reservations.Member)]
            public string Member { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Reservations.Media)]
            public string Media { get; init; } = string.Empty;

            [BsonElement(DBFieldNames.Reservations.StartDate)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime StartDate { get; init; } = DateTime.UtcNow;

            [BsonElement(DBFieldNames.Reservations.EndDate)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime EndDate { get; init; } = DateTime.UtcNow;
        }
    }
}