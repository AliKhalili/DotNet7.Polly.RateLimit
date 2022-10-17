namespace DotNet7.Polly.RateLimit.Tests;

public abstract class RateLimitSyntaxBaseTest
{
    protected bool TryExecutePolicy(DotNet7RateLimitPolicy policy)
    {
        return policy.Execute<bool>(() => true);
    }
    public abstract void Should_throw_when_option_is_null();
    public abstract void Should_throw_when_configure_option_is_null();
    public abstract void Given_limiter_with_one_permit_should_acquire_lease();
    public abstract void Given_limiter_with_one_permit_throw_rate_limit_exception_for_second_request();
    public abstract void Given_limiter_with_N_permit_throw_rate_limit_exception_for_N_plus_1_th_request(int permitLimit);
    public abstract void Given_limiter_with_N_permit_throw_rate_limit_exception_for_N_plus_1_th_request_and_acquire_for_next_N_th_after_replenishment(int permitLimit);
}

public abstract class AsyncRateLimitSyntaxBaseTest : RateLimitSyntaxBaseTest
{
    protected Task<bool> TryExecutePolicy(AsyncDotNet7RateLimitPolicy policy)
    {
        return policy.ExecuteAsync<bool>(() => Task.FromResult<bool>(true));
    }
    public abstract void Given_limiter_with_one_permit_and_one_queue_should_acquire_queued_and_throw_for_3_request();
}