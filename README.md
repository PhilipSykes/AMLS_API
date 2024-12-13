## References

### 1. Custom Authentication State Provider

**Classes Affected:**
- `Blazor/Services/AuthService.cs`
- `Blazor/Services/ClientAuthStateProvider.cs`

**Source:**  
Code for this service was inspired from [Securing Your Blazor Apps - Authentication with ClientSide Blazor Using WebAPI & ASP.NET Core Identity](https://chrissainty.com/securing-your-blazor-apps-authentication-with-clientside-blazor-using-webapi-aspnet-core-identity/)

### 2. Claims & Policies System

**Classes Affected:**
- `AuthService/TokenAuthService.cs`
- `MediaService/Configuration/PolicyConfig.cs`
- `ReservationService/Configuration/PolicyConfig.cs`
- `UserService/Configuration/PolicyConfig.cs`
- `MetricService/Configuration/PolicyConfig.cs`
- `Blazor/Services/ClientAuthStateProvider.cs`
- `Common/Constants/PolicyClaims.cs`

**Sources:**
Code for this system was inspired from
- [Clarifying Identity Authorization using Claims as Roles, Roles and Claims](https://stackoverflow.com/questions/45997100/clarifying-identity-authorization-using-claims-as-roles-roles-and-claims-or-ro)
- [Securing Your Blazor Apps - Configuring Role-Based Authorization with Client-Side Blazor](https://chrissainty.com/securing-your-blazor-apps-configuring-role-based-authorization-with-client-side-blazor/)
### 3. JWT Creation & Validation

**Classes Affected:**
-`AuthService/Program.cs`
- `AuthService/TokenAuthService.cs`
**Sources:**
https://auth0.com/blog/how-to-validate-jwt-dotnet/
https://dotnetfullstackdev.medium.com/jwt-token-authentication-in-c-a-beginners-guide-with-code-snippets-7545f4c7c597
