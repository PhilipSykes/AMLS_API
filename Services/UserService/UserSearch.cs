using Common.Constants;
using Common.Database;
using Common.Utils;
using static Common.Models.Shared;
using static Common.Models.Entities;

namespace Services.UserService;

public interface IUserSearch
{
    Task<List<Users>> SearchUsers((int, int) pagination, List<Filter> filters);
    Task<List<Login>> GetLoginCredentials(List<Filter> filters);
    Task<List<Branch>> GetBranches(List<Filter> filters);
}

public class UserSearch : IUserSearch
{
    private readonly ISearchRepository _searchRepository;

    public UserSearch(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<List<Login>> GetLoginCredentials(List<Filter> filters)
    {
        var bsonDocuments = await _searchRepository.Search(DocumentTypes.Login, filters);

        List<Login> loginCredentials = Utils.ConvertBsonToEntity<Login>(bsonDocuments);
        
        Console.WriteLine($"Search completed. Found {loginCredentials.Count} results");
        return loginCredentials;
    }
    
    public async Task<List<Branch>> GetBranches(List<Filter> filters)
    {
        var bsonDocuments = await _searchRepository.Search(DocumentTypes.Branches, filters);

        List<Branch> branchList = Utils.ConvertBsonToEntity<Branch>(bsonDocuments);
        return branchList;
    }

    public async Task<List<Users>> SearchUsers((int, int) pagination, List<Filter> filters)
    {
        Console.WriteLine($"Performing user search with {filters.Count} filters");
        var bsonDocuments = await _searchRepository.Search(DocumentTypes.Users, pagination, filters);

        return Utils.ConvertBsonToEntity<Users>(bsonDocuments);
    }
}