using System.Diagnostics;
using System.Threading.RateLimiting;
using Polly;
using Polly.RateLimit;

namespace DotNet7.Polly.RateLimit;

/// <summary>
/// A rate-limit policy that can be applied to synchronous delegates.
/// </summary>
public class DotNet7RateLimitPolicy : Policy, IRateLimitPolicy
{
    private readonly RateLimiter _rateLimiter;

    internal DotNet7RateLimitPolicy(RateLimiter rateLimiter)
    {
        _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
    }


    /// <inheritdoc/>
    [DebuggerStepThrough]
    protected override TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
        => RateLimitEngine.Implementation(_rateLimiter, null!, action, context, cancellationToken);
}

/// <summary>
/// A rate-limit policy that can be applied to synchronous delegates returning a value of type <typeparamref name="TResult"/>.
/// </summary>
public class DotNet7RateLimitPolicy<TResult> : Policy<TResult>, IRateLimitPolicy<TResult>
{
    private readonly RateLimiter _rateLimiter;
    private readonly Func<RateLimitLease, Context, TResult> _retryAfterFactory;

    internal DotNet7RateLimitPolicy(
        RateLimiter rateLimiter,
        Func<RateLimitLease, Context, TResult> retryAfterFactory)
    {
        _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
        _retryAfterFactory = retryAfterFactory;
    }

    /// <inheritdoc/>
    [DebuggerStepThrough]
    protected override TResult Implementation(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
        => RateLimitEngine.Implementation(_rateLimiter, _retryAfterFactory, action, context, cancellationToken);
}
