# Image Service

A test assignment made for an employer in order to establish my skills<br/>
Position: C# Middle Backend Developer

### The following terms of reference were given:
Functional requirements
- Availability of user registration
- The user should be able to upload an image
- The user should be able to view their images
- The user should be able to view the images of another user if he is his friend
- User should be able to add another user to friends (User A can view User B's images if user B has added user A to friends, but user B cannot view User A's images if he has not reciprocated)

Non-functional requirements
- The application must be written in .NET 7, C# 11, ASP.NET Core
- Any type of authorization (ASP.NET Identity, you can store a login + password in the database (use basic auth)
- EntityFrameworkCore, a rich domain model, must be used for implementation
- In the essence of the user, the set of images should be a private field, only the property with IReadOnlyCollection should be accessible to the outside (you need to configure the EF configuration for this to work)
- The images themselves must be stored on local storage, the path must be configurable via appsettings.json, when transferring the storage location and the images themselves, the application must work correctly (do not store absolute paths in the database)
- The user-friends relationship (many-to-many on users) should be implemented through the EF configuration
- The application must have a restful API, return correct error codes (404, 401, 403, etc.)
- The application must have a swagger, with the ability to configure the selected authorization method via the UI
- The application must have all the endpoints to implement the functional requirements

### Usages

- [MiniIO](https://min.io/) For images storage
- [PostgreSQL](https://www.postgresql.org/) for database
- [Entity Framework Core](https://learn.microsoft.com/ru-ru/ef/core/) for object-relational mapping Module (O/RM)
- [Asp.Net.Core.Indetity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-7.0&tabs=visual-studio) + [Jwt Bearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer) For authentication
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) For integrate swagger


### Installation
You can examine docker-compose to see the entire configuration, but setting the following env param is enough for a basic run:
**MINIO_SERVER_URL**

Then run 
> docker-compose up