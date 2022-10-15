using System.Threading.RateLimiting;

namespace DotNet7.Polly.RateLimit;

public abstract partial class DotNet7Policy
{
    public static DotNet7RateLimitPolicy FixedWindowRateLimit(FixedWindowRateLimiterOptions options)
    {
        RateLimiter rateLimiter = new FixedWindowRateLimiter(options);

        return new DotNet7RateLimitPolicy(rateLimiter);
    }
}

