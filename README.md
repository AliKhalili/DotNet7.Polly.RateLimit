# DotNet7.Polly.RateLimit

## Introduction

Rate limiting is the concept of controlling the rate of traffic sent by a client or a service. On the client side, we need to throttle third party APIs requests in order to prevent exceeding the total number of allowed requests over a specified period.

[Polly](https://github.com/App-vNext/Polly) is a fantastic .NET library, which provides several resiliency design patterns. Developers can build different policies, such as Rate-limiting in a fluent and thread safe manager.

.Net team also announced built-in Rate Limiting support as part of .NET 7 with the nuget package [System.Threading.RateLimiting](https://www.nuget.org/packages/DotNet7.Polly.RateLimit/). This package provides a few commonly used algorithms built-in with queuing strategy.

This repository is a tiny wrapper around .NET 7 Rate Limiting package that enables developers to define flexible Polly Rate limiting policies.

## Installing via .NET CLI

```shell
dotnet add package DotNet7.Polly.RateLimit --prerelease
```

## Defining Polly Policies

### Token Bucket limiter without queue

```c#
var rateLimiterPolicy = DotNet7Policy.TokenBucketRateLimit(
            option =>
            {
                option.TokenLimit = 5;
                option.TokensPerPeriod = 1;
                option.QueueLimit = 0;
                option.AutoReplenishment = true;
                option.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
            });
```

### Fixed Window limiter without queue

```c#
var rateLimiterPolicy = DotNet7Policy.FixedWindowRateLimit(
            option =>
            {
                option.PermitLimit = 5;
                option.QueueLimit = 0;
                option.AutoReplenishment = true;
                option.Window = TimeSpan.FromSeconds(1);
            });
```

### Sliding Window limiter without queue

```c#
var rateLimiterPolicy =  DotNet7Policy.SlidingWindowRateLimit(
            option =>
            {
                option.PermitLimit = 5;
                option.SegmentsPerWindow = 1;
                option.QueueLimit = 0;
                option.AutoReplenishment = true;
                option.Window = TimeSpan.FromSeconds(5);
            });
```


## Queuing requests
```c#
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
```

## Use Rate Limiting with Http Client Factory
```c#

builder.Services.AddHttpClient("ratelimiter")
.AddPolicyHandler(DotNet7Policy.FixedWindowRateLimitAsync<HttpResponseMessage>( option =>
            {
                option.PermitLimit = 5;
                option.QueueLimit = 0;
                option.AutoReplenishment = true;
                option.Window = TimeSpan.FromSeconds(1);
            }));

```