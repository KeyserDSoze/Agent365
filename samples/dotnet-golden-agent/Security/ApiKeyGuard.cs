using System.Security.Cryptography;
using System.Text;
using Agent365.GoldenAgent.Configuration;

namespace Agent365.GoldenAgent.Security;

public static class ApiKeyGuard
{
    public const string HeaderName = "X-Api-Key";

    public static bool IsAuthorized(HttpContext context, ApiOptions options)
    {
        if (!options.RequireApiKey)
        {
            return true;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var supplied))
        {
            return false;
        }

        var expectedBytes = Encoding.UTF8.GetBytes(options.ApiKey);
        var suppliedBytes = Encoding.UTF8.GetBytes(supplied.ToString());

        return expectedBytes.Length == suppliedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(expectedBytes, suppliedBytes);
    }
}
