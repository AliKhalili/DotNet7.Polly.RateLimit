using Polly.RateLimit;
using System.Diagnostics;

namespace DotNet7.Polly.RateLimit.Sample.Console
{
    public class Program
    {
        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static Task<bool> DoSomething()
        {
            return Task.FromResult(true);
        }

        static Stopwatch stopwatch = new Stopwatch();
        static async Task MainAsync()
        {
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
        }
    }
}
