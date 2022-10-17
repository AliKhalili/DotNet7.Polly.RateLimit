using System.Threading.RateLimiting;
using Polly.RateLimit;
using FluentAssertions;

namespace DotNet7.Polly.RateLimit.Tests;

public class SlidingWindow_RateLimitSyntaxTest : RateLimitSyntaxBaseTest
{

    [Fact]
    public override void Should_throw_when_option_is_null()
    {
        // Arrange
        var invalidSyntax = () => DotNet7Policy.SlidingWindowRateLimit(options: null!);

        // Act and Assert
        invalidSyntax.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("options");
    }

    [Fact]
    public override void Should_throw_when_configure_option_is_null()
    {
        // Arrange
        var invalidSyntax = () => DotNet7Policy.SlidingWindowRateLimit(configureOptions: null!);

        // Act and Assert
        invalidSyntax.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("configureOptions");
    }

    [Fact]
    public override void Given_limiter_with_one_permit_should_acquire_lease()
    {
        // Arrange
        var rateLimiter = DotNet7Policy.SlidingWindowRateLimit(new SlidingWindowRateLimiterOptions
        {
            PermitLimit = 1,
            SegmentsPerWindow = 1,
            AutoReplenishment = false,
            Window = TimeSpan.FromSeconds(2)
        });

        // Act
        var result = TryExecutePolicy(rateLimiter);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public override void Given_limiter_with_one_permit_throw_rate_limit_exception_for_second_request()
    {
        // Arrange
        var rateLimiter = DotNet7Policy.SlidingWindowRateLimit(new SlidingWindowRateLimiterOptions
        {
            PermitLimit = 1,
            SegmentsPerWindow = 1,
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
    public override void Given_limiter_with_N_permit_throw_rate_limit_exception_for_N_plus_1_th_request(int permitLimit)
    {
        // Arrange
        var rateLimiter = DotNet7Policy.SlidingWindowRateLimit(new SlidingWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            SegmentsPerWindow = 1,
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
    public override void Given_limiter_with_N_permit_throw_rate_limit_exception_for_N_plus_1_th_request_and_acquire_for_next_N_th_after_replenishment(int permitLimit)
    {
        // Arrange
        ReplenishingRateLimiter rateLimiter = null!;
        var rateLimiterPolicy = DotNet7Policy.SlidingWindowRateLimit(
            option =>
            {
                option.PermitLimit = permitLimit;
                option.SegmentsPerWindow = 1;
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

        Task.Delay(2).GetAwaiter().GetResult();
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