using Common.Constants;
using static Common.Models.Operations;
using static Common.Models.PayLoads;

namespace UserService;

public interface IUserManager
{
    Task<Response<string>> EditStaff(Request<StaffUser> user);
    Task<Response<string>> DeleteStaff(Request<StaffUser> user);
    Task<Response<string>> DeleteMember(Request<MemberUser> user);
}
public class UserManager : IUserManager
{

    public async Task<Response<string>> EditStaff(Request<StaffUser> user)
    {
        
        return new Response<string>()
        {
            Success = true,
            Message = "Item successfully edited",
            StatusCode = QueryResultCode.Ok,
        };
    }
    
    public async Task<Response<string>> DeleteStaff(Request<StaffUser> user)
    {
        //TODO filter builder usage within to find item by ID 
        return new Response<string>()
        {
            Success = true,
            Message = "Item successfully deleted",
            StatusCode = QueryResultCode.Ok,
        };
    }
    
    public async Task<Response<string>> DeleteMember(Request<MemberUser> user)
    {
        //TODO filter builder usage within to find item by ID 
        return new Response<string>()
        {
            Success = true,
            Message = "Item successfully deleted",
            StatusCode = QueryResultCode.Ok,
        };
    }
    
    
}