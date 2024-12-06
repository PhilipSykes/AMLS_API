namespace Common.Models;

public static class PayLoads
{
    public record Login
    {
        public required string Email { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
    }

    public record Reserve
    {
        public required string MediaId { get; init; }
        public required string UserId { get; init; }
    }

    public record Inventory
    {
        public required List<Entities.MediaInfo> MediaInfoList { get; set; }
        public required List<Entities.Branch> BranchesList { get; set; }
    }
}