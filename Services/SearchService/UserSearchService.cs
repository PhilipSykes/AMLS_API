using Common.Constants;
using Common.Database;
using Common.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Services.SearchService;

public interface IUserSearchService
{
    Task<Operations.Response<List<Entities.Users>>> SearchUsers((int, int) pagination, List<Filter> filters);
    Task<Operations.Response<List<Entities.Login>>> GetLoginCredentials(List<Filter> filters); 
}

public class UserSearchService : IUserSearchService
{
    private readonly ISearchRepository _searchRepository;

    public UserSearchService(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<Operations.Response<List<Entities.Login>>> GetLoginCredentials(List<Filter> filters)
    {
        List<BsonDocument> bsonDocuments = await _searchRepository.Search(DocumentTypes.Login, filters);
        
        List<Entities.Login> loginCredentials =  await _searchRepository.ConvertBsonToEntity<Entities.Login>(bsonDocuments);
        
        Console.WriteLine($"Search completed. Found {loginCredentials.Count} results");
        if (loginCredentials.Count == 0)
        {
            return new Operations.Response<List<Entities.Login>>
            {
                Success = false,
                Data = null,
                Error = "User Credentials do not match any existing users."
            };
        }
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
        
        List<Entities.Users> users =  await _searchRepository.ConvertBsonToEntity<Entities.Users>(bsonDocuments);

        Console.WriteLine($"Search completed. Found {users.Count} results");
        if (users.Count == 0)
        {
            return new Operations.Response<List<Entities.Users>>
            {
                Success = false,
                Data = null,
                Error = "No users found"
            };
        }
        return new Operations.Response<List<Entities.Users>>
        {
            Success = true,
            Data = users
        };
    }
    
}