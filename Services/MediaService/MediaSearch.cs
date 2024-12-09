using Common.Constants;
using Common.Database;
using Common.Utils;
using static Common.Models.Shared;
using static Common.Models.Entities;

namespace Services.MediaService;

public interface IMediaSearch
{
    Task<List<MediaInfo>> SearchMedia((int, int) pagination, List<Filter> filters);
}

public class MediaSearch : IMediaSearch
{
    private readonly ISearchRepository _searchRepository;

    public MediaSearch(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<List<MediaInfo>> SearchMedia((int, int) pagination,
        List<Filter> filters)
    {
        //Console.WriteLine($"Performing media search with {filters.Count} filters");
        var result = await _searchRepository.PaginatedSearch(DocumentTypes.MediaInfo, pagination, filters);

        return Utils.ConvertBsonToEntity<MediaInfo>(result.Data);
    }
}