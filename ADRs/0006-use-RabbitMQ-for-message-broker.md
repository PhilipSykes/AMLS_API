---
parent: Decisions
nav_order: 6
---

# Use RabbitMQ for message broker

## Context and Problem Statement

Our service-oriented architecture for AML requires an effective communication strategy between services including Media Management, User Authentication, and Search. Microsoft's guidance on microservice communication patterns [1] emphasizes that this choice significantly impacts system reliability and scalability.

We need to determine: Which inter-service communication method will best ensure reliability, scalability, and maintainability?

## Decision Drivers

- Reliable and traceable message delivery
- Support for high-volume inter-service communication
- Integration with .NET technology stack
- Asynchronous operation support

## Considered Options

- Direct HTTP communication between services
- Message broker (RabbitMQ)
- Message broker (Azure Service Bus)

## Decision Outcome

Selected RabbitMQ based on Enterprise Integration Patterns [2] guidance regarding message broker suitability for SOA, particularly for publish/subscribe patterns enabling service decoupling.

### Consequences

- Good, because enables service communication without direct coupling
- Good, because ensures message persistence during service downtime
- Good, because provides load management through queuing[3]
- Bad, because increases infrastructure complexity
- Bad, because requires team training on message broker concepts

### Confirmation

Sprint 1 implementation positioned the message broker between gateway and services on top of service-to-service. Performance testing revealed latency issues, leading to revision to service-to-service communication model as originally intended.

## Pros and Cons of the Options

### Direct HTTP communication

- Good, because simple initial implementation
- Good, because familiar HTTP/REST protocols
- Bad, because complicates asynchronous operations
- Bad, because scaling complexity increases with service count

### RabbitMQ

- Good, because enables decoupled service communication
- Good, because native asynchronous operation support
- Good, because supports automatic message retry[4]
- Good, because enables multi-service message subscription
- Bad, because requires additional infrastructure management
- Bad, because increases monitoring complexity

### Azure Service Bus

- Good, because provides native .NET/Blazor integration
- Bad, because requires paid subscription
- Bad, because restricts deployment to Azure platform

## More Information

During our research, we discovered that a message broker would be more suitable than direct communication for a SOA architecture. RabbitMQ's publish/subscribe model particularly stood out because:

When a media item is updated:

1.Search service needs to update its index
2.Notification service needs to notify relevant users
3.Report service needs to log the change

With publish/subscribe one message can trigger all these actions without the Media service needing to know about all these dependent services.

### References

[1]:[Microsoft. "Communication in microservice architecture." Microsoft Learn,](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/communication-in-microservice-architecture) - Communication patterns guide for microservice architectures.

[2]:["Use of messaging brokers", Enterprise Integration Patterns,](https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessageBroker.html) - Message broker implementation guidance.

[3]:[Microsoft. "Asynchronous messaging options." Microsoft Learn,](https://learn.microsoft.com/en-us/azure/architecture/guide/technology-choices/messaging) - Overview of message brokers and Azure Service Bus.

[4]:[RabbitMQ with .NET](https://www.rabbitmq.com/client-libraries/dotnet) - RabbitMQ .NET client documentation.
