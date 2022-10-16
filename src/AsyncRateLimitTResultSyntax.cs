using System.Threading.RateLimiting;
using Polly;

namespace DotNet7.Polly.RateLimit;

public abstract partial class DotNet7Policy
{
    /// <summary>
    /// Build a RateLimit <see cref="AsyncPolicy"/> that will rate-limit executions based on the <see cref="FixedWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="FixedWindowRateLimiterOptions"/>.</param>
    /// <param name="limiterStateAction"> Access to limiter object.
    /// <param name="retryAfterFactory">An (optional) factory to use to express retry-after back to the caller, when an operation is rate-limited.
    /// <remarks>If null, a <see cref="RateLimitRejectedException"/> with property <see cref="RateLimitRejectedException.RetryAfter"/> will be thrown to indicate rate-limiting.</remarks></param>
    /// <returns></returns>
    public static AsyncDotNet7RateLimitPolicy<TResult> FixedWindowRateLimitAsync<TResult>(
        Action<FixedWindowRateLimiterOptions> configureOptions,
        Action<RateLimiter> limiterStateAction = null!,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null!)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);
        var options = new FixedWindowRateLimiterOptions();
        configureOptions(options);

        return FixedWindowRateLimitAsync<TResult>(options, limiterStateAction, retryAfterFactory);
    }

    /// <summary>
    /// Build a RateLimit <see cref="AsyncPolicy"/> that will rate-limit executions based on the <see cref="FixedWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    /// <param name="options">Options to specify the behavior of the <see cref="FixedWindowRateLimiter"/>.</param>
    /// <param name="limiterStateAction"> Access to limiter object.
    /// <param name="retryAfterFactory">An (optional) factory to use to express retry-after back to the caller, when an operation is rate-limited.
    /// <remarks>If null, a <see cref="RateLimitRejectedException"/> with property <see cref="RateLimitRejectedException.RetryAfter"/> will be thrown to indicate rate-limiting.</remarks></param>
    /// <returns></returns>
    public static AsyncDotNet7RateLimitPolicy<TResult> FixedWindowRateLimitAsync<TResult>(
        FixedWindowRateLimiterOptions options,
        Action<RateLimiter> limiterStateAction = null!,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null!)
    {
        ArgumentNullException.ThrowIfNull(options);
        ReplenishingRateLimiter rateLimiter = new FixedWindowRateLimiter(options);
        if (limiterStateAction is not null)
        {
            limiterStateAction(rateLimiter);
        }

        return new AsyncDotNet7RateLimitPolicy<TResult>(rateLimiter, retryAfterFactory);
    }

    /// <summary>
    /// Build a RateLimit <see cref="AsyncPolicy"/> that will rate-limit executions based on the <see cref="SlidingWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="SlidingWindowRateLimiterOptions"/>.</param>
    /// <param name="limiterStateAction"> Access to limiter object.
    /// <param name="retryAfterFactory">An (optional) factory to use to express retry-after back to the caller, when an operation is rate-limited.
    /// <remarks>If null, a <see cref="RateLimitRejectedException"/> with property <see cref="RateLimitRejectedException.RetryAfter"/> will be thrown to indicate rate-limiting.</remarks></param>
    /// <returns></returns>
    public static AsyncDotNet7RateLimitPolicy<TResult> SlidingWindowRateLimitAsync<TResult>(
        Action<SlidingWindowRateLimiterOptions> configureOptions,
        Action<RateLimiter> limiterStateAction = null!,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null!)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);
        var options = new SlidingWindowRateLimiterOptions();
        configureOptions(options);

        return SlidingWindowRateLimitAsync<TResult>(options, limiterStateAction, retryAfterFactory);
    }

    /// <summary>
    /// Build a RateLimit <see cref="AsyncPolicy"/> that will rate-limit executions based on the <see cref="SlidingWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    /// <param name="options">Options to specify the behavior of the <see cref="SlidingWindowRateLimiter"/>.</param>
    /// <param name="limiterStateAction"> Access to limiter object.
    /// <param name="retryAfterFactory">An (optional) factory to use to express retry-after back to the caller, when an operation is rate-limited.
    /// <remarks>If null, a <see cref="RateLimitRejectedException"/> with property <see cref="RateLimitRejectedException.RetryAfter"/> will be thrown to indicate rate-limiting.</remarks></param>
    /// <returns></returns>
    public static AsyncDotNet7RateLimitPolicy<TResult> SlidingWindowRateLimitAsync<TResult>(
        SlidingWindowRateLimiterOptions options,
        Action<RateLimiter> limiterStateAction = null!,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null!)
    {
        ArgumentNullException.ThrowIfNull(options);
        ReplenishingRateLimiter rateLimiter = new SlidingWindowRateLimiter(options);
        if (limiterStateAction is not null)
        {
            limiterStateAction(rateLimiter);
        }

        return new AsyncDotNet7RateLimitPolicy<TResult>(rateLimiter, retryAfterFactory);
    }

    /// <summary>
    /// Build a RateLimit <see cref="AsyncPolicy"/> that will rate-limit executions based on the <see cref="TokenBucketRateLimiter" /> rate limiter.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="TokenBucketRateLimiterOptions"/>.</param>
    /// <param name="limiterStateAction"> Access to limiter object.
    /// <param name="retryAfterFactory">An (optional) factory to use to express retry-after back to the caller, when an operation is rate-limited.
    /// <remarks>If null, a <see cref="RateLimitRejectedException"/> with property <see cref="RateLimitRejectedException.RetryAfter"/> will be thrown to indicate rate-limiting.</remarks></param>
    /// <returns></returns>
    public static AsyncDotNet7RateLimitPolicy<TResult> TokenBucketRateLimitAsync<TResult>(
        Action<TokenBucketRateLimiterOptions> configureOptions,
        Action<RateLimiter> limiterStateAction = null!,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null!)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);
        var options = new TokenBucketRateLimiterOptions();
        configureOptions(options);

        return TokenBucketRateLimitAsync<TResult>(options, limiterStateAction, retryAfterFactory);
    }

    /// <summary>
    /// Build a RateLimit <see cref="AsyncPolicy"/> that will rate-limit executions based on the <see cref="TokenBucketRateLimiter" /> rate limiter.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    /// <param name="options">Options to specify the behavior of the <see cref="TokenBucketRateLimiter"/>.</param>
    /// <param name="limiterStateAction"> Access to limiter object.
    /// <param name="retryAfterFactory">An (optional) factory to use to express retry-after back to the caller, when an operation is rate-limited.
    /// <remarks>If null, a <see cref="RateLimitRejectedException"/> with property <see cref="RateLimitRejectedException.RetryAfter"/> will be thrown to indicate rate-limiting.</remarks></param>
    /// <returns></returns>
    public static AsyncDotNet7RateLimitPolicy<TResult> TokenBucketRateLimitAsync<TResult>(
        TokenBucketRateLimiterOptions options,
        Action<RateLimiter> limiterStateAction = null!,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null!)
    {
        ArgumentNullException.ThrowIfNull(options);
        ReplenishingRateLimiter rateLimiter = new TokenBucketRateLimiter(options);
        if (limiterStateAction is not null)
        {
            limiterStateAction(rateLimiter);
        }

        return new AsyncDotNet7RateLimitPolicy<TResult>(rateLimiter, retryAfterFactory);
    }

    /// <summary>
    /// Build a RateLimit <see cref="AsyncPolicy"/> that will rate-limit executions based on the <see cref="ConcurrencyLimiter" /> rate limiter.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="ConcurrencyLimiterOptions"/>.</param>
    /// <param name="limiterStateAction"> Access to limiter object.
    /// <param name="retryAfterFactory">An (optional) factory to use to express retry-after back to the caller, when an operation is rate-limited.
    /// <remarks>If null, a <see cref="RateLimitRejectedException"/> with property <see cref="RateLimitRejectedException.RetryAfter"/> will be thrown to indicate rate-limiting.</remarks></param>
    /// <returns></returns>
    public static AsyncDotNet7RateLimitPolicy<TResult> ConcurrencyRateLimitAsync<TResult>(
        Action<ConcurrencyLimiterOptions> configureOptions,
        Action<RateLimiter> limiterStateAction = null!,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null!)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);
        var options = new ConcurrencyLimiterOptions();
        configureOptions(options);

        return ConcurrencyRateLimitAsync<TResult>(options, limiterStateAction, retryAfterFactory);
    }

    /// <summary>
    /// Build a RateLimit <see cref="AsyncPolicy"/> that will rate-limit executions based on the <see cref="ConcurrencyLimiter" /> rate limiter.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    /// <param name="options">Options to specify the behavior of the <see cref="ConcurrencyLimiter"/>.</param>
    /// <param name="limiterStateAction"> Access to limiter object.
    /// <param name="retryAfterFactory">An (optional) factory to use to express retry-after back to the caller, when an operation is rate-limited.
    /// <remarks>If null, a <see cref="RateLimitRejectedException"/> with property <see cref="RateLimitRejectedException.RetryAfter"/> will be thrown to indicate rate-limiting.</remarks></param>
    /// <returns></returns>
    public static AsyncDotNet7RateLimitPolicy<TResult> ConcurrencyRateLimitAsync<TResult>(
        ConcurrencyLimiterOptions options,
        Action<RateLimiter> limiterStateAction = null!,
        Func<RateLimitLease, Context, TResult> retryAfterFactory = null!)
    {
        ArgumentNullException.ThrowIfNull(options);
        RateLimiter rateLimiter = new ConcurrencyLimiter(options);
        if (limiterStateAction is not null)
        {
            limiterStateAction(rateLimiter);
        }

        return new AsyncDotNet7RateLimitPolicy<TResult>(rateLimiter, retryAfterFactory);
    }
}