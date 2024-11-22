using Common.Constants;
using Common.Database;
using Common.Models;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Common.Utils;
using MongoDB.Bson;

namespace Services.UserService;

public interface IUserSearch
{
    Task<Response<List<Users>>> SearchUsers((int, int) pagination, List<Filter> filters);
    Task<Response<List<Login>>> GetLoginCredentials(List<Filter> filters); 
}

public class UserSearch : IUserSearch
{
    private readonly ISearchRepository _searchRepository;

    public UserSearch(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<Response<List<Login>>> GetLoginCredentials(List<Filter> filters)
    {
        List<BsonDocument> bsonDocuments = await _searchRepository.Search(DocumentTypes.Login, filters);
        
        List<Login> loginCredentials =  Utils.ConvertBsonToEntity<Login>(bsonDocuments);
        
        Console.WriteLine($"Search completed. Found {loginCredentials.Count} results");
        return new Response<List<Login>>
        {
            Success = true,
            Data = loginCredentials
        };
    }

    public async Task<Response<List<Users>>> SearchUsers((int, int) pagination, List<Filter> filters)
    {
        Console.WriteLine($"Performing user search with {filters.Count} filters");
        List<BsonDocument> bsonDocuments = await _searchRepository.Search(DocumentTypes.Users, pagination, filters);
        
        List<Users> users = Utils.ConvertBsonToEntity<Users>(bsonDocuments);

        Console.WriteLine($"Search completed. Found {users.Count} results");
        return new Response<List<Users>>
        {
            Success = true,
            Data = users
        };
    }
    
}