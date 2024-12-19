---
parent: Decisions
nav_order: 3
---

# Database software choice

## Context and Problem Statement

The Advanced Media Library (AML) requires the use of a hightly scalable, secure and performant database software
to store the large amounts of information on media, members, branches and payments.
The database should easy to host using cloud services in order to automate scalability,
and be secure to ensure GDPR compliance. We must choose a database software that facilitates fast implementation,
scalability, security, and provides good performance while maintaining data integrity.

## Decision Drivers

- Team's existing experience and expertise
- Ease of scalability
- Good performance, particularly in searches
- Assurance of maintaining data integrity
- Cost-effectiveness in development and deployment
- Security for user data
- Maintainability

## Considered Options

- Neo4j
- MongoDB
- MySQL

## Decision Outcome

Chosen option: MongoDB

### Consequences

- Good, as it allows us to automate resource scaling, cloud hosting and encryption.
- Good, as it is easy to change in according to new business requirements, or unforseen problems.
- Bad, as it will be harder to implement without prior experience

### Confirmation

No confirmation yet

## Pros and Cons of the Options

### Neo4j

- Good, because it has fast search performance, particularly when traversing between many records.
- Good, becuase it ensures data integrity through ACID compliance[1]
- Good, because new nodes and relationships can be easily added in future, according to changing business requirements
- Bad, because no-one in the team has used Neo4j before, so implementation is slower
- Bad, because it is well suited for complex and varying relationships between entities which the system does not need[1].
- Bad, because it takes up more storage space, as relationships are stored at the record level. This also means if hosted on the cloud, it will cost more
- Bad, because it does not provide encryption, though hosting on the cloud may mitigate this.

### MongoDB

- Good, because it can be automatically hosted using AWS, Azure or Google cloud services, meaning the requirement for scalability can be fulfilled easily[2].
- Good, because it supports a wide range of languages including C#, which is part of out tech stack
- Good, because has good search performance
- Good, because it is free to use upto a limit of half a gigabyte, so a proof on concept may be implemented for free
- Good, because it is a no SQL database, allowing for flexible data storage which will allow a simpler and more appropriate database structure, that can accomodate various media types easily even if all attributes arn't known in advance[6].
- Good, because data can be embedded, simplifying the table structure
- Good, because it provides excellent security measures by default
- Bad, because no-one in the team is very familiar with MongoDB, so implementation will be slower
- Bad, support for transactions is present, though worse than relational databases, so it is harder to maintain data integrity

### MySQL

- Good, because the team has used SQL databases multiple times before so it can be implemented more quickly and with less risk of errors
- Good, because the system provides a stable schema, which is suitable for SQL and helps with data consistency
- Good, because SQL queries are powerful and are good for complex queries
- Good, because relational databases are well suited to the structured data which we have to work with
- Good, because it supports ACID transactions[6], which is crucial data integrity for borrowing and payments
- Bad, because SQL databases can struggle with scalability, as they are less suitable to horizontal scaling, while vertical scaling has practical limits.
- Bad, because it is limited to a rigid schema, requiring a more complex structure with more table joins which can be slow
- Bad, because all information that must be stored on media must be known in advance. If some media doesn't conform to the existing design, then it could not be stored and the database and system must be redesigned.

## More Information

### References

-[1]:[Neo4j Documentation](https://neo4j.com/docs/)

-[2]:[MongoDB Documentation](https://www.mongodb.com/docs/manual/)

-[3]:[Guide to MongoDB Transactions](https://www.mongodb.com/products/capabilities/transactions)

-[4]:[Tutorial on non-relational database design](https://www.youtube.com/watch?v=QAqK-R9HUhc)

-[5]:[Comparison of relational and non-relational databases](https://www.mongodb.com/resources/compare/relational-vs-non-relational-databases)

-[6]:[Discussion on the best uses for different database types](https://www.reddit.com/r/Database/comments/13polb6/how_do_you_choose_between_relational_database_and/)
