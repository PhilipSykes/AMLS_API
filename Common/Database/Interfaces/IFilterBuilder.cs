using Common.Models;
using MongoDB.Driver;

namespace Common.Database.Interfaces;

    public interface IFilterBuilder<T>
    {
        FilterDefinition<T> BuildFilter(List<Filter> filtersIn);
    }
