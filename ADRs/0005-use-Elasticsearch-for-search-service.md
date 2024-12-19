---
parent: Decisions
nav_order: 5
---

# Use Elasticsearch for search service

## Context and Problem Statement

The Advanced Media Library (AML) requires efficient search functionality to handle large volumes of diverse media types across multiple branches. The search system must provide fast, accurate results while handling complex queries and scaling with system growth.

We need to determine: How to implement search capabilities that meet performance requirements while integrating with our service-oriented architecture?

## Decision Drivers

- Search performance across large media volume
- Scalability for growing collections and user base
- Architecture compatibility
- Real-time indexing support
- Performance under concurrent load

## Considered Options

- Elasticsearch
- PostgreSQL full-text search
- Custom-built search solution

## Decision Outcome

Selected Elasticsearch due to its scalability, feature set, and alignment with architectural requirements.

### Consequences

- Good, because achieved required media search performance
- Good, because NEST client enables smooth .NET integration
- Good, because supports zero-downtime index updates
- Bad, because increased deployment and monitoring complexity

### Confirmation

Implementation abandoned during sprint 1 development. Performance testing showed pagination,query limits & mongo indexing provided sufficient optimization without dedicated search service.

## Pros and Cons of the Options

### Elasticsearch

- Good, because provides high performance and scalability[1]
- Good, because offers advanced query capabilities and faceted search[2]
- Good, because supports real-time indexing
- Good, because maintains active community with extensive documentation[2]
- Good, because reduces primary database load
- Bad, because requires additional infrastructure management
- Bad, because complicates data synchronization

### PostgreSQL full-text search

- Good, because integrates with existing database infrastructure[4]
- Good, because adequate for small/medium dataset searching
- Bad, because limited scalability for large datasets[4]
- Bad, because lacks advanced search features[4]
- Bad, because concentrates load on primary database

### Custom-built search solution

- Good, because allows precise feature customization
- Good, because eliminates external dependencies
- Good, because reduces database bottlenecks
- Bad, because requires significant development resources
- Bad, because lacks optimization of established solutions

## More Information

### References

[1]:[Elasticsearch vs PostgresSQL](https://stackshare.io/stackups/elasticsearch-vs-postgresql) - Feature comparison between Elasticsearch and PostgreSQL.

[2]:[Elasticsearch basics](https://www.elastic.co/guide/en/elasticsearch/reference/current/elasticsearch-intro-what-is-es.html) - Overview of Elasticsearch features and use cases.

[3]:[search systems within micro-services](https://medium.com/@tusharsheth/tech-stack-for-search-system-with-micro-services-architecture-38852c769d87) - Guide to search implementation in microservices architecture.

[4]:[elasticsearch vs postgres](https://www.paradedb.com/blog/elasticsearch_vs_postgres) - Analysis of search capabilities between solutions.
