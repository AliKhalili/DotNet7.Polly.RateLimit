using System.Threading.RateLimiting;
using Polly.RateLimit;
using FluentAssertions;

namespace DotNet7.Polly.RateLimit.Tests;

public class RateLimitSyntaxTest
{
    private Func<bool> GetExecution(DotNet7RateLimitPolicy policy)
    {
        return () => policy.Execute<bool>(() => true);
    }

    [Fact]
    public void Given_fixed_window_with_one_permit_should_acquire_lease()
    {
        // Arrange
        var limiter = DotNet7Policy.FixedWindowRateLimit(new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(10)
        });

        // Act
        var request1 = GetExecution(limiter);

        // Asset
        request1.Invoke().Should().Be(true);
    }

    [Fact]
    public void Given_fixed_window_with_one_permit_throw_rate_limit_exception_for_second_request()
    {
        // Arrange
        var limiter = DotNet7Policy.FixedWindowRateLimit(new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(10)
        });

        // Act
        var request1 = GetExecution(limiter);
        var request2 = GetExecution(limiter);

        // Asset
        request1.Invoke().Should().Be(true);
        request2.Should().Throw<RateLimitRejectedException>();
    }
}