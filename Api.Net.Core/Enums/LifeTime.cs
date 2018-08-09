using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Enums
{
    //
    // Summary:
    //     Specifies the lifetime of a service in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    public enum LifeTime
    {
        //
        // Summary:
        //     Specifies that a single instance of the service will be created.
        Singleton = 0,
        //
        // Summary:
        //     Specifies that a new instance of the service will be created for each scope.
        //
        // Remarks:
        //     In ASP.NET Core applications a scope is created around each server request.
        Scoped = 1,
        //
        // Summary:
        //     Specifies that a new instance of the service will be created every time it is
        //     requested.
        Transient = 2
    }
}
