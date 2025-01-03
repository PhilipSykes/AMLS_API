using System.ComponentModel.DataAnnotations;

namespace Common.Models;

public static class PayLoads
{
    public record Login
    {
        [EmailAddress]
        [RegularExpression(@"^[^${}()\[\]]*$")]
        public required string Email { get; set; } = string.Empty;
        
        [RegularExpression(@"^[^${}()\[\]]*$")]
        public required string Password { get; set; } = string.Empty;
    }
    
    public record RefreshToken
    {
        public required string Token { get; set; }
    }

    public record Borrow
    {
        public required string MediaId { get; init; }
        public required string UserId { get; init; }
    }

    public record StaffUser
    {
        public Entities.Staff User { get; set; }
    }
    
    public record MemberUser
    {
        public Entities.Members User { get; set; }
    }

    public record Inventory
    {
        public required List<Entities.PhysicalInventory> PhysicalMediaList { get; set; }
        public required List<Entities.Branch> BranchesList { get; set; }
    }
    
    public record StaffData
    {
        public required List<Entities.Staff> StaffList { get; set; }
        public required List<Entities.Branch> BranchesList { get; set; }
    }
    
    public record MemberData
    { 
        public required List<Entities.Members> MemberList { get; set; }
        
        public required List<Entities.Branch> BranchesList { get; set; }
    }

    public record ReservationExtension
    {
        public required string ReservationId { get; init; }
        public required DateTime NewEndDate { get; init; }
    }
}