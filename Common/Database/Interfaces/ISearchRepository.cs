using Common.Models;
using Common.Constants;

namespace Common.Database.Interfaces;

public interface ISearchRepository
{
    Task<SearchResponse> Search(string documentType, (int, int) pagination, List<Filter> filters = null);
}