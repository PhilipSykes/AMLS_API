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
        _testMemberId = "6768742cf21f11238218b235";

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
        Debug.WriteLine($"Edit Non-existent Staff Result: {result.Message}");
    }

    [Test]
    public async Task DeleteStaffBadIdTest()
    {
        var result = await _userManager.DeleteStaff(_testStaffBadId);

        Assert.That(result.Success, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.NotFound));
        Debug.WriteLine($"Delete Non-existent Staff Result: {result.Message}");
    }
    
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
        Debug.WriteLine($"Delete Non-existent Member Result: {result.Message}");
    }
    
}