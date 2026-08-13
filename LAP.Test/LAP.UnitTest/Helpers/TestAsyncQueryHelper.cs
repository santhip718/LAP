using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace LAP.UnitTest.Helpers;

internal static class AsyncQueryableHelper
{
    public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source)
        where T : class
    {
        var queryable = source.AsQueryable();
        return new TestAsyncEnumerable<T>(queryable);
    }

    public static IQueryable<T> AsAsyncQueryable<T>(this IQueryable<T> source)
    {
        return new TestAsyncEnumerable<T>(source.Expression);
    }
}

internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    private readonly IQueryable<T> _inner;

    public TestAsyncEnumerable(IQueryable<T> inner) : base(inner)
    {
        _inner = inner;
    }

    public TestAsyncEnumerable(Expression expression) : base(expression)
    {
        _inner = new EnumerableQuery<T>(expression) as IQueryable<T> ?? new List<T>().AsQueryable();
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new TestAsyncEnumerator<T>(_inner.AsEnumerable().GetEnumerator());

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(_inner.Provider);
}

internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

    public T Current => _inner.Current;

    public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }
}

internal class TestAsyncQueryProvider<T> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

    public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<T>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new TestAsyncEnumerable<TElement>(expression);

    public object? Execute(Expression expression) => _inner.Execute(expression);

    public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        var resultType = typeof(TResult);
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var innerType = resultType.GetGenericArguments()[0];
            var result = _inner.Execute(expression);
            var fromResultMethod = typeof(Task)
                .GetMethods()
                .First(m => m.Name == nameof(Task.FromResult) && m.ContainsGenericParameters)
                .MakeGenericMethod(innerType);
            return (TResult)fromResultMethod.Invoke(null, new[] { result })!;
        }
        return (TResult)_inner.Execute(expression);
    }
}
