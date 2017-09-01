using System.Collections.Generic;

namespace MicroNetCore.AspNetCore.Proxy
{
    /// <summary>
    ///     Represents a collection of keys and values, where a key is a HTTP request absolute path, and a value is target
    ///     destination for redirecting the request.
    /// </summary>
    public sealed class ProxyOptions : Dictionary<string, string>
    {
    }
}