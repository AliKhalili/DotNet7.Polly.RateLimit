using System.Threading.RateLimiting;

namespace DotNet7.Polly.RateLimit;

public abstract partial class DotNet7Policy
{
    public static AsyncDotNet7RateLimitPolicy FixedWindowRateLimitAsync(FixedWindowRateLimiterOptions options)
    {
        RateLimiter rateLimiter = new FixedWindowRateLimiter(options);

        return new AsyncDotNet7RateLimitPolicy(rateLimiter);
    }
}

