using Common.Models;

namespace Common.Database.Interfaces;

public interface ISearchRepository
{
    Task<SearchResponse> SearchMediaInfo(List<Filter> filters);
    Task<SearchResponse> SearchPhysicalMedia(List<Filter> filters);
}