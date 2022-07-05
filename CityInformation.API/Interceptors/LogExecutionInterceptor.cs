using Castle.DynamicProxy;

namespace CityInformation.API.Interceptors
{
    public class LogExecutionInterceptor : IAsyncInterceptor
    {
        private readonly ILogger<LogExecutionInterceptor> _logger;

        public LogExecutionInterceptor(ILogger<LogExecutionInterceptor> logger)
        {
            _logger = logger;
        }
        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            _logger.LogInformation("[InterceptSynchronous] - START Execution of method {methodName} of {className}",
                        invocation.Method.Name,
                        invocation.TargetType.Name);

            invocation.Proceed();

            _logger.LogInformation("[InterceptSynchronous] - END Execution of method {methodName} of {className}",
                        invocation.Method.Name,
                        invocation.TargetType.Name);
        }

        private async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            _logger.LogInformation("[InterceptAsynchronous] - START Execution of method {methodName} of {className}",
                        invocation.Method.Name,
                        invocation.TargetType.Name);

            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;

            _logger.LogInformation("[InterceptAsynchronous] - END Execution of method {methodName} of {className}",
                        invocation.Method.Name,
                        invocation.TargetType.Name);
        }

        private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            _logger.LogInformation("[InterceptAsynchronous<TResult>] - START Execution of method {methodName} of {className}",
                        invocation.Method.Name,
                        invocation.TargetType.Name);

            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            TResult result = await task;

            _logger.LogInformation("[InterceptAsynchronous<TResult>] - END Execution of method {methodName} of {className}",
                        invocation.Method.Name,
                        invocation.TargetType.Name);

            return result;
        }
    }
}
