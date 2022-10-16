using System.Threading.RateLimiting;
using Polly;
namespace DotNet7.Polly.RateLimit;

public abstract partial class DotNet7Policy
{
    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="FixedWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="FixedWindowRateLimiterOptions"/>.</param>
    /// <param name="tryReplenishment"> Attempts to replenish permits.
    /// <returns>The policy instance.</returns>
    public static DotNet7RateLimitPolicy FixedWindowRateLimit(Action<FixedWindowRateLimiterOptions> configureOptions, TryReplenishment? tryReplenishment = null)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);
        var options = new FixedWindowRateLimiterOptions();
        configureOptions(options);

        return FixedWindowRateLimit(options, tryReplenishment);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="FixedWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="FixedWindowRateLimiter"/>.</param>
    /// <param name="tryReplenishment"> Attempts to replenish permits.
    /// <returns>The policy instance.</returns>
    public static DotNet7RateLimitPolicy FixedWindowRateLimit(FixedWindowRateLimiterOptions options, TryReplenishment? tryReplenishment = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ReplenishingRateLimiter rateLimiter = new FixedWindowRateLimiter(options);
        if (tryReplenishment is not null)
        {
            tryReplenishment += () => { return rateLimiter.TryReplenish(); };
        }
        return new DotNet7RateLimitPolicy(rateLimiter);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="SlidingWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="SlidingWindowRateLimiterOptions"/>.</param>
    /// <param name="tryReplenishment"> Attempts to replenish permits.
    /// <returns>The policy instance.</returns>
    public static DotNet7RateLimitPolicy SlidingWindowRateLimit(Action<SlidingWindowRateLimiterOptions> configureOptions, TryReplenishment? tryReplenishment = null)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);
        var options = new SlidingWindowRateLimiterOptions();
        configureOptions(options);

        return SlidingWindowRateLimit(options, tryReplenishment);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="SlidingWindowRateLimiter" /> rate limiter.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="SlidingWindowRateLimiter"/>.</param>
    /// <param name="tryReplenishment"> Attempts to replenish permits.
    /// <returns>The policy instance.</returns>
    public static DotNet7RateLimitPolicy SlidingWindowRateLimit(SlidingWindowRateLimiterOptions options, TryReplenishment? tryReplenishment = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ReplenishingRateLimiter rateLimiter = new SlidingWindowRateLimiter(options);
        if (tryReplenishment is not null)
        {
            tryReplenishment += () => { return rateLimiter.TryReplenish(); };
        }
        return new DotNet7RateLimitPolicy(rateLimiter);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="TokenBucketRateLimiter" /> rate limiter.
    /// </summary>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="TokenBucketRateLimiterOptions"/>.</param>
    /// <param name="tryReplenishment"> Attempts to replenish permits.
    /// <returns>The policy instance.</returns>
    public static DotNet7RateLimitPolicy TokenBucketRateLimit(Action<TokenBucketRateLimiterOptions> configureOptions, TryReplenishment? tryReplenishment = null)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);
        var options = new TokenBucketRateLimiterOptions();
        configureOptions(options);

        return TokenBucketRateLimit(options, tryReplenishment);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="TokenBucketRateLimiter" /> rate limiter.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="TokenBucketRateLimiter"/>.</param>
    /// <param name="tryReplenishment"> Attempts to replenish permits.
    /// <returns>The policy instance.</returns>
    public static DotNet7RateLimitPolicy TokenBucketRateLimit(TokenBucketRateLimiterOptions options, TryReplenishment? tryReplenishment = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ReplenishingRateLimiter rateLimiter = new TokenBucketRateLimiter(options);
        if (tryReplenishment is not null)
        {
            tryReplenishment += () => { return rateLimiter.TryReplenish(); };
        }
        return new DotNet7RateLimitPolicy(rateLimiter);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="ConcurrencyLimiter" /> rate limiter.
    /// </summary>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="ConcurrencyLimiterOptions"/>.</param>
    /// <returns>The policy instance.</returns>
    public static DotNet7RateLimitPolicy ConcurrencyRateLimit(Action<ConcurrencyLimiterOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);
        var options = new ConcurrencyLimiterOptions();
        configureOptions(options);

        return ConcurrencyRateLimit(options);
    }

    /// <summary>
    /// Build a RateLimit <see cref="Policy"/> that will rate-limit executions based on the <see cref="ConcurrencyLimiter" /> rate limiter.
    /// </summary>
    /// <param name="options">Options to specify the behavior of the <see cref="ConcurrencyLimiter"/>.</param>
    /// <returns>The policy instance.</returns>
    public static DotNet7RateLimitPolicy ConcurrencyRateLimit(ConcurrencyLimiterOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        RateLimiter rateLimiter = new ConcurrencyLimiter(options);
        return new DotNet7RateLimitPolicy(rateLimiter);
    }
}

