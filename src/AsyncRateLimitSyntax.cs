using System.Threading.RateLimiting;
using Polly;
namespace DotNet7.Polly.RateLimit;

public abstract partial class DotNet7Policy
{
    /// <summary>
    /// Build a RateLimit <see cref="AsyncPolicy"/> that will rate-limit executions based on the <see cref="FixedWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="FixedWindowRateLimiter"/>.</param>
    /// <returns>The policy instance.</returns>
    public static AsyncDotNet7RateLimitPolicy FixedWindowRateLimitAsync(FixedWindowRateLimiterOptions options)
    {
        RateLimiter rateLimiter = new FixedWindowRateLimiter(options);

        return new AsyncDotNet7RateLimitPolicy(rateLimiter);
    }

    /// <summary>
    /// Build a RateLimit <see cref="AsyncPolicy"/> that will rate-limit executions based on the <see cref="SlidingWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="SlidingWindowRateLimiter"/>.</param>
    /// <returns>The policy instance.</returns>
    public static AsyncDotNet7RateLimitPolicy SlidingWindowRateLimitAsync(SlidingWindowRateLimiterOptions options)
    {
        RateLimiter rateLimiter = new SlidingWindowRateLimiter(options);

        return new AsyncDotNet7RateLimitPolicy(rateLimiter);
    }

    /// <summary>
    /// Build a RateLimit <see cref="AsyncPolicy"/> that will rate-limit executions based on the <see cref="TokenBucketRateLimiter" /> rate limiter.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="TokenBucketRateLimiter"/>.</param>
    /// <returns>The policy instance.</returns>
    public static AsyncDotNet7RateLimitPolicy TokenBucketRateLimitAsync(TokenBucketRateLimiterOptions options)
    {
        RateLimiter rateLimiter = new TokenBucketRateLimiter(options);

        return new AsyncDotNet7RateLimitPolicy(rateLimiter);
    }

    /// <summary>
    /// Build a RateLimit <see cref="AsyncPolicy"/> that will rate-limit executions based on the <see cref="ConcurrencyLimiter" /> rate limiter.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="ConcurrencyLimiter"/>.</param>
    /// <returns>The policy instance.</returns>
    public static AsyncDotNet7RateLimitPolicy ConcurrencyRateLimitAsync(ConcurrencyLimiterOptions options)
    {
        RateLimiter rateLimiter = new ConcurrencyLimiter(options);

        return new AsyncDotNet7RateLimitPolicy(rateLimiter);
    }
}

