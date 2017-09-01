using System;
using System.Collections.Generic;

namespace MicroNetCore.AspNetCore.Proxy
{
    /// <summary>
    ///     Default implementation of <see cref="T:MicroNetCore.AspNetCore.Proxy.IProxy" />.
    /// </summary>
    public sealed class Proxy : IProxy
    {
        private readonly ProxyOptions _proxyOptions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:MicroNetCore.AspNetCore.Proxy.Proxy" /> class.
        /// </summary>
        /// <param name="proxyOptions">
        ///     <see cref="T:MicroNetCore.AspNetCore.Proxy.ProxyOptions" /> for resolving target
        ///     destinations.
        /// </param>
        public Proxy(ProxyOptions proxyOptions)
        {
            _proxyOptions = proxyOptions;
        }

        /// <summary>
        ///     Maps request URI to target URI.
        /// </summary>
        /// <param name="uri">Request <see cref="T:System.Runtime.Serialization.Uri" />.</param>
        /// <returns>Target <see cref="T:System.Runtime.Serialization.Uri" />.</returns>
        public Uri Map(Uri uri)
        {
            var path = uri.AbsolutePath.ToLower();
            var pathAndQuery = uri.PathAndQuery.ToLower();

            foreach (var key in _proxyOptions.Keys)
            {
                if (!Match(path, key)) continue;

                return new Uri(pathAndQuery.Replace(key.ToLower(), _proxyOptions[key].ToLower()));
            }

            throw new KeyNotFoundException();
        }

        private static bool Match(string uri, string pattern)
        {
            if (string.Equals(uri, pattern, StringComparison.OrdinalIgnoreCase))
                return true;

            if (uri.StartsWith(pattern, StringComparison.CurrentCultureIgnoreCase) && uri[pattern.Length] == '/')
                return true;

            return false;
        }
    }
}