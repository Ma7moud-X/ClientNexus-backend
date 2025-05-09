using System.Reflection;
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using StackExchange.Redis;

namespace ClientNexus.Infrastructure
{
    public class CacheTryCatchDecorator<T> : DispatchProxy
        where T : class
    {
        protected T? _decorated;

        public static T Create(T decorated)
        {
            ArgumentNullException.ThrowIfNull(decorated);
            object proxy = Create<T, CacheTryCatchDecorator<T>>();
            ((CacheTryCatchDecorator<T>)proxy)._decorated = decorated;
            return (T)proxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            try
            {
                var result = targetMethod?.Invoke(_decorated, args);

                // Handle different types of Task returns properly
                if (result == null)
                    return null;

                Type resultType = result.GetType();

                // Is it a Task?
                if (resultType == typeof(Task))
                {
                    return HandleAsyncVoid((Task)result);
                }
                // Is it a generic Task<T>?
                else if (
                    resultType.IsGenericType
                    && resultType.GetGenericTypeDefinition() == typeof(Task<>)
                )
                {
                    // Get the T in Task<T>
                    Type taskResultType = resultType.GetGenericArguments()[0];

                    // Use reflection to call the correct HandleAsync<T> method
                    MethodInfo? handleMethod = typeof(CacheTryCatchDecorator<T>).GetMethod(
                        nameof(HandleAsyncWithResult),
                        BindingFlags.NonPublic | BindingFlags.Instance
                    );

                    if (handleMethod is null)
                    {
                        throw new InvalidOperationException(
                            "Failed to find HandleAsyncWithResult method"
                        );
                    }

                    MethodInfo genericMethod = handleMethod.MakeGenericMethod(taskResultType);

                    return genericMethod.Invoke(this, [result]);
                }

                return result;
            }
            catch (RedisConnectionException ex)
            {
                throw new ConnectionEstablishmentException("Cache connection failed.", ex);
            }
            catch (RedisTimeoutException ex)
            {
                throw new TimeoutException("Cache operation timed out.", ex);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        private async Task HandleAsyncVoid(Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (RedisConnectionException ex)
            {
                throw new ConnectionEstablishmentException("Cache connection failed.", ex);
            }
            catch (RedisTimeoutException ex)
            {
                throw new TimeoutException("Cache operation timed out.", ex);
            }
        }

        // Handle generic Task<T>
        private async Task<T1> HandleAsyncWithResult<T1>(Task<T1> task)
        {
            try
            {
                return await task.ConfigureAwait(false);
            }
            catch (RedisConnectionException ex)
            {
                throw new ConnectionEstablishmentException("Cache connection failed.", ex);
            }
            catch (RedisTimeoutException ex)
            {
                throw new TimeoutException("Cache operation timed out.", ex);
            }
        }
    }
}
