---
parent: Decisions
nav_order: 8
---

# Use JWT Tokens and .NET Claims for Authorization

## Context and Problem Statement

Our application needs a robust authentication and authorization mechanism that can securely manage user identity and permissions across our services. We need a solution that is secure, scalable, and integrates well with our .NET ecosystem.

## Decision Drivers

- Need for stateless authentication to support horizontal scaling
- Must support fine-grained authorization controls
- Solution should work seamlessly across multiple services
- Must be secure and follow industry best practices
- Need for efficient token validation and minimal database lookups
- Good integration with .NET ecosystem

## Considered Options

- JWT tokens with .NET claims
- Session-based authentication
- OAuth2 with external identity provider

## Decision Outcome

Chosen option: "JWT tokens with .NET claims" because it provides a stateless, secure authentication mechanism that integrates naturally with .NET's authorization system.

### Consequences

- Good, because tokens can be validated without database lookups
- Good, because .NET claims provide a standard way to handle authorization
- Good, because it's scalable and works well in distributed systems
- Bad, because tokens cannot be invalidated before expiry
- Bad, because token size increases with number of claims
- Bad, because requires careful handling of token storage on client side

### Confirmation

Implementation created & tested in sprint 2 with user roles and permissions. We also incorporated sessions but purely for token storage on client-side & not authentication.

## Pros and Cons of the Options

### JWT tokens with .NET claims

- Good, because it's built into ASP.NET Core ecosystem[1]
- Good, because claims-based authorization is flexible and easily extended with policies[3]
- Good, because JWTs can transfer user details eliminating the need to query the database or authentication server[4]
- Bad, because tokens must be kept small to minimize overhead

### Session-based authentication

- Good, because sessions can be invalidated immediately
- Good, because it's simpler to implement
- Bad, because requires session state storage
- Bad, because doesn't scale horizontally without additional infrastructure
- Bad, because requires more database lookups

### OAuth2 with external identity provider

- Good, because OAuth has well-tested client libraries in almost every language and web framework[4]
- Good, because provides additional security features
- Bad, because adds external dependency
- Bad, because requires more complex implementation
- Bad, because may have associated costs

## More Information

### References

[1]: [ASP.NET Core JWT Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
[2]: [Claims-based authorization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/claims)
[3]:[Claims based Authentication](https://chrissainty.com/securing-your-blazor-apps-authentication-with-clientside-blazor-using-webapi-aspnet-core-identity/) - useful guide to claims & policy configuration in blazor apps.
[4]:[OAuth vs JWT](https://frontegg.com/blog/oauth-vs-jwt#:~:text=OAuth%20and%20JWT%20are%20both,server%2Dto%2Dserver%20authorization.) - article detailing pros & cons of OAuth & JWT.
