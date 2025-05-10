using System.Reflection;
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ClientNexus.Infrastructure.Repositories
{
    public class DbTryCatchDecorator<T> : DispatchProxy
        where T : class
    {
        protected T? _decorated;

        public static T Create(T decorated)
        {
            ArgumentNullException.ThrowIfNull(decorated);
            object proxy = Create<T, DbTryCatchDecorator<T>>();
            ((DbTryCatchDecorator<T>)proxy)._decorated = decorated;
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
                    MethodInfo? handleMethod = typeof(DbTryCatchDecorator<T>).GetMethod(
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
            catch (SqlException ex)
            {
                if (ex.Number == -2 || ex.Number == 0)
                {
                    throw new ConnectionEstablishmentException("Database connection failed.", ex);
                }
                else if (ex.Number == 258)
                {
                    throw new TimeoutException("Database operation timed out.", ex);
                }

                throw;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 515)
                    {
                        throw new InvalidInputException("Invalid data", sqlException);
                    }
                }

                throw;
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
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 0)
            {
                throw new ConnectionEstablishmentException("Database connection failed.", ex);
            }
            catch (SqlException ex) when (ex.Number == 258)
            {
                throw new TimeoutException("Database operation timed out.", ex);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 515)
                    {
                        throw new InvalidInputException("Invalid data", sqlException);
                    }
                }

                throw;
            }
        }

        // This method will be called via reflection with the correct generic type
        private async Task<TResult> HandleAsyncWithResult<TResult>(Task<TResult> task)
        {
            try
            {
                return await task.ConfigureAwait(false);
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 0)
            {
                throw new ConnectionEstablishmentException("Database connection failed.", ex);
            }
            catch (SqlException ex) when (ex.Number == 258)
            {
                throw new TimeoutException("Database operation timed out.", ex);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 515)
                    {
                        throw new InvalidInputException("Invalid data", sqlException);
                    }
                }

                throw;
            }
        }
    }
}
