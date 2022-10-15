using System.Threading.RateLimiting;
using Polly;

namespace DotNet7.Polly.RateLimit;

public abstract partial class DotNet7Policy
{
    public static AsyncDotNet7RateLimitPolicy<TResult> FixedWindowRateLimitAsync<TResult>(
        FixedWindowRateLimiterOptions options,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null)
    {
        RateLimiter rateLimiter = new FixedWindowRateLimiter(options);

        return new AsyncDotNet7RateLimitPolicy<TResult>(rateLimiter, retryAfterFactory);
    }
}

