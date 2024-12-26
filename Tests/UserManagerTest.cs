using System.Diagnostics;
using Common;
using Common.Constants;
using Common.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using UserService;
using static Common.Models.Operations;
using static Common.Models.PayLoads;

namespace Tests;

[TestFixture]
[TestOf(typeof(UserManager))]
public class UserManagerTest
{
    private UserManager _userManager;
    private string _testStaffBadId,_testMemberBadId;
    private string _testStaffId,_testMemberId;
    private string _testStaffCurrentRole, _testStaffNewRole;
    private string _testBranchId, _testBranchNewId;

    /// <summary>
    /// Current Staff user state in database (Staff Table):
    /// <code>
    /// {
    ///     "_id": "67686b3cf21f11238218b230"
    ///     "firstName":"Test"
    ///     "lastName":"Test"
    ///     "dateOfBirth": "2002-01-26T00:00:00.000+00:00"
    ///     "role": "currentRole"
    ///     "email":"test@aml.com"
    ///     "phone":"test"
    ///     "branches" Array ["testBranchValue"]
    /// }
    /// </code>
    /// 
    /// Expected state after update:
    /// <code>
    /// {
    ///     "_id": "67686b3cf21f11238218b230"
    ///     "firstName":"Test"
    ///     "lastName":"Test"
    ///     "dateOfBirth": "2002-01-26T00:00:00.000+00:00"
    ///     "role": "newRole"
    ///     "email":"test@aml.com"
    ///     "phone":"test"
    ///     "branches" Array ["testBranchValue","newTestBranchValue"]
    /// }
    /// </code>
    /// Current Staff user in Login table:
    /// <code>
    ///     "_id": "676d9f837f30b6d3411432ea"
    ///     "user":"67686b3cf21f11238218b230"
    ///     "role": "currentRole"
    ///     "email":"test@aml.com"
    ///     "branches" Array ["testBranchValue"]
    ///</code>
    /// </summary>
    [OneTimeSetUp]
    public void Setup()
    {
        var settings = Options.Create(new MongoDBConfig
        {
            ConnectionString = "mongodb+srv://c1023778:X4M8yMPq6DNgrOck@cluster0.simvp.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0",
            DatabaseName = "AdvancedMediaLibrary",
        });
        _userManager = new UserManager(settings);
        
        //Test Member & User created in DB, credentials 
        _testStaffId = "67686b3cf21f11238218b230";
        _testStaffCurrentRole = "currentRole";
        _testBranchId = "testBranchValue";
        _testMemberId = "676da1647f30b6d3411432ee";

        _testStaffNewRole = "newRole";
        _testBranchNewId = "newTestBranchValue";
        
        // Generate bad IDs
        _testStaffBadId = ObjectId.GenerateNewId().ToString();
        _testMemberBadId = ObjectId.GenerateNewId().ToString();
    }

    [Test]
    public async Task EditStaffTest()
    {
        var staffUser = new StaffUser
        {
            User = new Entities.Staff
            {
                ObjectId = _testStaffId, 
                Role = _testStaffNewRole,
                Branches = new string[] { _testBranchId, _testBranchNewId } 
            }
        };
        var request = new Request<StaffUser> { Data = staffUser };
        
        var result = await _userManager.EditStaff(request);
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.Ok));
        Debug.WriteLine($"Edit Staff Result: {result.Message}");
    }

    [Test]
    public async Task EditStaffBadIdTest()
    {
        var staffUser = new StaffUser
        {
            User = new Entities.Staff
            {
                ObjectId = _testStaffBadId,
                Role = _testStaffCurrentRole,
                Branches = new string[] { _testBranchId }
            }
        };
        var request = new Request<StaffUser> { Data = staffUser };
        
        var result = await _userManager.EditStaff(request);
        
        Assert.That(result.Success, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.NotFound));
        Debug.WriteLine($"Edit BadId Staff Result: {result.Message}");
    }

    [Test]
    public async Task DeleteStaffBadIdTest()
    {
        var result = await _userManager.DeleteStaff(_testStaffBadId);

        Assert.That(result.Success, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.NotFound));
        Debug.WriteLine($"Delete BadId Staff Result: {result.Message}");
    }
    /// <summary>
    /// Current Member user state in database (Members Table):
    /// <code>
    /// {
    ///     "_id": "676da1647f30b6d3411432ee"
    ///     "firstName":"Test"
    ///     "lastName":"Test"
    ///     "dateOfBirth": "2002-01-26T00:00:00.000+00:00"
    ///     "email":"test@sky.com"
    ///     "phone":"test"
    /// }
    /// </code>
    /// Current Member user in Login table:
    /// <code>
    ///     "_id": "676da3627f30b6d3411432f0"
    ///     "user":"676da1647f30b6d3411432ee"
    ///     "role": "Member"
    ///     "email":"test@sky.com"
    ///     "branches" Array ["testBranchValue"]
    ///</code>
    /// </summary>  
    [Test]
    public async Task DeleteMemberTest()
    {

        var result = await _userManager.DeleteMember(_testMemberId);
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.Ok));
        Debug.WriteLine($"Delete Member Result: {result.Message}");
    }
    
    [Test]
    public async Task DeleteMemberBadIdTest()
    {

        var result = await _userManager.DeleteMember(_testMemberBadId);
        
        Assert.That(result.Success, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.NotFound));
        Debug.WriteLine($"Delete BadId Member Result: {result.Message}");
    }
    
}