# DotNet7.Polly.RateLimit

```powershell
dotnet add package DotNet7.Polly.RateLimit --version 0.1.1
```

```c#
builder.Services.AddHttpClient("ratelimiter")
.AddPolicyHandler(DotNet7Policy.FixedWindowRateLimitAsync<HttpResponseMessage>(new FixedWindowRateLimiterOptions()
{
    AutoReplenishment = true,
    PermitLimit = 1,
    QueueLimit = 0,
    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
    Window = TimeSpan.FromSeconds(5)
}));

```