---
parent: Decisions
nav_order: 7
---

# Use Ocelot for gateway

## Context and Problem Statement

After deciding to implement a service-oriented architecture for AML, we need a way to manage and route requests to our various services efficiently. The system needs a single entry point that can handle routing, load balancing, and authentication.

## Decision Drivers

-Need for a centralized entry point to our services
-Requirement for request routing and load balancing
-Authentication and authorization must be handled consistently
-Solution must integrate well with our .NET ecosystem
-Need for monitoring and rate limiting capabilities

## Considered Options

-Ocelot API Gateway
-YARP
-Custom Gateway

## Decision Outcome

Chosen option: "Ocelot API Gateway". As it offers more key features such as load-balancing & authentication that we want to utlize for our project than contemporaries like YARP.

### Consequences

-Good, because it provides built-in integration with .NET Core authentication[3]
-Good, because it supports dynamic routing and load balancing out of the box
-Good, because it allows for easy configuration through JSON files
-Bad, because it adds another layer of complexity to our architecture
-Bad, because team needs to learn Ocelot's configuration patterns

### Confirmation

Initial implementation completed in sprint 1 with basic routing configuration. Sprint 2 saw us add authentication integration.

## Pros and Cons of the Options

### Ocelot API Gateway

-Good, because it's simple to implement initially
-Good, because team are already familiar with HTTP/REST
-Bad, because it makes asynchronous operations more complex
-Bad, because scaling becomes more difficult as services multiply

### YARP

-Good, because it's built and maintained by the Microsoft .NET team[2]
-Good, because it provides more flexibility in configuration and customization[2]
-Bad, because it requires more manual implementation of features like authentication
-Bad, because it needs more boilerplate code compared to Ocelot
-Bad, because there are fewer ready-made middleware components available unlike Ocelot

### Custom Gateway

-Good, because we have complete control over the implementation
-Good, because it can be optimized for our specific use cases
-Bad, because it requires significant development effort[1]
-Bad, because we would need to implement and test our own security patterns

## More Information

### References

[1]:[Building a custom API gateway](https://ga1.medium.com/api-gateway-ae3098818028)
[2]:[YARP Documentation](https://microsoft.github.io/reverse-proxy/api/index.html)
[3]:[Ocelot Documentation](https://ocelot.readthedocs.io/en/latest/introduction/bigpicture.html)
