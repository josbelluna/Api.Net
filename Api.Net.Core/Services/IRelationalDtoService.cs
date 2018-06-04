using System;

namespace Api.Net.Core.Services
{
    public interface IRelationalDtoService
    {
        Tuple<string, string> TranslateRoute(string url);
    }
}