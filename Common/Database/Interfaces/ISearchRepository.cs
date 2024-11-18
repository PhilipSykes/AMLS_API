using Common.Models;
using Common.Constants;

namespace Common.Database.Interfaces;

public interface ISearchRepository
{
    Task<SearchResponse> Search(List<Filter> filters,string documentType);
}