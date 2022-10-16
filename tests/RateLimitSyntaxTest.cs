using System.Threading.RateLimiting;
using Polly.RateLimit;
using FluentAssertions;

namespace DotNet7.Polly.RateLimit.Tests;

public class RateLimitSyntaxTest
{
    private bool TryExecutePolicy(DotNet7RateLimitPolicy policy)
    {
        return policy.Execute<bool>(() => true);
    }

    [Fact]
    public void Should_throw_when_option_is_null()
    {
        // Arrange
        var invalidSyntax = () => DotNet7Policy.FixedWindowRateLimit(options: null!);

        // Act and Assert
        invalidSyntax.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("options");
    }

    [Fact]
    public void Should_throw_when_configure_option_is_null()
    {
        // Arrange
        var invalidSyntax = () => DotNet7Policy.FixedWindowRateLimit(configureOptions: null!);

        // Act and Assert
        invalidSyntax.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("configureOptions");
    }

    [Fact]
    public void Given_fixed_window_with_one_permit_should_acquire_lease()
    {
        // Arrange
        var rateLimiter = DotNet7Policy.FixedWindowRateLimit(new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            AutoReplenishment = false,
            Window = TimeSpan.FromSeconds(2)
        });

        // Act
        var result = TryExecutePolicy(rateLimiter);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public void Given_fixed_window_with_one_permit_throw_rate_limit_exception_for_second_request()
    {
        // Arrange
        var rateLimiter = DotNet7Policy.FixedWindowRateLimit(new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            AutoReplenishment = false,
            Window = TimeSpan.FromSeconds(2)
        });

        // Act
        var result1 = TryExecutePolicy(rateLimiter);
        var exceededRequest = () => TryExecutePolicy(rateLimiter);

        // Assert
        result1.Should().Be(true);
        exceededRequest.Should().Throw<RateLimitRejectedException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(50)]
    public void Given_fixed_window_with_N_permit_throw_rate_limit_exception_for_N_plus_1_th_request(int permitLimit)
    {
        // Arrange
        var rateLimiter = DotNet7Policy.FixedWindowRateLimit(new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            AutoReplenishment = false,
            Window = TimeSpan.FromSeconds(2)
        });

        // Act
        var results = new bool[permitLimit];
        for (int index = 0; index < permitLimit; index++)
        {
            results[index] = TryExecutePolicy(rateLimiter);
        }
        var exceededRequest = () => TryExecutePolicy(rateLimiter);

        // Assert
        results.Should().AllBeEquivalentTo(true);
        exceededRequest.Should().Throw<RateLimitRejectedException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(50)]
    public async void Given_fixed_window_with_N_permit_throw_rate_limit_exception_for_N_plus_1_th_request_and_acquire_for_next_N_th_after_replenishment(int permitLimit)
    {
        // Arrange
        ReplenishingRateLimiter rateLimiter = null!;
        var rateLimiterPolicy = DotNet7Policy.FixedWindowRateLimit(
            option =>
            {
                option.PermitLimit = permitLimit;
                option.QueueLimit = 0;
                option.AutoReplenishment = false;
                option.Window = TimeSpan.FromMilliseconds(1);
            },
            limiter =>
            {
                rateLimiter = limiter;
            });

        // Act
        var results = new bool[permitLimit];
        for (int index = 0; index < permitLimit; index++)
        {
            results[index] = TryExecutePolicy(rateLimiterPolicy);
        }
        var exceededRequest = () => TryExecutePolicy(rateLimiterPolicy);

        // Assert
        results.Should().AllBeEquivalentTo(true);
        exceededRequest.Should().Throw<RateLimitRejectedException>();

        await Task.Delay(1);
        rateLimiter!.TryReplenish();

        // Act
        var nextResults = new bool[permitLimit];
        for (int index = 0; index < permitLimit; index++)
        {
            nextResults[index] = TryExecutePolicy(rateLimiterPolicy);
        }

        // Assert
        nextResults.Should().AllBeEquivalentTo(true);
    }
}