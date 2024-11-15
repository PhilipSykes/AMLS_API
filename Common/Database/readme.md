Note: This is currently just designed to explain what the services need, it's inner workings aren't currently in here. Let me know if you want me to put in a comprehensive explanation

2nd Note: I'm still working on the code, so this readme may not match current implementation exactly
#### Classes (2):

##### Filter
Filter is a class for transporting filter settings from the UI to the database queries
```c#
    public class Filter (string key, object value, char operation)
    {
        public string key = key;
        public object value = value;
        public char operation = operation;
    }
```
###### Fields:
 - Key: Holds the name of the field being used, it is the same as in the database
 - Value: This is simply passed onto the builder, accepts anything
 - Operator: holds the desired operation as a character from the following options:
	 - '=' - Equal to
	 - '>' - Greater than
	 - '<' - Less than
	 - '!' - Not equal to
	 - '~' - Contains

Important! - Cant currently search arrays like genre
###### Notes:
- Many more operations are possible, but I think this is all we need
- All attributes are currently public, though i might make them private, I don't think they need to change
###### Example:
Where rating is greater than 4
```c#
new Filter("rating", 4, '>');
```




##### SearchRepository

```c#
SearchRepository search = new SearchRepository("c1023778:X4M8yMPq6DNgrOck");
```
- SearchRepository is a class for providing database access to the search service.
- All search methods' filters will be applied together, so there is not currently an OR, only AND.
##### Methods (2):

###### SearchMediaInfo:
```c#
Task<List<BsonDocument>> SearchMediaInfo(List<Filter>)
```
	 - Returns a list of BsonDocument
	 - Takes in a list of Filter objects
	 - Asynchronous
Searches the MediaInfo table with the given filters


###### SearchPhysicalMedia:
```c#
Task<List<BsonDocument>> SearchPhysicalMedia(List<Filter> filters);
```
	- Returns a list of BsonDocument
	- Takes in a list of Filter objects
	- Asynchronous
Searches the PhysicalMedia table with the given filters



Here is a more complete example of a search in action:
```c#
List<Filter> filters = new List<Filter>();
filters.Add(new Filter("rating", 4, '>'));
List<BsonDocument> test = await search.SearchMediaInfo(filters);
PrintResults(test);

void PrintResults(List<BsonDocument> list)
{
    foreach (BsonDocument doc in list)
    {
        Console.WriteLine(doc);
    }
}
```