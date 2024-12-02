using MongoDB.Driver;
using static Common.Models.Shared;

namespace Common.Database.Interfaces;

public interface IFilterBuilder<T>
{
    FilterDefinition<T> BuildFilter(List<Filter> filterObjectsIn);
}