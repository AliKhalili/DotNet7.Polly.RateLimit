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
var policy = DotNet7Policy.FixedWindowRateLimitAsync(options =>
{
    options.Window = TimeSpan.FromSeconds(1);
    options.PermitLimit = 1;
    options.QueueLimit = 4;
    options.AutoReplenishment = true;
    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
});
stopwatch.Start();
var tasks = new Task<(bool IsAcquire, long ElapsedTicks)>[6];
ManualResetEventSlim gate = new();
for (int i = 0; i < tasks.Length; i++)
{
    tasks[i] = Task.Run<(bool, long)>(async () =>
        {
            try
            {
                gate.Wait();
                return (await policy.ExecuteAsync(() => DoSomething()), stopwatch.ElapsedTicks);
            }
            catch (RateLimitRejectedException exception)
            {
                return (false, stopwatch.ElapsedTicks);
            }
        });
}
gate.Set();
await Task.WhenAll(tasks);
stopwatch.Stop();
var index = 1;
foreach (var task in tasks.OrderBy(x => x.Result.ElapsedTicks))
{
    System.Console.WriteLine("Time: {0:s\\.fff}, Task #{1} was acquired task: {2}",
        TimeSpan.FromTicks(task.Result.ElapsedTicks),
        index++,
        task.Result.IsAcquire);
}
```
```
Time: 0.008, Task #1 was acquired task: True
Time: 0.013, Task #2 was acquired task: False
Time: 1.014, Task #3 was acquired task: True
Time: 2.009, Task #4 was acquired task: True
Time: 3.007, Task #5 was acquired task: True
Time: 4.018, Task #6 was acquired task: True
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