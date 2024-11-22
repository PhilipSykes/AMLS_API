using Common.Constants;
using Common.Database;
using Common.Models;
using Common.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Services.UserService;

public interface IUserSearch
{
    Task<Operations.Response<List<Entities.Users>>> SearchUsers((int, int) pagination, List<Filter> filters);
    Task<Operations.Response<List<Entities.Login>>> GetLoginCredentials(List<Filter> filters); 
}

public class UserSearch : IUserSearch
{
    private readonly ISearchRepository _searchRepository;

    public UserSearch(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<Operations.Response<List<Entities.Login>>> GetLoginCredentials(List<Filter> filters)
    {
        List<BsonDocument> bsonDocuments = await _searchRepository.Search(DocumentTypes.Login, filters);
        
        List<Entities.Login> loginCredentials =  Utils.ConvertBsonToEntity<Entities.Login>(bsonDocuments);
        
        Console.WriteLine($"Search completed. Found {loginCredentials.Count} results");
        return new Operations.Response<List<Entities.Login>>
        {
            Success = true,
            Data = loginCredentials
        };
    }

    public async Task<Operations.Response<List<Entities.Users>>> SearchUsers((int, int) pagination, List<Filter> filters)
    {
        Console.WriteLine($"Performing user search with {filters.Count} filters");
        List<BsonDocument> bsonDocuments = await _searchRepository.Search(DocumentTypes.Users, pagination, filters);
        
        List<Entities.Users> users = Utils.ConvertBsonToEntity<Entities.Users>(bsonDocuments);

        Console.WriteLine($"Search completed. Found {users.Count} results");
        return new Operations.Response<List<Entities.Users>>
        {
            Success = true,
            Data = users
        };
    }
    
}