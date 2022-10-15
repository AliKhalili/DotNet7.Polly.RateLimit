using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Polly;
using DotNet7.Polly.RateLimit;


var builder = WebApplication.CreateBuilder(args);

var noOp = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();

builder.Services.AddHttpClient("ratelimiter")
.AddPolicyHandler(DotNet7Policy.FixedWindowRateLimitAsync<HttpResponseMessage>(new FixedWindowRateLimiterOptions()
{
    AutoReplenishment = true,
    PermitLimit = 1,
    QueueLimit = 0,
    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
    Window = TimeSpan.FromSeconds(5)
}));

var app = builder.Build();

app.MapGet("/", async ([FromServices] IHttpClientFactory httpClientFactory) =>
{

    var httpClient = httpClientFactory.CreateClient("ratelimiter");
    var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://jsonplaceholder.typicode.com/posts?userId=1");

    var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
    if (httpResponseMessage.IsSuccessStatusCode)
    {
        var content = await httpResponseMessage.Content.ReadAsStringAsync();
        return Results.Ok(content);
    }
    return Results.Problem("too many request");
});

app.Run();