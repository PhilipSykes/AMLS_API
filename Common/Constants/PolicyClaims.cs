namespace Common.Constants;

public static class PolicyClaims
{
    public const string BranchAccess = "branch_access";
    
}
public static class PolicyRoles
{
    public const string SystemAdmin = "SystemAdmin";
    public const string BranchLibrarian = "BranchLibrarian";
    public const string BranchManager = "BranchManager";
    public const string Accountant = "Accountant";
    public const string Member = "Member";
}

public static class Policies
{
    //Member permissions
    public const string CanReserveMedia = "CanReserveMedia";
    public const string CanCancelMedia = "CanCancelMedia";
    public const string CanBorrowMedia = "CanBorrowMedia";
    public const string CanExtendReservation = "CanExtendReservation";
    public const string CanReturnMedia = "CanReturnMedia";
    
    //Branch staff permissions
    public const string HasBranchAccess = "HasBranchAccess";
    public const string CanEditMedia = "CanEditMedia";
    public const string CanEditBranchMedia = "CanEditBranchMedia";
    public const string CanCreateMedia = "CanCreateMedia";
    public const string CanDeleteMedia = "CanDeleteMedia";
    public const string CanViewInventory = "CanViewInventory";
    
    //SystemAdmin permissions 
    public const string CanViewStaff = "CanViewStaff";
    public const string CanViewMembers = "CanViewMembers";
    public const string CanEditUserRoles = "CanEditUsers";
    public const string CanEditUserPermissions = "CanEditUserPermissions";
    public const string CanViewMetricsReports = "CanViewMetricsReports";
    public const string CanDeleteUserAccounts = "CanDeleteUserAccounts";
    
}