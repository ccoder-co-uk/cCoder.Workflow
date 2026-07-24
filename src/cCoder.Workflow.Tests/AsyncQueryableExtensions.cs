// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;


namespace cCoder.Core.Services.Tests;

internal static class AsyncQueryableExtensions
{
    internal static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source) =>
        new TestAsyncEnumerable<T>(source);
}

internal sealed class TestAsyncEnumerable<T>
    : EnumerableQuery<T>,
        IAsyncEnumerable<T>,
        IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable) { }

    public TestAsyncEnumerable(Expression expression)
        : base(expression) { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

    IQueryProvider IQueryable.Provider =>
        new TestAsyncQueryProvider<T>(((IQueryable)this).Expression);
}

internal sealed class TestAsyncQueryProvider<TEntity>(Expression expression) : IAsyncQueryProvider
{
    public IQueryable CreateQuery(Expression queryExpression) =>
        new TestAsyncEnumerable<TEntity>(queryExpression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression queryExpression) =>
        new TestAsyncEnumerable<TElement>(queryExpression);

    public object Execute(Expression queryExpression) => CreateProvider().Execute(queryExpression)!;

    public TResult Execute<TResult>(Expression queryExpression) =>
        CreateProvider().Execute<TResult>(queryExpression);

    public TResult ExecuteAsync<TResult>(
        Expression queryExpression,
        CancellationToken cancellationToken = default
    )
    {
        Type expectedResultType = typeof(TResult).GetGenericArguments()[0];

        object executionResult = typeof(IQueryProvider)
            .GetMethods()
            .Single(method =>
                method.Name == nameof(IQueryProvider.Execute) && method.IsGenericMethod
            )
            .MakeGenericMethod(expectedResultType)
            .Invoke(CreateProvider(), [queryExpression])!;

        return (TResult)
            typeof(Task)
                .GetMethods()
                .Single(method => method.Name == nameof(Task.FromResult))
                .MakeGenericMethod(expectedResultType)
                .Invoke(null, [executionResult])!;
    }

    private IQueryProvider CreateProvider() =>
        ((IQueryable)new EnumerableQuery<TEntity>(expression)).Provider;
}

internal sealed class TestAsyncEnumerator<T>(IEnumerator<T> inner) : IAsyncEnumerator<T>
{
    public T Current => inner.Current;

    public ValueTask DisposeAsync()
    {
        inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync() => new(inner.MoveNext());
}