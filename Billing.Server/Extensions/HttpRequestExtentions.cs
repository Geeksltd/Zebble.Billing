namespace Zebble.Billing
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Olive;

    public static class HttpRequestExtentions
    {
        public static bool Matches(this HttpRequest request, Uri absoluteUri)
        {
            return request.Path.StartsWithSegments(absoluteUri.AbsolutePath);
        }

        public static bool IsGet(this HttpRequest request) => request.MethodIs("GET");

        public static bool IsPost(this HttpRequest request) => request.MethodIs("POST");

        static bool MethodIs(this HttpRequest request, string verb)
        {
            return request.Method.Equals(verb, caseSensitive: false);
        }
    }
}
