using Microsoft.Extensions.Options;
using Common;
using Common.Constants;
using Common.Database;

namespace Services.SearchService;

public class SearchServiceMessageReceiver : BaseMessageReceiver
{
    private static readonly string[] SearchMessageTypes =
    [
        MessageTypes.Media.Search,
    ];

    public SearchServiceMessageReceiver(IOptions<RabbitMQConfig> options)
        : base(options, SearchMessageTypes)
    {
    }

    protected override async Task HandleMessage(string messageType, string message)
    {
        Console.WriteLine(message);
        SearchRepository search = new SearchRepository("c1023778:X4M8yMPq6DNgrOck");
        search.SearchMediaInfo()
        
    }
}