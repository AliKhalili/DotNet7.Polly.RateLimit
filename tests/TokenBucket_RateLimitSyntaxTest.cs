using System.Threading.RateLimiting;
using Polly.RateLimit;
using FluentAssertions;

namespace DotNet7.Polly.RateLimit.Tests;

public class TokenBucket_RateLimitSyntaxTest : RateLimitSyntaxBaseTest
{
    [Fact]
    public override void Should_throw_when_option_is_null()
    {
        // Arrange
        var invalidSyntax = () => DotNet7Policy.TokenBucketRateLimit(options: null!);

        // Act and Assert
        invalidSyntax.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("options");
    }

    [Fact]
    public override void Should_throw_when_configure_option_is_null()
    {
        // Arrange
        var invalidSyntax = () => DotNet7Policy.TokenBucketRateLimit(configureOptions: null!);

        // Act and Assert
        invalidSyntax.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("configureOptions");
    }

    [Fact]
    public override void Given_limiter_with_one_permit_should_acquire_lease()
    {
        // Arrange
        var rateLimiter = DotNet7Policy.TokenBucketRateLimit(new TokenBucketRateLimiterOptions
        {
            TokenLimit = 1,
            TokensPerPeriod = 1,
            AutoReplenishment = false,
            ReplenishmentPeriod = TimeSpan.FromSeconds(2)
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
        var rateLimiter = DotNet7Policy.TokenBucketRateLimit(new TokenBucketRateLimiterOptions
        {
            TokenLimit = 1,
            TokensPerPeriod = 1,
            AutoReplenishment = false,
            ReplenishmentPeriod = TimeSpan.FromSeconds(2)
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
        var rateLimiter = DotNet7Policy.TokenBucketRateLimit(new TokenBucketRateLimiterOptions
        {
            TokenLimit = permitLimit,
            TokensPerPeriod = permitLimit,
            AutoReplenishment = false,
            ReplenishmentPeriod = TimeSpan.FromSeconds(1)
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
        var rateLimiterPolicy = DotNet7Policy.TokenBucketRateLimit(
            option =>
            {
                option.TokenLimit = permitLimit;
                option.TokensPerPeriod = permitLimit;
                option.QueueLimit = 0;
                option.AutoReplenishment = false;
                option.ReplenishmentPeriod = TimeSpan.FromMilliseconds(1);
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