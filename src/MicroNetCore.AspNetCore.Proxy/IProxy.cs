using System;

namespace MicroNetCore.AspNetCore.Proxy
{
    /// <summary>
    ///     Maps request URI to target URI.
    /// </summary>
    public interface IProxy
    {
        /// <summary>
        ///     Maps request URI to target URI.
        /// </summary>
        /// <param name="uri">Request <see cref="T:System.Runtime.Serialization.Uri" />.</param>
        /// <returns>Target <see cref="T:System.Runtime.Serialization.Uri" />.</returns>
        Uri Map(Uri uri);
    }
}