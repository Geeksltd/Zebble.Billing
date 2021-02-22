namespace Zebble.Billing
{
    using System;
    using Microsoft.AspNetCore.Http;

    public static class HttpContextExtentions
    {
        public static Uri ToAbsolute(this IHttpContextAccessor contextAccessor, Uri relativeUri)
        {
            var request = contextAccessor.HttpContext.Request;
            return new Uri(new Uri($"{request.Scheme}://{request.Host}"), relativeUri);
        }
    }
}
