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
        /// <param name="requestPath">Request path.</param>
        /// <returns>Target URI string.</returns>
        string Map(string requestPath);
    }
}