// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using MongoDB;
// using MongoDB.Driver;
// using MongoDB.Bson;
// using MongoDB.Driver.Core.Events;
// using Common.Database;
// using System.Net;
// using System.Xml.Linq;
//
//
// //Test Code
// /*
// SearchRepository search = new SearchRepository("c1023778:X4M8yMPq6DNgrOck");
//
// List<Filter> filters = new List<Filter>();
// filters.Add(new Filter("title", "hunger", '~'));
// List<BsonDocument> test = await search.SearchMediaInfo(filters);
// PrintResults(test);
// */
//
// // void PrintResults(List<BsonDocument> list)
// // {
// //     foreach (BsonDocument doc in list)
// //     {
// //         Console.WriteLine(doc);
// //     }
// // }
// //TODO, write functions for these tests, so i don't have to keep re-writing them
//
//
//
// namespace Common.Database
// {
//
//
//
//     public interface IDatabaseConnection
//     {
//         public Task<bool> HealthCheckAsync();
//         IMongoDatabase Database { get; }
//     }
//
//     // Class to handle database connectivity
//     public class DatabaseConnection : IDatabaseConnection
//     {
//
//
//         private MongoClient connection;
//
//         public IMongoDatabase Database { get; }
//
//         public DatabaseConnection(string authorisation)
//         {
//             string settings =
//                 "retryWrites=true&w=majority&appName=Cluster0"; //Probably should find a better solution for this
//             string connectionString = $"mongodb+srv://{authorisation}@cluster0.simvp.mongodb.net/?{settings}";
//             connection = new MongoClient(connectionString);
//             Database = connection.GetDatabase("AdvancedMediaLibrary");
//         }
//
//
//         // From Claude AI, just in case i need to reference this
//         public async Task<bool> HealthCheckAsync()
//         {
//             try
//             {
//                 await Database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
//                 return true;
//             }
//             catch (Exception)
//             {
//                 return false;
//             }
//         }
//     }
//
//
//
//
//     public class BaseRepository
//     {
//         protected IDatabaseConnection database;
//         protected IFilterBuilder<BsonDocument> filterBuilder = new BsonFilterBuilder();
//
//         public BaseRepository(string authorisation)
//         {
//             // Look at this later
//             // https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/connection/connection-options/
//
//             database = new DatabaseConnection(authorisation);
//
//         }
//
//         // Oh god its hideous, future me will hate this.
//         public async Task<List<BsonDocument>> Search(IMongoCollection<BsonDocument> collection, List<Filter> filterO)
//         {
//
//             return await collection.Find(filterBuilder.BuildFilter(filterO)).ToListAsync();
//         }
//     }
//
//
//
//
//     public interface ISearchRepository
//     {
//         Task<List<BsonDocument>> SearchMediaInfo(List<Filter> filters);
//         Task<List<BsonDocument>> SearchPhysicalMedia(List<Filter> filters);
//     }
//
//     // Repository of database functions for the Search Service
//     public class SearchRepository : BaseRepository, ISearchRepository
//     {
//         public SearchRepository(string authorisation) : base(authorisation)
//         {
//
//         }
//
//         public async Task<List<BsonDocument>> SearchMediaInfo(List<Filter> filters)
//         {
//             //TODO: database.Database is stupid, and needs fixing
//             var collection = database.Database.GetCollection<BsonDocument>("MediaInfo");
//             return await Search(collection, filters);
//         }
//
//         public async Task<List<BsonDocument>> SearchPhysicalMedia(List<Filter> filters = null)
//         {
//             var collection = database.Database.GetCollection<BsonDocument>("PhysicalMedia");
//             return await Search(collection, filters);
//         }
//
//     }
//
//
//
//     // Interface means another Filter builder could be created for different formats later, e.g. if database software changes/expands
//     public interface IFilterBuilder<T>
//     {
//         // Converts a filter from the UI in dictionary format, to the format used by mongo
//         public FilterDefinition<T> BuildFilter(List<Filter> filtersIn);
//     }
//
//     // Class for converting filter formats between filter objects from the UI, to mongo Bson format
//     public class BsonFilterBuilder : IFilterBuilder<BsonDocument>
//     {
//         enum Operations
//         {
//             // Possible this should be moved so it can be distributed to services/UI? makes making them a bit easier
//             Equals = '=',
//             NotEquals = '!',
//             GreaterThan = '>',
//             LessThan = '<',
//             Contains = '~'
//         }
//
//         public FilterDefinition<BsonDocument> BuildFilter(List<Filter> filtersIn)
//         {
//             var builder = Builders<BsonDocument>.Filter; // Query builder from Mongo
//             var filters = new List<FilterDefinition<BsonDocument>>(); // List of individual filters
//
//
//             foreach (var filterIn in filtersIn)
//             {
//                 //TODO, figure out how i can search arrays/nested fields
//                 switch (filterIn.operation)
//                 {
//                     // Note: There are many more operations than this, but i think this is all we need
//                     case (char)Operations.Equals:
//                         filters.Add(builder.Eq(filterIn.key, filterIn.value));
//                         break;
//                     case (char)Operations.GreaterThan:
//                         filters.Add(builder.Gt(filterIn.key, filterIn.value));
//                         break;
//                     case (char)Operations.LessThan:
//                         filters.Add(builder.Lt(filterIn.key, filterIn.value));
//                         break;
//                     case (char)Operations.NotEquals:
//                         filters.Add(builder.Not(builder.Eq(filterIn.key,
//                             filterIn.value))); // This line might not be right, test later
//                         break;
//                     case (char)Operations.Contains:
//                         filters.Add(
//                             builder.Regex(filterIn.key,
//                                 new BsonRegularExpression($".*{filterIn.value}.*",
//                                     "i"))); // TODO, Check if it is string, also look into regex i general
//                         break;
//                 }
//             }
//
//             // Merge filters and return
//             return filters.Count > 0 ? builder.And(filters) : builder.Empty;
//         }
//     }
//
//
//
//     // Filter class for transporting filter settings from the UI to the database queries
//     public class Filter
//     {
//         public string key { get; set; }
//         public object value { get; set; }
//         public char operation { get; set; }
//
//     }
// }
//
