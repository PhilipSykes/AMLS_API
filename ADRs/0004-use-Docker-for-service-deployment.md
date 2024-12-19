---
parent: Decisions
nav_order: 4
---

# Use Docker for service deployment

## Context and Problem Statement

The Advanced Media Library (AML) requires high scalability which informed our decision to use a service oriented arcitecture. We must be able to get the most out of the benefits of the arcitecture, we must select a deployment solution that allows individual control over services with the ability to shut down and restart services.

## Decision Drivers

- Need for encapsulated services
- Need to restart services on the deployed application
- Requirement for maintainability
- Ability to benchmark services

## Considered Options

- Azure Functions
- Docker

## Decision Outcome

Chosen option: Docker

### Consequences

Good, as it allows us to encapsulate services and resources in images
Good, as it allows us to manage and maintain services without shutting down unrelated services
Bad, as its infrastucture is not externally managed

### Confirmation

Docker deployment live as of sprint 2. Implementation has aligned well with decision factors.

## Pros and Cons of the Options

### Azure Functions

Good, as it provides automatic scaling based on demand
Good, as it integrates with other Azure services
Good, as it eliminates infrastructure management overhead
Bad, as it creates potential vendor lock-in to Azure
Bad, as it will become expensive at high load

### Docker

Good, as it provides consistent environments across development, testing, and production
Good, as it enables portability across different cloud providers and on-premises installations
Good, as it enables precise control over application startup, shutdown, and health checks
Bad, as it increases operational overhead for container orchestration and management
Bad, as it can result in unnecessary resource allocation if not properly optimized

## More Information

### Links
