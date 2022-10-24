using System.Threading.RateLimiting;
using Polly;
using Polly.RateLimit;

namespace DotNet7.Polly.RateLimit;
internal static class AsyncRateLimitEngine
{
    internal static async Task<TResult> ImplementationAsync<TResult>(
        RateLimiter rateLimiter,
        Func<RateLimitLease, Context, TResult> retryAfterFactory,
        Func<Context, CancellationToken, Task<TResult>> action,
        Context context,
        CancellationToken cancellationToken,
        bool continueOnCapturedContext
        )
    {
        var lease = await rateLimiter.AcquireAsync(1, cancellationToken);

        if (lease.IsAcquired)
        {
            return await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
        }

        if (retryAfterFactory != null)
        {
            return retryAfterFactory(lease, context);
        }
        
        lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter);
        throw new RateLimitRejectedException(retryAfter);
    }
}