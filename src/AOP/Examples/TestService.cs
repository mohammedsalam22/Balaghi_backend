namespace AOP.Examples
{
    /// <summary>
    /// Example service to demonstrate AOP interception
    /// This service can be used for testing the PerformanceInterceptor
    /// </summary>
    public interface ITestService
    {
        Task<string> FastMethodAsync();
        Task<string> SlowMethodAsync(int delayMs);
        Task<string> MethodThatThrowsAsync();
    }

    public class TestService : ITestService
    {
        public async Task<string> FastMethodAsync()
        {
            await Task.Delay(10); // Fast operation
            return "Fast method completed";
        }

        public async Task<string> SlowMethodAsync(int delayMs)
        {
            await Task.Delay(delayMs); // Configurable delay
            return $"Slow method completed after {delayMs}ms";
        }

        public async Task<string> MethodThatThrowsAsync()
        {
            await Task.Delay(10);
            throw new InvalidOperationException("This is a test exception");
        }
    }
}

