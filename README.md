_____________References______________
-------------------------------------
_________Name_________
CustomAuthenticationStateProvider
_________Classes affected_________
Blazor/Services/AuthService.cs
Blazor/Services/ClientAuthStateProvider.cs
_________Source_________
Code for this service was inspired from https://chrissainty.com/securing-your-blazor-apps-authentication-with-clientside-blazor-using-webapi-aspnet-core-identity/
-------------------------------------
_________Name_________
Claims & Policies system
_________Classes affected_________
AuthService/TokenAuthService.cs
MediaService/Configuration/PolicyConfig.cs
ReservationService/Configuration/PolicyConfig.cs
UserService/Configuration/PolicyConfig.cs
MetricService/Configuration/PolicyConfig.cs
Blazor/Services/ClientAuthStateProvider.cs
Common/Constants/PolicyClaims.cs
_________Source_________
Code for this system was inspired from: 
https://stackoverflow.com/questions/45997100/clarifying-identity-authorization-using-claims-as-roles-roles-and-claims-or-ro
https://chrissainty.com/securing-your-blazor-apps-configuring-role-based-authorization-with-client-side-blazor/
-------------------------------------
