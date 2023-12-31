using Microsoft.AspNetCore.Http;

namespace Common.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetBaseUrl(this HttpRequest request)
        {
            var host = request.Host.ToUriComponent();
            var pathBase = request.PathBase.ToUriComponent();

            return $"{request.Scheme}://{host}{pathBase}";
        }
    }
}
