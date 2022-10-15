using System.Threading.RateLimiting;
using Polly;

namespace DotNet7.Polly.RateLimit;

public abstract partial class DotNet7Policy
{
    public static DotNet7RateLimitPolicy<TResult> FixedWindowRateLimit<TResult>(
        FixedWindowRateLimiterOptions options,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null)
    {
        RateLimiter rateLimiter = new FixedWindowRateLimiter(options);

        return new DotNet7RateLimitPolicy<TResult>(rateLimiter, retryAfterFactory);
    }
}

