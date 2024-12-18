# API Docs

Group 7 AMLS 

---


# Overview

The AMLS API is an internal service-oriented application built using [ASP.NET](http://ASP.NET) Core. This API provides centralised access to all library management functionality and is intended to be used solely for AMLS systems.

### Default Path

```
https://localhost:7500/api/
```

**API Versioning -** the current path does not include path versioning 

### Technical Stack

- [ASP.NET](http://ASP.NET) Core 8.0
- [Ocelot](https://ocelot.readthedocs.io/en/latest/) API Gateway
- MongoDB [Atlas](https://www.mongodb.com/products/platform/cloud)
- JWT Authentication

### Authentication

Most API endpoints require JWT authentication, This token must be included in the auth header using the Bearer scheme.

# Format & Examples

```jsx
Authorization: Bearer {your-jwt-token}
```

### Example Implementation

---

**.Net HttpClient**

```csharp
HttpClient.DefaultRequestHeaders.Authorization = new 
System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
```

**HTTP Request**

```jsx
GET /api/admin/metric HTTP/1.1
Host: api.example.com
Authorization: Bearer {your-jwt-token}
```

---

# Endpoint Overview

### Generic Response Format

```json
{
    "data": {},
    "success": true,
    "message": "Success Message",
    "statusCode": 200
}
```

### Response Codes

| HTTP Code | Description |
| --- | --- |
| 200  `OK` | Request was successfully processed |
| 201   `Created` | A new resource has been successfully created |
| 400  `Bad Request` | Server cannot process the request due to client input errors |
| 401   `Unauthorised` | The client is not authenticated |
| 403  `Forbidden` | The client does not have sufficient claim for the requested resource |
| 404  `Not Found` | The requested resource does not exist |
| 500  `Internal Server Error` | The server encountered an unexpected error that prevented it from completing the request |

### Authentication and Claims Requirement

| Upstream Path | Supported HTTP Methods | Required Role | Required Auth |
| --- | --- | --- | --- |
| `/api/auth/{everything}` | `POST` | None | None |
| `/api/user/{everything}` | `POST`, `GET` | `SystemAdmin` | `JWT` |
| `/api/catalog/{everything}` | `POST`, `GET` | None | None |
| `/api/inventory/{everything}` | `POST`, `GET`, `PUT`, `DELETE` | `BranchLibrarian`, `BranchManager` | `JWT` |
| `/api/reservations/{everything}` | `POST`, `DELETE` | `Member` | `JWT` |
| `/api/loans/{everything}` | `POST`, `GET`, `PUT` | `Member` | `JWT` |
| `/api/admin/{everything}` | `POST`, `GET`, `PUT`, `DELETE` | `SystemAdmin` | `JWT` |

<aside>

### Auth Service

---

AuthController.cs

<aside>

`POST /auth/login` 

---

**DESC:** Paginated User Search

---

**BODY PARAM:**

`email` - `string`

`password` - `string`

---

- **Example Response**
    
    ```json
    {
        "data": {
            "userID": "67236480b4d08aab049740aa",
            "branches": [
                "67236fc4b4d08aab049740ca"
            ],
            "token": "user-JWT-token"
        },
        "success": true,
        "message": "Login successful",
        "statusCode": 0
    }
    ```
    
</aside>

*JWT Tokens are obtained through this endpoint and are required for all authenticated endpoints.*

</aside>

<aside>

### User Service

---

UserManagementController.cs

<aside>

`GET /UserManagement/staff`

---

**DESC:** Retrieve paginated staff data, including branch information

---

**QUERY PARAM:**

`page` - `int` Page number

`count` - `int` Items per page

---

- **Example Response**
    
    ```json
    {
      "matchCount": 1,
      "data": {
        "staffList": [
          {
            "objectId": "675194ee681a517146fc4945",
            "firstName": "Bob",
            "lastName": "Dole",
            "dateOfBirth": "2002-01-26T00:00:00Z",
            "role": "BranchManager",
            "email": "",
            "phoneNumber": "",
            "branches": [
              "6733491ba1ed2d02eef46a42",
              "67236fc4b4d08aab049740ca",
              "673348f4a1ed2d02eef46a41"
            ]
          }
        ],
        "branchesList": [
          {
            "objectId": "67236fc4b4d08aab049740ca",
            "name": "Elm Park Library",
            "address": {
              "street": "15 Elm Street",
              "town": "Birmingham",
              "postcode": "B3 2AX"
            }
          }
        ]
      },
      "success": true,
      "message": null,
      "statusCode": 200
    }
    ```
    

---

**AUTH:** Requires policy `CanViewStaff`.

</aside>

<aside>

`POST /UserManagement/staff/search`

---

**DESC:** Perform paginated search on staff records with specified search filters

---

**QUERY PARAM:**

`page` - `int` Page number

`count` - `int` Items per page

`filters` - List of `Filter` objects

---

- **Example Response**
    
    ```json
    {
      "matchCount": 1,
      "data": [
        {
          "objectId": "67519756681a517146fc4953",
          "firstName": "Billy",
          "lastName": "Connolly",
          "dateOfBirth": "2002-01-26T00:00:00Z",
          "role": "SystemAdmin",
          "email": "",
          "phoneNumber": "",
          "branches": [
            "675197c7681a517146fc4955"
          ]
        }
      ],
      "success": true,
      "message": null,
      "statusCode": 200
    }
    ```
    

---

**AUTH:** Requires policy `CanViewStaff`

</aside>

<aside>

`PUT /UserManagement/staff/edit`

---

**DESC:** Edit staff data

---

**BODY PARAM:**

`user` - `Request<PayLoads.StaffUser>` new staff data to update

---

- **Example Response**
    
    ```json
    {
      "data": null,
      "success": true,
      "message": "Staff member updated successfully",
      "statusCode": 200
    }
    ```
    

---

**AUTH:** Requires policy `CanEditUserRoles` and `CanEditUserPermissions`

</aside>

<aside>

`DELETE /UserManagement/staff/delete/{userid}`

---

**DESC:** Delete staff account data

---

**ROUTE PARAM:**

`userId` - `string` staff user Id

---

- **Example Response**
    
    ```json
    {
      "data": null,
      "success": true,
      "message": "Staff member deleted successfully",
      "statusCode": 200
    }
    ```
    

---

**AUTH:** Requires policy `CanDeleteUserAccounts`

</aside>

<aside>

`GET /UserManagement/members`

---

**DESC:** Retrieve paginated member data, including branch information

---

**QUERY PARAM:**

`page` - `int` Page number

`count` - `int` Items per page

---

- **Example Response**
    
    ```json
    {
      "matchCount": 1,
      "data": {
        "memberList": [
          {
            "objectId": "67236480b4d08aab049740aa",
            "firstName": "Will",
            "lastName": "Turner",
            "dateOfBirth": "2002-01-26T00:00:00Z",
            "email": "",
            "phoneNumber": "07728 194742",
            "settings": [],
            "favourites": [],
            "history": [
              "67586896360a7f5735c67352",
              "675c4df0c251f6bc58a16749",
              "675c4e65dc48be565ddfceb7",
              "675c5b43db096414f3beaec2"
            ],
            "nearestBranch": "67236fc4b4d08aab049740ca"
          }
        ],
        "branchesList": [
          {
            "objectId": "67236fc4b4d08aab049740ca",
            "name": "Elm Park Library",
            "address": {
              "street": "15 Elm Street",
              "town": "Birmingham",
              "postcode": "B3 2AX"
            }
          }
        ]
      },
      "success": true,
      "message": null,
      "statusCode": 200
    }
    ```
    

---

**AUTH:** Requires policy `CanViewMembers`

</aside>

<aside>

`POST /UserManagement/members/search`

---

**DESC:** Perform paginated search on member records with specified search filters

---

**QUERY PARAM:**

`page` - `int` Page number

`count` - `int` Items per page

`filters` - List of `Filter` objects

---

- **Example Response**
    
    ```json
    {
      "matchCount": 16,
      "data": {
        "memberList": [
          {
            "objectId": "67236480b4d08aab049740aa",
            "firstName": "Will",
            "lastName": "Turner",
            "dateOfBirth": "2002-01-26T00:00:00Z",
            "email": "",
            "phoneNumber": "07728 194742",
            "settings": [],
            "favourites": [],
            "history": [
              "67586896360a7f5735c67352",
              "675c4df0c251f6bc58a16749",
              "675c4e65dc48be565ddfceb7",
              "675c5b43db096414f3beaec2"
            ],
            "nearestBranch": "67236fc4b4d08aab049740ca"
          }
        ],
        "branchesList": [
          {
            "objectId": "67236fc4b4d08aab049740ca",
            "name": "Elm Park Library",
            "address": {
              "street": "15 Elm Street",
              "town": "Birmingham",
              "postcode": "B3 2AX"
            }
          }
        ]
      },
      "success": true,
      "message": null,
      "statusCode": 200
    }
    ```
    

---

**AUTH:** Requires policy `CanViewMembers`

</aside>

<aside>

`DELETE /UserManagement/members/delete/{userid}`

---

**DESC:** Delete member account data

---

**ROUTE PARAM:**

`userId` - `string` member user Id

---

- **Example Response**
    
    ```json
    {
      "data": null,
      "success": true,
      "message": "Member deleted successfully",
      "statusCode": 200
    }
    ```
    

---

**AUTH:** Requires policy `CanDeleteUserAccounts`

</aside>

</aside>

<aside>

### **Reservation Service**

---

ReservationsController.cs

<aside>

`POST /reservations/create` 

---

**DESC:** Creates new reservation

---

**BODY PARAM:** 

`reservation` - `Reservation` Object with details of reservation

---

**AUTH:** Requires policy `CanReserveMedia`

</aside>

<aside>

`POST /reservations/cancel` 

---

**DESC:** Cancels an existing reservation

---

**BODY PARAM:**

`id` - `string` Reservation ID

---

**AUTH:** Requires policy `CanCancelMedia`

</aside>

<aside>

`POST /reservations/extend` 

---

**DESC:** Extends an existing Reservation

---

**BODY PARAM:**

`ReservationExtension` - `Reservation` Object contains `ReservationId` and `NewEndDate`

---

**AUTH:** Requires policy `CanExtendReservation`

</aside>

<aside>

`POST /reservations/getReservable` 

---

**DESC:** Get reservable items based on member branches and availability 

---

**BODY PARAM:**

`reservation` - `Reservation` Object

---

**AUTH:** Requires policy `CanReserveMedia`

</aside>

LoansController.cs

<aside>

`POST /loans/check-in` 

---

**DESC:** Check in a borrowed item by its physical media Id

---

**BODY PARAM:**

`physicalId` - `string` Physical media ID

---

**AUTH:** Requires policy `CanReserveMedia`

</aside>

<aside>

`POST /loans/check-out` 

---

**DESC:** Check out a borrowed item by its physical media Id

---

**BODY PARAM:**

`physicalId` - `string` Physical media ID

`memberId` - `string` Member ID

---

**AUTH:** Requires policy `CanReserveMedia`

</aside>

</aside>

<aside>

### **Metric Service**

---

MetricController.cs

<aside>

`GET /metric` 

---

**DESC:** Retrieves a snapshot of all docker containers

---

- **Example Response**
    
    ```json
    [
      {
        "containerId": "cc908095acba107845957dacd2d2a13afa69cdcbd68fb5cf092be725b25a19d8",
        "containerName": "amls_api-gateway-1",
        "cpuUsage": 0,
        "deltaCpuUsage": 0,
        "cpuPercentage": 0,
        "memoryUsage": 0,
        "memoryPercentage": 0,
        "memoryLimit": 0,
        "timestamp": "2024-12-17T13:50:30.010908Z"
      }
    ]
    ```
    

---

**AUTH:** Requires policy `CanViewMetricsReports`

</aside>

</aside>

<aside>

### **Media Service**

---

InventoryController.cs

<aside>

`GET /inventory` 

---

**DESC:** Retrieves paginated media entries including physical media data

---

**QUERY PARAM:**

`page` - `int` Page number

`count` - `int` Items per page

---

- **Example Response**
    
    ```json
    {
        "matchCount": 810,
        "data": {
            "physicalMediaList": [
                {
                    "objectID": "673f6d28e580ac7f9f4fa9b3",
                    "status": "Available",
                    "branch": "673348f4a1ed2d02eef46a41",
                    "mediaInfo": {
                        "id": "673f6cfae27bd6488d0ab1a0",
                        "title": "to kill a mockingbird",
                        "language": "English",
                        "description": "...",
                        "rating": 4.28,
                        "releaseDate": "2006-05-23T00:00:00Z",
                        "type": "Book",
                        "genres": [
                            "classics",
                            "fiction",
                            "historical fiction",
                            "school",
                            "literature",
                            "young adult",
                            "historical",
                            "novels",
                            "read for school",
                            "high school"
                        ],
                        "isbn": "1043321819600",
                        "author": "Harper Lee",
                        "publisher": "Harper",
                        "director": "",
                        "studio": "",
                        "creator": "",
                        "network": "",
                        "season": 0,
                        "episodes": 0,
                        "endDate": null,
                        "physicalCopies": []
                    },
                    "branchDetails": {
                        "objectID": "673348f4a1ed2d02eef46a41",
                        "name": "Hillside Public Library",
                        "address": {
                            "street": "14 Grove Avenue",
                            "town": "Manchester",
                            "postcode": "M2 5JD"
                        }
                    },
                    "reservations": []
                }
            ],
            "branchesList": [
                {
                    "objectID": "67236fc4b4d08aab049740ca",
                    "name": "Elm Park Library",
                    "address": {
                        "street": "15 Elm Street",
                        "town": "Birmingham",
                        "postcode": "B3 2AX"
                    }
                },
                {
                    "objectID": "673348f4a1ed2d02eef46a41",
                    "name": "Hillside Public Library",
                    "address": {
                        "street": "14 Grove Avenue",
                        "town": "Manchester",
                        "postcode": "M2 5JD"
                    }
                }
            ]
        },
        "success": true,
        "message": null,
        "statusCode": 200
    }
    ```
    

---

**AUTH:** Requires policy `CanViewInventory`

</aside>

<aside>

`POST /inventory/search` 

---

**DESC:** Retrieves paginated media entries including physical media data with specified search filters

---

**QUERY PARAM:**

`page` - `int` Page number

`count` - `int` Items per page

`filters` - List of `Filter` objects

---

- **Example Response**
    
    ```json
    {
      "matchCount": 1,
      "data": [
        {
          "objectId": "675c4f01f21f11238218b1c9",
          "status": "Available",
          "branch": "6733491ba1ed2d02eef46a42",
          "mediaInfo": {
            "objectId": "673f6d0ce27bd6488d0ab503",
            "title": "breaking bad: season 1",
            "language": "English",
            "description": "Season 1 follows Walter White, a struggling high school chemistry teacher who is diagnosed with terminal lung cancer. To secure his family's financial future, he turns to a life of crime, producing and selling methamphetamine.",
            "rating": 4.6,
            "releaseDate": "2008-01-20T00:00:00Z",
            "type": "TV Series",
            "genres": [
              "crime",
              "drama",
              "thriller",
              "suspense",
              "dark",
              "crime drama",
              "psychological",
              "contemporary",
              "family",
              "neo-western"
            ],
            "isbn": "",
            "author": "",
            "publisher": "",
            "director": "",
            "studio": "",
            "creator": "Vince Gilligan",
            "network": "AMC",
            "season": 1,
            "episodes": 7,
            "endDate": null,
            "physicalCopies": []
          },
          "branchDetails": {
            "objectId": "6733491ba1ed2d02eef46a42",
            "name": "Southgate Community Library",
            "address": {
              "street": "27 Market Square",
              "town": "London",
              "postcode": "N14 6DN"
            }
          },
          "reservations": []
        }
      ],
      "success": true,
      "message": null,
      "statusCode": 200
    }
    ```
    

---

**AUTH:** Requires policy `CanViewInventory`

</aside>

<aside>

`PUT /inventory/{branchId}/edit` 

---

**DESC:** Updates existing media item in specified branch

---

**ROUTE PARAM:**

`branchId` - `string` Branch ID

**BODY PARAM:**

`MediaInfo` - Object

---

- **Example Response**
    
    ```json
    {
      "data": null,
      "success": true,
      "message": "Media updated successfully",
      "statusCode": 200
    }
    ```

---

**AUTH:** Requires policy `CanEditMedia` and user requires branch access

</aside>

<aside>

`DELETE /inventory/{branchId}/delete`

---

**DESC:** Deletes existing media item in specified branch

---

**ROUTE PARAM:**

`branchId` - `string` Branch ID

**BODY PARAM:**

`MediaInfo` - Object


---

- **Example Response**
    
    ```json
    {
      "data": null,
      "success": true,
      "message": "Media updated successfully",
      "statusCode": 200
    }
    ```


---

**AUTH:** Requires policy `CanDeleteMedia` and user requires branch access

</aside>

---

CatalogController.cs

<aside>

`GET /catalog` 

---

**DESC:** Retrieves paginated media entries including branch availability

---

**QUERY PARAM:**

`page` - `int` Page number

`count` - `int` Items per page

---

- **Example Response**
    
    ```json
    {
      "matchCount": 882,
      "data": [
        {
          "objectId": "673f6cfae27bd6488d0ab19e",
          "title": "the hunger games",
          "language": "English",
          "description": "WINNING MEANS FAME AND FORTUNE.LOSING MEANS CERTAIN DEATH.THE HUNGER GAMES HAVE BEGUN. . . .In the ruins of a place once known as North America lies the nation of Panem, a shining Capitol surrounded by twelve outlying districts. The Capitol is harsh and cruel and keeps the districts in line by forcing them all to send one boy and once girl between the ages of twelve and eighteen to participate in the annual Hunger Games, a fight to the death on live TV.Sixteen-year-old Katniss Everdeen regards it as a death sentence when she steps forward to take her sister's place in the Games. But Katniss has been close to dead before—and survival, for her, is second nature. Without really meaning to, she becomes a contender. But if she is to win, she will have to start making choices that weight survival against humanity and life against love.",
          "rating": 4.33,
          "releaseDate": "2008-09-14T00:00:00Z",
          "type": "Book",
          "genres": [
            "young adult",
            "fiction",
            "dystopia",
            "fantasy",
            "science fiction",
            "romance",
            "adventure",
            "teen",
            "post apocalyptic",
            "action"
          ],
          "isbn": "9780439023481",
          "author": "Suzanne Collins",
          "publisher": "Scholastic",
          "director": "",
          "studio": "",
          "creator": "",
          "network": "",
          "season": 0,
          "episodes": 0,
          "endDate": null,
          "physicalCopies": [
            {
              "branch": "6733491ba1ed2d02eef46a42",
              "status": "Reserved"
            }
          ]
        }
      ],
      "success": true,
      "message": null,
      "statusCode": 200
    }
    ```
    
</aside>

<aside>

`POST /catalog/search`

---

**DESC:** Searches paginated media entries including branch availability with specified search filters

---

**QUERY PARAM:**

`page` - `int` Page number

`count` - `int` Items per page

`filters` - List of `Filter` objects

---

- **Example Response**
    
    ```json
    {
      "matchCount": 1,
      "data": [
        {
          "objectId": "673f6d0ce27bd6488d0ab503",
          "title": "breaking bad: season 1",
          "language": "English",
          "description": "Season 1 follows Walter White, a struggling high school chemistry teacher who is diagnosed with terminal lung cancer. To secure his family's financial future, he turns to a life of crime, producing and selling methamphetamine.",
          "rating": 4.6,
          "releaseDate": "2008-01-20T00:00:00Z",
          "type": "TV Series",
          "genres": [
            "crime",
            "drama",
            "thriller",
            "suspense",
            "dark",
            "crime drama",
            "psychological",
            "contemporary",
            "family",
            "neo-western"
          ],
          "isbn": "",
          "author": "",
          "publisher": "",
          "director": "",
          "studio": "",
          "creator": "Vince Gilligan",
          "network": "AMC",
          "season": 1,
          "episodes": 7,
          "endDate": null,
          "physicalCopies": [
            {
              "branch": "6733491ba1ed2d02eef46a42",
              "status": "Available"
            }
          ]
        }
      ],
      "success": true,
      "message": null,
      "statusCode": 200
    }
    ```
    
</aside>

</aside>