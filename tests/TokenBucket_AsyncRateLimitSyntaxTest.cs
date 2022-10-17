using System.Threading.RateLimiting;
using Polly.RateLimit;
using FluentAssertions;

namespace DotNet7.Polly.RateLimit.Tests;

public class TokenBucket_AsyncRateLimitSyntaxTest : AsyncRateLimitSyntaxBaseTest
{
    [Fact]
    public override void Should_throw_when_option_is_null()
    {
        // Arrange
        var invalidSyntax = () => DotNet7Policy.TokenBucketRateLimitAsync(options: null!);

        // Act and Assert
        invalidSyntax.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("options");
    }

    [Fact]
    public override void Should_throw_when_configure_option_is_null()
    {
        // Arrange
        var invalidSyntax = () => DotNet7Policy.TokenBucketRateLimitAsync(configureOptions: null!);

        // Act and Assert
        invalidSyntax.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("configureOptions");
    }

    [Fact]
    public override async void Given_limiter_with_one_permit_should_acquire_lease()
    {
        // Arrange
        var rateLimiter = DotNet7Policy.TokenBucketRateLimitAsync(new TokenBucketRateLimiterOptions
        {
            TokenLimit = 1,
            TokensPerPeriod = 1,
            AutoReplenishment = false,
            ReplenishmentPeriod = TimeSpan.FromSeconds(2)
        });

        // Act
        var result = await TryExecutePolicy(rateLimiter);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public override async void Given_limiter_with_one_permit_throw_rate_limit_exception_for_second_request()
    {
        // Arrange
        var rateLimiter = DotNet7Policy.TokenBucketRateLimitAsync(
            option =>
        {
            option.TokenLimit = 1;
            option.TokensPerPeriod = 1;
            option.AutoReplenishment = false;
            option.ReplenishmentPeriod = TimeSpan.FromSeconds(2);
        });

        // Act
        var result1 = await TryExecutePolicy(rateLimiter);
        var exceededRequest = () => TryExecutePolicy(rateLimiter);

        // Assert
        result1.Should().Be(true);
        await exceededRequest.Should().ThrowAsync<RateLimitRejectedException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(50)]
    public override async void Given_limiter_with_N_permit_throw_rate_limit_exception_for_N_plus_1_th_request(int permitLimit)
    {
        // Arrange
        var rateLimiter = DotNet7Policy.TokenBucketRateLimitAsync(
            option =>
        {
            option.TokenLimit = permitLimit;
            option.TokensPerPeriod = 1;
            option.AutoReplenishment = false;
            option.ReplenishmentPeriod = TimeSpan.FromSeconds(2);
        });

        // Act
        var results = new bool[permitLimit];
        for (int index = 0; index < permitLimit; index++)
        {
            results[index] = await TryExecutePolicy(rateLimiter);
        }
        var exceededRequest = () => TryExecutePolicy(rateLimiter);

        // Assert
        results.Should().AllBeEquivalentTo(true);
        await exceededRequest.Should().ThrowAsync<RateLimitRejectedException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(50)]
    public override async void Given_limiter_with_N_permit_throw_rate_limit_exception_for_N_plus_1_th_request_and_acquire_for_next_N_th_after_replenishment(int permitLimit)
    {
        // Arrange
        ReplenishingRateLimiter rateLimiter = null!;
        var rateLimiterPolicy = DotNet7Policy.TokenBucketRateLimitAsync(
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
            results[index] = await TryExecutePolicy(rateLimiterPolicy);
        }
        var exceededRequest = () => TryExecutePolicy(rateLimiterPolicy);

        // Assert
        results.Should().AllBeEquivalentTo(true);
        await exceededRequest.Should().ThrowAsync<RateLimitRejectedException>();

        await Task.Delay(2);
        rateLimiter!.TryReplenish();

        // Act
        var nextResults = new bool[permitLimit];
        for (int index = 0; index < permitLimit; index++)
        {
            nextResults[index] = await TryExecutePolicy(rateLimiterPolicy);
        }

        // Assert
        nextResults.Should().AllBeEquivalentTo(true);
    }

    [Fact]
    public override async void Given_limiter_with_one_permit_and_one_queue_should_acquire_queued_and_throw_for_3_request()
    {
        // Arrange
        ReplenishingRateLimiter rateLimiter = null!;
        var rateLimiterPolicy = DotNet7Policy.TokenBucketRateLimitAsync(
                    option =>
                    {
                        option.TokenLimit = 1;
                        option.TokensPerPeriod = 1;
                        option.QueueLimit = 1;
                        option.AutoReplenishment = false;
                        option.ReplenishmentPeriod = TimeSpan.FromMilliseconds(1);
                    },
                    limiter =>
                    {
                        rateLimiter = limiter;
                    });

        // Act
        var req1 = TryExecutePolicy(rateLimiterPolicy);
        var req2 = TryExecutePolicy(rateLimiterPolicy);
        var req3 = TryExecutePolicy(rateLimiterPolicy);
        await req1;

        // Assert
        req1.Status.Should().Be(TaskStatus.RanToCompletion);
        req2.Status.Should().Be(TaskStatus.WaitingForActivation);
        req3.Status.Should().Be(TaskStatus.Faulted);

        req1.Result.Should().BeTrue();
        req3.Exception.Should().BeOfType<AggregateException>();
        req3.Exception!.InnerException.Should().BeOfType<RateLimitRejectedException>();

        await Task.Delay(2);
        rateLimiter!.TryReplenish();

        await req2;
        req2.Status.Should().Be(TaskStatus.RanToCompletion);
        req2.Result.Should().BeTrue();
    }
}