using Castle.DynamicProxy;
using System.Diagnostics;

public class PerformanceInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var stopwatch = Stopwatch.StartNew();
        invocation.Proceed();
        stopwatch.Stop();
        Console.WriteLine($"Method {invocation.Method.Name} took {stopwatch.ElapsedMilliseconds}ms");
    }
}
