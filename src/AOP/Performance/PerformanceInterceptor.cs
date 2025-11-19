using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AOP.Performance
{
    public class PerformanceInterceptor : IInterceptor
    {
        private readonly ILogger<PerformanceInterceptor> _logger;
        private readonly PerformanceInterceptorOptions _options;

        public PerformanceInterceptor(
            ILogger<PerformanceInterceptor> logger,
            IOptions<PerformanceInterceptorOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));
                
            var methodName = $"{invocation.TargetType.Name}.{invocation.Method.Name}";
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Log method entry (only if enabled)
                if (_options.LogMethodEntry)
                {
                    _logger.LogDebug(
                        "Executing method {MethodName} in {TargetType}",
                        invocation.Method.Name,
                        invocation.TargetType.Name);
                }

                // Execute the actual method
                invocation.Proceed();

                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                // Log performance based on thresholds
                LogPerformance(methodName, elapsedMs, success: true);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                _logger.LogError(ex,
                    "Method {MethodName} failed after {ElapsedMs}ms in {TargetType}",
                    invocation.Method.Name,
                    elapsedMs,
                    invocation.TargetType.Name);

                // Re-throw to maintain original exception behavior
                throw;
            }
        }

        private void LogPerformance(string methodName, long elapsedMs, bool success)
        {
            // Check if performance logging is enabled
            if (!_options.Enabled)
            {
                return;
            }

            // Determine log level based on thresholds
            if (elapsedMs >= _options.ErrorThresholdMs)
            {
                _logger.LogError(
                    "Method {MethodName} took {ElapsedMs}ms (exceeds error threshold of {ErrorThreshold}ms)",
                    methodName,
                    elapsedMs,
                    _options.ErrorThresholdMs);
            }
            else if (elapsedMs >= _options.WarningThresholdMs)
            {
                _logger.LogWarning(
                    "Method {MethodName} took {ElapsedMs}ms (exceeds warning threshold of {WarningThreshold}ms)",
                    methodName,
                    elapsedMs,
                    _options.WarningThresholdMs);
            }
            else if (_options.LogAllMethods || elapsedMs >= _options.InfoThresholdMs)
            {
                _logger.LogInformation(
                    "Method {MethodName} completed in {ElapsedMs}ms",
                    methodName,
                    elapsedMs);
            }
            else
            {
                // Only log at Debug level for fast methods
                _logger.LogDebug(
                    "Method {MethodName} completed in {ElapsedMs}ms",
                    methodName,
                    elapsedMs);
            }
        }
    }

    public class PerformanceInterceptorOptions
    {
        public bool Enabled { get; set; } = true;
        public bool LogMethodEntry { get; set; } = false;
        public bool LogAllMethods { get; set; } = false;
        public int InfoThresholdMs { get; set; } = 100;
        public int WarningThresholdMs { get; set; } = 1000;
        public int ErrorThresholdMs { get; set; } = 5000;
    }
}
