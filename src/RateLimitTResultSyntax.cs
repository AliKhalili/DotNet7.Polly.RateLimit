using System.Threading.RateLimiting;
using Polly;
using Polly.RateLimit;

namespace DotNet7.Polly.RateLimit;

public abstract partial class DotNet7Policy
{
    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="FixedWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    /// <param name="options">Options to specify the behavior of the <see cref="FixedWindowRateLimiter"/>.</param>
    /// <param name="tryReplenishment"> Attempts to replenish permits.
    /// <param name="retryAfterFactory">An (optional) factory to use to express retry-after back to the caller, when an operation is rate-limited.
    /// <remarks>If null, a <see cref="RateLimitRejectedException"/> with property <see cref="RateLimitRejectedException.RetryAfter"/> will be thrown to indicate rate-limiting.</remarks></param>
    /// <returns></returns>
    public static DotNet7RateLimitPolicy<TResult> FixedWindowRateLimit<TResult>(
        FixedWindowRateLimiterOptions options,
        TryReplenishment? tryReplenishment = null,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null!)
    {
        ReplenishingRateLimiter rateLimiter = new FixedWindowRateLimiter(options);
        if (tryReplenishment is not null)
        {
            tryReplenishment += () => { return rateLimiter.TryReplenish(); };
        }
        return new DotNet7RateLimitPolicy<TResult>(rateLimiter, retryAfterFactory);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="SlidingWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    /// <param name="options">Options to specify the behavior of the <see cref="SlidingWindowRateLimiter"/>.</param>
    /// <param name="tryReplenishment"> Attempts to replenish permits.
    /// <param name="retryAfterFactory">An (optional) factory to use to express retry-after back to the caller, when an operation is rate-limited.
    /// <remarks>If null, a <see cref="RateLimitRejectedException"/> with property <see cref="RateLimitRejectedException.RetryAfter"/> will be thrown to indicate rate-limiting.</remarks></param>
    /// <returns></returns>
    public static DotNet7RateLimitPolicy<TResult> SlidingWindowRateLimit<TResult>(
        SlidingWindowRateLimiterOptions options,
        TryReplenishment? tryReplenishment = null,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null!)
    {
        ReplenishingRateLimiter rateLimiter = new SlidingWindowRateLimiter(options);
        if (tryReplenishment is not null)
        {
            tryReplenishment += () => { return rateLimiter.TryReplenish(); };
        }
        return new DotNet7RateLimitPolicy<TResult>(rateLimiter, retryAfterFactory);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="TokenBucketRateLimiter" /> rate limiter.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    /// <param name="options">Options to specify the behavior of the <see cref="TokenBucketRateLimiter"/>.</param>
    /// <param name="tryReplenishment"> Attempts to replenish permits.
    /// <param name="retryAfterFactory">An (optional) factory to use to express retry-after back to the caller, when an operation is rate-limited.
    /// <remarks>If null, a <see cref="RateLimitRejectedException"/> with property <see cref="RateLimitRejectedException.RetryAfter"/> will be thrown to indicate rate-limiting.</remarks></param>
    /// <returns></returns>
    public static DotNet7RateLimitPolicy<TResult> TokenBucketRateLimit<TResult>(
        TokenBucketRateLimiterOptions options,
        TryReplenishment? tryReplenishment = null,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null!)
    {
        ReplenishingRateLimiter rateLimiter = new TokenBucketRateLimiter(options);
        if (tryReplenishment is not null)
        {
            tryReplenishment += () => { return rateLimiter.TryReplenish(); };
        }
        return new DotNet7RateLimitPolicy<TResult>(rateLimiter, retryAfterFactory);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="ConcurrencyLimiter" /> rate limiter.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    /// <param name="options">Options to specify the behavior of the <see cref="ConcurrencyLimiter"/>.</param>
    /// <param name="retryAfterFactory">An (optional) factory to use to express retry-after back to the caller, when an operation is rate-limited.
    /// <remarks>If null, a <see cref="RateLimitRejectedException"/> with property <see cref="RateLimitRejectedException.RetryAfter"/> will be thrown to indicate rate-limiting.</remarks></param>
    /// <returns></returns>
    public static DotNet7RateLimitPolicy<TResult> ConcurrencyRateLimit<TResult>(
        ConcurrencyLimiterOptions options,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null!)
    {
        RateLimiter rateLimiter = new ConcurrencyLimiter(options);

        return new DotNet7RateLimitPolicy<TResult>(rateLimiter, retryAfterFactory);
    }
}

