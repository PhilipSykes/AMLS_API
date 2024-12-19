---
parent: Decisions
nav_order: 1
---

# Use Service-oriented architecture

## Context and Problem Statement

The Advanced Media Library (AML) requires a modern library management system to streamline operations across various access points while serving a large user base across the country. The system needs to handle multiple media types while maintaining fast access times and data integrity across various access points.

We need to determine: What architecture style will best support our scalability and performance requirements?

## Decision Drivers

- Need for a system that can handle 13.4 million users with 10% annual growth i.e. how intuitively scalable is the architecture style
- Performance requirements for rapid search results and quick webpage loading times
- Requirement for centralized management with distributed operations
- Must efficiently handle a large number of diverse media types
- Need for easy updates and modifications without disrupting the entire system

## Considered Options

- Three-Tier Architecture
- Microservices Architecture
- Service-Oriented Architecture (SOA)

## Decision Outcome

Chosen option: "Service-Oriented Architecture (SOA)", because it gives us a good balance between the flexibility of microservices and the simplicity of monolithic systems, while meeting our scalability and performance requirements.

### Consequences

- Good, because it has allowed us to explore a new unused architecture type for the team.
- Good, because it has honed our ability to delegate specific sections between the team.
- Good, because its provided the team with better fault isolation & hotfix options throughout development.
- Good, because it allows for easier integration of new technologies and services in the future.
- Bad, because it has resulted in an increased workload developing the various independant services.

### Confirmation

Architecture fully implemented as of sprint 2 with services deployed independently of each other locally or via docker thereby meeting our maintainability & scalability needs.

## Pros and Cons of the Options

### Three-Tier Architecture

- Good, because it's simpler to develop and deploy initially[1].
- Good, because it provides clear separation of concerns between presentation, business logic, and data layers[1]
- Bad, because horizontal scaling is more complex than in service-based architectures[4].
- Bad, because updates to one part of the system might require redeploying the entire application.
- Bad, because it might not meet our scalability and maintainability needs in the long term.

### Microservices Architecture

- Good, because it offers the highest level of scalability and flexibility[2].
- Good, because it allows for independent deployment of services[2].
- Bad, because it introduces massive complexity in service management and communication.
- Bad, because it might be overkill for our current needs.
- Bad, because it may require a lot more set up,resources & time to implement effectively.

### Service-oriented Architecture

- Good, because it provides a balance between scalability and manageability.
- Good, because it allows for modular development and deployment of services.[5]
- Good, because unlike microservices, its inherently aimed at component sharing[3]
- Good, because it supports our maintainability requirements by allowing updates to individual services.
- Good, because it can help us meet our performance requirements with optimized service design.
- Neutral, because it introduces some complexity in service communication, but not as much as microservices.
- Bad, because it may require more initial setup compared to a monolithic or three-tier system.

## More Information

We performed a degree of research from our chosen options to formulate our final decision. links provided below.

### References

[1]:[IBM. "Three-tier architecture." IBM Topics,](https://www.ibm.com/topics/three-tier-architecture)- Comprehensive guide by IBM to three-tier systems.

[2]:[IBM. "SOA vs. Microservices: What's the Difference?" IBM Think Blog,](https://www.ibm.com/think/topics/soa-vs-microservices)- IBM's blog post explains the differences between SOA and microservices, giving insight into why SOA might be a better fit for certain scenarios.

[3]:[NGINX. "Microservices vs. Service-Oriented Architecture." F5 Networks,](https://www.f5.com/content/dam/f5/corp/global/pdf/ebooks/Microservices_vs_SOA_NGINX.pdf) - NGINX's comparison between microservices and SOA provides additional context for our decision to choose SOA over microservices.

[4]:[TechTarget. "Three-tier vs. microservices architecture: How to choose." SearchAppArchitecture,](https://www.techtarget.com/searchapparchitecture/tip/Three-tier-vs-microservices-architecture-How-to-choose) - Article by techtarget which briefly discusses considerations to be made when deciding on three-tier / Microservice architectures.

[5]:[Amazon "What’s the Difference Between SOA and Microservices?" AWS Amazon,](https://aws.amazon.com/compare/the-difference-between-soa-microservices/) - Amazon guide talking through the different facets of SOA vs Microservices.
