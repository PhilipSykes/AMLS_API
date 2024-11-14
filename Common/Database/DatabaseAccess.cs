﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Core.Events;
using Common.Database;
using System.Net;
using System.Xml.Linq;

//Test Code
SearchRepository search = new SearchRepository("c1023778:X4M8yMPq6DNgrOck");

List<Filter> filters = new List<Filter>();
Filter filter1 = new Filter("title", "The Hunger Games", '='); // Example filter
filters.Add(filter1);
List<BsonDocument> test = await search.SearchMediaInfo(filters);


foreach (BsonDocument testDoc in test)
{
    Console.WriteLine(testDoc);
}

namespace Common.Database
{



    public interface IDatabaseConnection
    {
        public Task<bool> HealthCheckAsync();
        IMongoDatabase Database { get; }
    }

    // Class to handle database connectivity
    public class DatabaseConnection : IDatabaseConnection
    {


        private MongoClient connection;

        public IMongoDatabase Database { get; }

        public DatabaseConnection(string authorisation)
        {
            string settings = "retryWrites=true&w=majority&appName=Cluster0"; //Probably should find a better solution for this
            string connectionString = $"mongodb+srv://{authorisation}@cluster0.simvp.mongodb.net/?{settings}";
            connection = new MongoClient(connectionString);
            Database = connection.GetDatabase("AdvancedMediaLibrary");
        }


        // From Claude AI, just in case i need to reference this
        public async Task<bool> HealthCheckAsync()
        {
            try
            {
                await Database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }




    public class BaseRepository
    {
        protected IDatabaseConnection database;
        protected IFilterBuilder<BsonDocument> filterBuilder = new BsonFilterBuilder();
        public BaseRepository(string authorisation)
        {
            // Look at this later
            // https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/connection/connection-options/

            database = new DatabaseConnection(authorisation);

        }

        // Oh god its hideous, future me will hate this.
        public async Task<List<BsonDocument>> Search(IMongoCollection<BsonDocument> collection, List<Filter> filterO)
        {
            return await collection.Find(filterBuilder.BuildFilter(filterO)).ToListAsync();
        }
    }




    public interface ISearchRepository
    {
        Task<List<BsonDocument>> SearchMediaInfo(List<Filter> filters);
        Task<List<BsonDocument>> SearchPhysicalMedia(List<Filter> filters);
    }

    // Repository of database functions for the Search Service
    public class SearchRepository : BaseRepository, ISearchRepository
    {
        public SearchRepository(string authorisation) : base(authorisation)
        {

        }

        public async Task<List<BsonDocument>> SearchMediaInfo(List<Filter> filters)
        {
            //TODO: database.Database is stupid, and needs fixing
            var collection = database.Database.GetCollection<BsonDocument>("MediaInfo");
            return await Search(collection, filters);
        }

        public async Task<List<BsonDocument>> SearchPhysicalMedia(List<Filter> filters)
        {
            var collection = database.Database.GetCollection<BsonDocument>("PhysicalMedia");
            return await Search(collection, filters);
        }

    }



    // Interface means another Filter builder could be created for different formats later, e.g. if database software changes/expands
    public interface IFilterBuilder<T>
    {
        // Converts a filter from the UI in dictionary format, to the format used by mongo
        public FilterDefinition<T> BuildFilter(List<Filter> filtersIn);
    }

    // Class for converting filter formats between filter objects from the UI, to mongo Bson format
    public class BsonFilterBuilder : IFilterBuilder<BsonDocument>
    {

        public FilterDefinition<BsonDocument> BuildFilter(List<Filter> filtersIn)
        {
            var builder = Builders<BsonDocument>.Filter; // Query builder from Mongo
            var filters = new List<FilterDefinition<BsonDocument>>(); // List of individual filters


            foreach (var filterIn in filtersIn)
            {
                switch (filterIn.operation)
                { // Note: There are many more operations than this, but i think this is all we need
                    case '=': // Equals
                        filters.Add(builder.Eq(filterIn.key, filterIn.value));
                        break;
                    case '>': // Greater than
                        filters.Add(builder.Gt(filterIn.key, filterIn.value));
                        break;
                    case '<': // Less than
                        filters.Add(builder.Lt(filterIn.key, filterIn.value));
                        break;
                    case '!': // Not equal to
                        filters.Add(builder.Not(builder.Eq(filterIn.key, filterIn.value))); // This line might not be right, test later
                        break;
                }
            }

            // Merge filters and return
            return filters.Count > 0 ? builder.And(filters) : builder.Empty;
        }
    }



    // Filter class for transporting filter settings from the UI to the database queries
    public class Filter(string key, object value, char operation)
    {
        public string key = key;
        public object value = value;
        public char operation = operation;
    }
}
