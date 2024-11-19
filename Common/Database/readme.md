# Search System Documentation

## Overview
The search system provides a flexible way to query the media database using filters. It uses MongoDB as the underlying database and supports various filter operations.

## Core Components

### Filter
Filter is a class for transporting filter settings from the UI to the database queries

```csharp
public class Filter(string key, object value, char operation)
{
    public string Key { get; set; } = key;
    public object Value { get; set; } = value;
    public char Operation { get; set; } = operation;
}
```

#### Fields
- `Key`: The name of the field being queried (matches database field name)
- `Value`: The value to filter by (accepts any object type)
- `Operation`: The filter operation to perform, options:
    - `'='` - Equal to
    - `'>'` - Greater than
    - `'<'` - Less than
    - `'!'` - Not equal to
    - `'~'` - Contains (case-insensitive)

#### Example
Filter for media with rating greater than 4:
```csharp
new Filter("rating", 4, '>');
```

### SearchResponse Class
Represents the response from any search operation.

```csharp
public class SearchResponse
{
    public List<BsonDocument> Results { get; set; }
    public int TotalCount { get; set; }
    public string? Error { get; set; }
}
```

#### Fields
- `Results`: List of matching documents
- `TotalCount`: Number of results found
- `Error`: Error message if the search failed (null if successful)

### SearchRepository

The SearchRepository provides database access for search operations. Configuration is handled through dependency injection using `MongoDBConfig`.

```csharp
// Registration in Program.cs
builder.Services.Configure<MongoDBConfig>(
    builder.Configuration.GetSection("MongoDB"));
builder.Services.AddScoped<ISearchRepository, SearchRepository>();
```

#### Methods

##### SearchMediaInfo
```csharp
Task<SearchResponse> SearchMediaInfo(List<Filter> filters)
```
- Searches the MediaInfo collection
- Returns a SearchResponse containing results or error
- Takes a list of Filter objects
- Filters are combined with AND logic

##### SearchPhysicalMedia
```csharp
Task<SearchResponse> SearchPhysicalMedia(List<Filter> filters)
```
- Searches the PhysicalMedia collection
- Returns a SearchResponse containing results or error
- Takes a list of Filter objects
- Filters are combined with AND logic

### Complete Example

```csharp
// Service injection
public class MediaSearchService
{
    private readonly ISearchRepository _searchRepository;

    public MediaSearchService(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<SearchResponse> SearchMediaAsync(List<Filter> filters)
    {
        try 
        {
            var response = await _searchRepository.SearchMediaInfo(filters);
            Console.WriteLine($"Found {response.TotalCount} results");
            return response;
        }
        catch (Exception ex)
        {
            return new SearchResponse 
            { 
                Error = $"Search failed: {ex.Message}",
                Results = new List<BsonDocument>(),
                TotalCount = 0
            };
        }
    }
}

// Usage example
var filters = new List<Filter>
{
    new Filter("rating", 4, '>'),
    new Filter("title", "matrix", '~')  // Case-insensitive contains
};

var response = await _mediaSearchService.SearchMediaAsync(filters);

if (response.Error != null)
{
    Console.WriteLine($"Error: {response.Error}");
    return;
}

foreach (var doc in response.Results)
{
    Console.WriteLine(doc);
}
```

## Configuration
The search system requires MongoDB connection details in appsettings.json:

```json
{
  "MongoDB": {
    "ConnectionString": "your-connection-string+connection settings",
    "DatabaseName": "AdvancedMediaLibrary"
  }
}
```

> **Note**: This documentation reflects the current implementation. Future updates may include support for array field searches and OR operations between filters.