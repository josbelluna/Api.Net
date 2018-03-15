# Api.Net

[![NuGet](http://img.shields.io/nuget/v/Api.Net.svg)](https://www.nuget.org/packages/Api.Net/)

### What is `Api.Net`?
Api.Net is a simple implementation of web api using the Repository/Service/Dto patterns and based on Asp Net Core Mvc. 

# Table of Contents
1. [Getting Started](#get-started)
2. [Example2](#example2)
3. [Third Example](#third-example)



### <a id="get-started" /> How do I get started? 
##### 1. In your startup.cs add
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
##### 2. Create DbContext in TestContext.cs
```csharp
public partial class TestContext : DbContext
{
    public TestContext() : base("YourConnectionStringHere") {}
    public virtual DbSet<Article> Articles { get; set; }
    public virtual DbSet<Author> Authors { get; set; }
}
```
##### 3. Create Dto in AuthorDto.cs as 
```csharp
public class AuthorDto : Dto<AuthorDto, Author>
{
    public string Name { get; set; }
}
```
##### 4. Populate some data in database, then browse type /api/author 
```json
{"count":1,"data":[{"name":"Josbel Luna"}]}
```
### Rest urls 

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

### Changing default route prefix 
By default Api.Net use /api for all routes, to change this in Startup.cs
```csharp
public class Startup
{        
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc().AddApi(opt => opt.RoutePrefix = "api/myblog");
    }  
}
```
Now author endpoint will be in /api/myblog/author 
```json
{"count":1,"data":[{"name":"Josbel Luna"}]}
```
### Changing endpoint name 
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
{"count":1,"data":[{"name":"Josbel Luna"}]}
```
### Mapping dtos
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
{"count": 1,"data": [{"id": 2,"name": "Josbel","lastName": "Luna","fullName": "Josbel Luna"}]}
```
### Filtering and ordering and paginating data
Aumming `GET api/myblog/authors` returns the following data
```json
{"count":3,"data":[{"id":2,"name":"Josbel","lastName":"Luna","fullName":"Josbel Luna"},{"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"},{"id":6,"name":"Carl","lastName":"Johnson","fullName":"Carl Johnson"}]}
```
We could filter the data with:
##### Equality
To find and exact value just write in the querystring the name of the field + = + the value you want to match like:

`GET /api/myblog/authors?name=john`
```json
{"count":1,"data":[{"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"}]}
```
##### More or less
you could query this with the following statements
`/api/myblog/authors?id>=2 //More than 2`
`/api/myblog/authors?id>==2 //More or equal to 2`
`/api/myblog/authors?id<=2 //Less than 2`
`/api/myblog/authors?id<==2 //Less or equial to 2`

`GET /api/myblog/authors?id>=2 //More than 2` will return
```json
{"count":2,"data":[{"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"},{"id":6,"name":"Carl","lastName":"Johnson","fullName":"Carl Johnson"}]}
```
#### Alternation (Or)
Just separate the statements by `,` as follows:

`GET /api/myblog/authors?id=2,6`
```json
{"count":2,"data":[{"id":2,"name":"Josbel","lastName":"Luna","fullName":"Josbel Luna"},{"id":6,"name":"Carl","lastName":"Johnson","fullName":"Carl Johnson"}]}
```
#### Between
Just separate the statements by `|` as follows:
`GET /api/myblog/authors?id=1|5`
```json
{"count":2,"data":[{"id":2,"name":"Josbel","lastName":"Luna","fullName":"Josbel Luna"},{"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"}]}
```
#### Match Patterns
* Use the `$` character as:
`Value$$ //Start with value`
`$$Value //End with value`
`$Value$ //Contains value`

`GET /api/myblog/authors?fullName=John$$`
```json
{"count":1,"data":[{"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"}]}
```
`GET /api/myblog/authors?fullName=$$Luna`
```json
{"count":1,"data":[{"id":2,"name":"Josbel","lastName":"Luna","fullName":"Josbel Luna"}]}
```
`GET /api/myblog/authors?fullName=$John$`
```json
{"count":2,"data":[{"id":5,"name":"John","lastName":"Doe","fullName":"John Doe"},{"id":6,"name":"Carl","lastName":"Johnson","fullName":"Carl Johnson"}]}
```
* Use the `*` character as space pattern:
`fullName=*ValueA ValueB //Equivalent to fullname=$ValueA$&fullName=$ValueB$`

`GET /api/myblog/authors?fullName=*Jo Lu`
```json
{"count":1,"data":[{"id":2,"name":"Josbel","lastName":"Luna","fullName":"Josbel Luna"}]}
```

### Events in the Dto
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
### Validations in the Dto
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

Be aware about using complex validation inside the dtos, for that cases you could use Service Validation instead.

### Understanding Api.Net Architecture

To undertand how Api.Net works you must undertand the following diagram

![Architecture](./Api.net/docs/images/architecture.jpg)

In words it means

* Context is the data provider, they expose entities to the repositories.
* The data access logic (such as find, where, etc) must be in the repositories exposing a single entity.
* The bussiness logic must be in the service, it use one or multiple repositories to process this logic and expose dtos to the controller.
* The controller routes the data and manages one or more services to complete his work. They expose the serialized dto to the client.
