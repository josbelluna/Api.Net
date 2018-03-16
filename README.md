# Api.Net

[![NuGet](http://img.shields.io/nuget/v/Api.Net.svg)](https://www.nuget.org/packages/Api.Net/)

### What is `Api.Net`?
Api.Net is a simple implementation of web api using the Repository/Service/Dto patterns and based on Asp Net Core Mvc. 

# Table of Contents
1. [Getting Started](#get-started)
2. [Rest Urls](#rest)
3. [Configuration](#configuration)
4. [Working With Dtos](#dtos)
5. [Filtering Data](#filtering)
6. [Ordering Data](#ordering)
7. [Paginating Data](#pagination)
8. [Showing/Hidding Fields](#fields)
9. [Undertanding Api.Net Architecture](#architecture)
10. [Events](#events)
11. [Validation](#validations)
12. [Custom Repositories](#custom-repositories)
13. [Custom Services](#custom-services)
14. [Custom Controllers](#custom-controllers)
15. [Working with Dependency Injection](#injection)



### 1. <a id="get-started" /> How do I get started? 
##### a. In your startup.cs add
```csharp
public class Startup
{        
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc().AddApi();
    }
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseMvcWithDefaultRoute();         
    }
}
```
##### b. Create DbContext in TestContext.cs
```csharp
public partial class TestContext : DbContext
{
    public TestContext() : base("YourConnectionStringHere") {}
    public virtual DbSet<Article> Articles { get; set; }
    public virtual DbSet<Author> Authors { get; set; }
}
```
##### c. Create Dto in AuthorDto.cs as 
```csharp
public class AuthorDto : Dto<AuthorDto, Author>
{
    public string Name { get; set; }
}
```
##### d. Populate some data in database, then browse type /api/author 
```json
{
    "count":1,
    "data":
    [
        {"name":"Josbel Luna"}
    ]
}
```
### 2. <a id="rest" />  Rest urls 

Api.Net creates the following endpoints for each dto:
```csharp
* GET /api/author //get all authors
* GET /api/author/1 //get author with id 1
* POST /api/author //insert new author
* PUT /api/author/1 //update the author with id 1
* PATCH /api/author/1 //partial update the author with id 1
* DELETE /api/author/1 //delete the author with id 1
```
And they will return the following status codes
```csharp
* 200 //OK
* 400 //Bad Request
* 500 //Internal Server Error
```
### 3. <a id="configuration" />  Configuration
#### a. Changing default route prefix 
By default Api.Net use /api for all routes, to change this in Startup.cs
```csharp
public class Startup
{        
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc().AddApi(opt => opt.RoutePrefix = "api/myblog");
    }  
    ...
}
```
Now author endpoint will be in /api/myblog/author 
```json
{
    "count":1,
    "data":
    [
        {"name":"Josbel Luna"}
    ]
}
```
#### b. Changing endpoint name 
The convention to resolve the name of the endpoint is the class name without the suffix Dto. To change this behaviour just add the attribute ApiEndpoint in your AuthorDto.cs as
```csharp
[ApiEndpoint("authors")]
public class AuthorDto : Dto<AuthorDto, Author>
{
    public string Name { get; set; }
}
```
And the result endpoint api/myblog/authors will bring
```json
{
    "count":1,
    "data":
    [
        {"name":"Josbel Luna"}
    ]
}
```

### 4. <a id="dtos" /> Working with Dtos
#### a. Mapping dtos
Api.Net mappings are entirely based on Autommaper. They allow us to create very complex mapping an conventions to facilitate our work. By convention all the properties in the dto will map the properties in the entity if they has the same name so for example:
```csharp
public partial class Author
{
    public Author()
    {
        Article = new HashSet<Article>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public DateTime JoinDate { get; set; }
    public DateTime LastUpdate { get; set; }

    public ICollection<Article> Articles { get; set; }
}

[ApiEndpoint("authors")]
public class AuthorDto : Dto<AuthorDto, Author>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
}
```
The properties Id, Name and LastName will be mapped by convention, for the FullName property you must map manually by overriding the Map method: 
```csharp
public class AuthorDto : Dto<AuthorDto, Author>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public override void Map(IMap<AuthorDto, Author> mapper)
    {
        mapper.Map(t => t.FullName, t => t.Name + " " + t.LastName);
    }

}
```
Making a the request:
`GET /api/myblog/authors`

Will return 
```json
{
    "count": 1,
    "data": 
    [
        {"id": 2,"name": "Josbel","lastName": "Luna","fullName": "Josbel Luna"}
    ]
}
```
### 5. <a id="filtering" /> Filtering data
Aumming `GET api/myblog/authors` returns the following data
```json
{
    "count":3,
    "data":
    [
        {"id":2,"name":"Josbel","lastName":"Luna","fullName":"Josbel Luna"},
        {"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"},
        {"id":6,"name":"Carl","lastName":"Johnson","fullName":"Carl Johnson"}
    ]
}
```
We could filter the data with:
##### a. Equality
To find and exact value just write in the querystring the name of the field + = + the value you want to match like:

`GET /api/myblog/authors?name=john`
```json
{
    "count":1,
    "data":
    [
        {"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"}
    ]
}
```
##### b. More or less
you could query this with the following statements
* `/api/myblog/authors?id>=2 //More than 2`
* `/api/myblog/authors?id>==2 //More or equal to 2`
* `/api/myblog/authors?id<=2 //Less than 2`
* `/api/myblog/authors?id<==2 //Less or equal to 2`

`GET /api/myblog/authors?id>=2 //More than 2` will return
```json
{
    "count":2,
    "data":
    [
        {"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"},
        {"id":6,"name":"Carl","lastName":"Johnson","fullName":"Carl Johnson"}
    ]
}
```
#### c. Alternation (Or)
Just separate the statements by `,` as follows:

`GET /api/myblog/authors?id=2,6`
```json
{
    "count":2,
    "data":
    [
        {"id":2,"name":"Josbel","lastName":"Luna","fullName":"Josbel Luna"},
        {"id":6,"name":"Carl","lastName":"Johnson","fullName":"Carl Johnson"}
    ]
}
```
#### d. Between
Just separate the statements by `|` as follows:
`GET /api/myblog/authors?id=1|5`
```json
{
    "count":2,
    "data":
    [
        {"id":2,"name":"Josbel","lastName":"Luna","fullName":"Josbel Luna"},
        {"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"}
    ]
}
```
#### e. Match Patterns
- Use the `$` character as:
* `Value$$ //Start with value`
* `$$Value //End with value`
* `$Value$ //Contains value`

`GET /api/myblog/authors?fullName=John$$`
```json
{
    "count":1,
    "data":
    [
        {"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"}
    ]
}
```
`GET /api/myblog/authors?fullName=$$Luna`
```json
{
    "count":1,
    "data":
    [
        {"id":2,"name":"Josbel","lastName":"Luna","fullName":"Josbel Luna"}
    ]
}
```
`GET /api/myblog/authors?fullName=$John$`
```json
{
    "count":2,
    "data":
    [
        {"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"},
        {"id":6,"name":"Carl","lastName":"Johnson","fullName":"Carl Johnson"}
    ]
}
```
- Use the `*` character as space pattern:
* `fullName=*ValueA ValueB //Equivalent to fullname=$ValueA$&fullName=$ValueB$`

`GET /api/myblog/authors?fullName=*Jo Lu`
```json
{
    "count":1,
    "data":
    [
        {"id":2,"name":"Josbel","lastName":"Luna","fullName":"Josbel Luna"}
    ]
}
```
#### f. Inner properties

The next dto 
```csharp
[ApiEndpoint("articles")]
public class ArticleDto : Dto<ArticleDto, Article>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int AuthorId { get; set; }
    public AuthorDto Author { get; set; }
}
```
Will expose

`GET /api/myblog/articles`
```json
{
    "count":3,
    "data":
    [
        {"id":2,"name":".Net Core 2.0 New Features","authorId":2,"author":{"id":2,"name":"Josbel","lastName":"Luna","fullName":"Josbel Luna"}},
        {"id":3,"name":"Big data in 2018","authorId":2,"author":{"id":2,"name":"Josbel","lastName":"Luna","fullName":"Josbel Luna"}},
        {"id":7,"name":"Managing concurrency in databases","authorId":5,"author":{"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"}}
    ]
}
```
The all filters explained before can be applied to inner properties just adding `.`  as for example

`GET /api/myblog/articles?author.fullName=*John`
```json
{"count":1,"data":[{"id":7,"name":"Managing concurrency in databases","authorId":5,"author":{"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"}}]}
```

### 6. <a id="ordering"/> Ordering Data 
To select fields to order you must specified that in `orderdy` param as

`Get /api/myblog/authors?orderby=name`
```json
{
    "count":3,
    "data":
    [
        {"id":6, "name":"Carl", "lastName":"Johnson", "fullName":"Carl Johnson"},
        {"id":5, "name":"John", "lastName":"Doe", "fullName":"John Doe"},
        {"id":2, "name":"Josbel", "lastName":"Luna", "fullName":"Josbel Luna"}
    ]
}
```

and for decending order just add another param as `decending=true` as

`Get /api/myblog/authors?orderby=name&descending=true`

```json
{
    "count":3,
    "data":
    [
        {"id":2, "name":"Josbel", "lastName":"Luna", "fullName":"Josbel Luna"},
        {"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"},
        {"id":6,"name":"Carl","lastName":"Johnson","fullName":"Carl Johnson"}
    ]
}
```
### 7. <a id="paginating"/> Paginating Data 
Api.Net provides a pagination mechanism by just adding two params `page` and `pagesize`, by default the page is always `1` and pagesize is `0` witch means all. making a request to the next endpoint

`GET /api/myblog/authors?pagesize=1`
will return
```json
{
    "count":3,
    "data":
    [
        {"id":2,"name":"Josbel", "lastName":"Luna", "fullName":"Josbel Luna"}
    ]
}
```
Notice at the count metadata attribute, it shows `3` meaning you are not filtering the data but paginating it, to access to the next page just make

`GET /api/myblog/authors?pagesize=1&page=2`
will return
```json
{
    "count":3,
    "data":
    [
        {"id":5,"name":"John", "lastName":"Doe", "fullName":"John Doe"}
    ]
}
```
And you could continue doing the same until consuming all the data.
### 8. <a id="fields"/> Showing/Hinding fields
Sometimes you need just an specific part of the dto instead of bringin the entire object, for example populating a select box when you must of the times just need the id and name fields.

You could create another dto with the 2 properties but for that you have to make another endpoint for each new dto. Api.Net provides instead the the fields param witch allows to specify the fields you want to bring to you. You could request as

`GET /api/myblog/authors?fields=id,name`

```json
{
    "count":3,
    "data":
    [
        {"id":2,"name":"Josbel"},
        {"id":5,"name":"John"},
        {"id":6,"name":"Carl"}
    ]
}
```

This aims to reduce the resources you consuming sending/receiving and storing data and the consult to the database is impacted too reducing the fields you're selecting. This provides to you a safe endpoint to avoid consults getting slowed when you increment the properties used in the dto.

You could hide instead show fields by use the `exclude` param

`GET /api/myblog/authors?exclude=name,lastname`
```json
{
    "count":3,
    "data":
    [
        {"id":2,"fullName":"Josbel Luna"},
        {"id":5,"fullName":"John Doe"},
        {"id":6,"fullName":"Carl Johnson"}
    ]
}
```
### 9. <a id="events"/> Events in the Dto
Api.Net provide a serveral events in their Dto implementation, these events are:
```csharp
* BeforeGet
* BeforeInsert
* BeforeUpdate
* BeforeSave
* AfterInsert
* AfterUpdate
* AfterSave
```
The before save event is triggered when insert and update an entity and you can use it as follows in your AuthorDto.cs:
```csharp
[ApiEndpoint("authors")]
public class AuthorDto : Dto<AuthorDto, Author>
{
    public string Name { get; set; }
    public DateTime JoinDate { get; set; }
    public DateTime LastUpdate { get; set; }

    public override void BeforeInsert(AuthorDto dto)
    {
        dto.JoinDate = DateTime.Now;
    }
    public override void BeforeSave(AuthorDto dto)
    {
        dto.LastUpdate = DateTime.Now;
    }
}
```
Later you'll see you could use events at service layer.
### 10. <a id="validations"/> Validations in the Dto
You could use dto events to actually validate the endpoint and throwing a ```ValidateException``` inside of then will result status code bad request with te message of the error as follows: 
```csharp
[ApiEndpoint("authors")]
public class AuthorDto : Dto<AuthorDto, Author>
{
    public string Name { get; set; }
    public DateTime JoinDate { get; set; }
    public DateTime LastUpdate { get; set; }

    public override void BeforeSave(AuthorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ValidateException("The name is required");
    }
}
```

```
POST /api/myblog/authors 
{}

will result BAD REQUEST(400)
The name is required
```

However Api.Net provides a mechanism to manage this and multiple errors too just overriding these methods:
```csharp
* ValidateInsert
* ValidateUpdate
* ValidateSave
```
Agains events these provides a second parameter who acts as an error register, working as follow:

```csharp
[ApiEndpoint("authors")]
public class AuthorDto : Dto<AuthorDto, Author>
{
    public int Id { get; set; }

    public string Name { get; set; }
    public DateTime JoinDate { get; set; }
    public DateTime LastUpdate { get; set; }

    public override void ValidateUpdate(AuthorDto dto, Error error)
    {
        if (Id <= 0)
            error.Add("Cannot find the specified author");
    }
    public override void ValidateSave(AuthorDto dto, Error error)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            error.Add("The name is required");
    }
}
```

There are some kind of validation witch needs to stop the validation flow, for that cases you could call the specific method in ```Error``` class as for example:

``` csharp
public override void ValidateSave(AuthorDto dto, Error error)
{
    if(dto == null)
    {
        error.Add("Invalid author");
        error.Validate();
    }

    if (string.IsNullOrWhiteSpace(dto.Name))
        error.Add("The name is required");
}
```
This method will throw the ```ValidateException``` and stop the validation avoiding for example a possible ```NullReferenceException``` in the next lines.

Be aware about using complex validation inside the dtos, for that cases you could use validation in services instead.

### 11. <a id="architecture"/> Understanding Api.Net Architecture

To undertand how Api.Net works you must undertand the following diagram

![Architecture](./Api.net/docs/images/architecture.jpg)

In words it means

* Context is the data provider, they expose entities to the repositories.
* The data access logic (such as find, where, etc) must be in the repositories exposing a single entity.
* The bussiness logic must be in the service, it use one or multiple repositories to process this logic and expose dtos to the controller.
* The controller routes the data and manages one or more services to complete his work. They expose the serialized dto to the client.

You could access to any of these components throw constructor service injection used in asp net core. By default, as you see, you just need to add the dto, all the other missing parts are provided by Api.Net as generic repositories, services and controllers. The next topics you'll know how to change these generics to specific ones.

### 12. <a id="custom-repositories"/> Custom Repositories

To change the generic repository attached with an entity, you must create a class and inherit from the `Repository` base class, and use the `[ApiRepository]` attribute as shown in `AuthorRepository.cs`
```csharp
[ApiRepository]
public class AuthorRepository : Repository<TestContext, Author>
{
    public override Author Find(object key)
    {
        var author = base.Find(key);
        author.Name +=  " Intercepted by Repository";
        return author;
    }
}
```
 `GET /api/myblog/authors/2` 
 will return  
 ```json 
{
"id": 2,
"name": "Josbel Intercepted by Repository",
"lastName": "Luna",
"fullName": "Josbel Intercepted by Repository Luna"
}
 ```
Another way to override the generic repository is using the interface   `IRepository` instead of the base class, for example:
```csharp
[ApiRepository]
public class AuthorRepository : IRepository<Author>{
    ...
}
```
In this case you have to provide the implementation of the interface manually.

### 13. <a id="custom-services"/> Custom Services
The same way as repositories, services can be overrided using `Service` base class and using `[ApiService]` attribute
``` csharp
[ApiService]
public class AuthorService : Service<AuthorDto, Author>
{
    public AuthorService(IRepository<Author> repository) : base(repository)
    {
    }

    public override AuthorDto Find(object key)
    {
        var dto = base.Find(key);
        dto.LastName += " Intercepted by Service";
        return dto;
    }
}
```
Notice you have to inject the repository by controller, this IRepository interface is the generic Repository supplied by the Api or a custom repository as we shown in the last example.

 `GET /api/myblog/authors/2` 
 will return  
 ```json 
{
"id": 2,
"name": "Josbel Intercepted by Repository",
"lastName": "Luna Intercepted by Service",
"fullName": "Josbel Intercepted by Repository Luna"
}
 ```
As the repositories you could use `IService<>` interface instead the class getting the same result.
### 14. <a id="custom-controllers"/> Custom Controllers
To use a custom controller just inherit from ApiController class as follows:

```csharp
[Route("api/[controller]")]
public class AuthorsController : ApiController<AuthorDto>
{
    public override IActionResult Find(int id)
    {
        var author = Service.Find(id);
        author.FullName += " Intercepted by Controller";
        return Ok(author);
    }
}
```
`GET /api/authors` will return
```json
{
"id": 2,
"name": "Josbel Intercepted by Repository",
"lastName": "Luna Intercepted by Service",
"fullName": "Josbel Intercepted by Repository Luna Intercepted by Controller"
}
```
### 15. <a id="injection"/> Working with Dependency Injection
As we see last time we have  some constructor dependency injection like this
```csharp
 public AuthorService(IRepository<Author> repository)
 ```

 Api.Net Configure all the interfaces as scoped services if they have an `ApiService` or `ApiRepository` attribute on the implementation. Based on that you could use these attributes to inject some other services you want to use, be or not a Dto service.

 For example we have 
```csharp
[ApiService]
public class StringService : IStringService
{
    public string Capitalize(string word)
    {
        if (string.IsNullOrEmpty(word)) return word;
        return char.ToUpper(word[0]) + word.Substring(1).ToLower();
    }
}
```
and 
```csharp
public interface IStringService
{
    string Capitalize(string word);
}
```

you could use this Istringservice into a controller, service or repository by just injected by constructor as the following example:
```csharp
[Route("api/[controller]")]
public class AuthorsController : ApiController<AuthorDto>
{
    private readonly IStringService _stringService;

    public AuthorsController(IStringService stringService)
    {
        _stringService = stringService;
    }

    public override IActionResult Find(int id)
    {
        var author = Service.Find(id);
        author.FullName += " Intercepted by Controller";

        author.Name = _stringService.Capitalize(author.Name);
        author.LastName = _stringService.Capitalize(author.LastName);
        author.FullName = _stringService.Capitalize(author.FullName);
        return Ok(author);
    }
}
```
`GET /api/authors/2` now returns
```json
{
"id": 2,
"name": "Josbel intercepted by repository",
"lastName": "Luna intercepted by service",
"fullName": "Josbel intercepted by repository luna intercepted by controller"
}
```