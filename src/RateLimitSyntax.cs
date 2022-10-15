using System.Threading.RateLimiting;
using Polly;
namespace DotNet7.Polly.RateLimit;

public abstract partial class DotNet7Policy
{
    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="FixedWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="FixedWindowRateLimiter"/>.</param>
    /// <returns>The policy instance.</returns>
    public static DotNet7RateLimitPolicy FixedWindowRateLimit(FixedWindowRateLimiterOptions options)
    {
        RateLimiter rateLimiter = new FixedWindowRateLimiter(options);

        return new DotNet7RateLimitPolicy(rateLimiter);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="SlidingWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="SlidingWindowRateLimiter"/>.</param>
    /// <returns>The policy instance.</returns>
    public static DotNet7RateLimitPolicy SlidingWindowRateLimit(SlidingWindowRateLimiterOptions options)
    {
        RateLimiter rateLimiter = new SlidingWindowRateLimiter(options);

        return new DotNet7RateLimitPolicy(rateLimiter);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="TokenBucketRateLimiter" /> rate limiter.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="TokenBucketRateLimiter"/>.</param>
    /// <returns>The policy instance.</returns>
    public static DotNet7RateLimitPolicy TokenBucketRateLimit(TokenBucketRateLimiterOptions options)
    {
        RateLimiter rateLimiter = new TokenBucketRateLimiter(options);

        return new DotNet7RateLimitPolicy(rateLimiter);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="ConcurrencyLimiter" /> rate limiter.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="ConcurrencyLimiter"/>.</param>
    /// <returns>The policy instance.</returns>
    public static DotNet7RateLimitPolicy ConcurrencyRateLimit(ConcurrencyLimiterOptions options)
    {
        RateLimiter rateLimiter = new ConcurrencyLimiter(options);

        return new DotNet7RateLimitPolicy(rateLimiter);
    }
}

