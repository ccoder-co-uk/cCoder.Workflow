// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;

namespace Web.AcceptanceTests.Infrastructure;

internal sealed class IntegrationSession : ISession
{
    private readonly Dictionary<string, byte[]> values = [];

    public bool IsAvailable =>
        true;

    public string Id { get; } = Guid.NewGuid()
        .ToString(format: "N");

    public IEnumerable<string> Keys =>
        values.Keys;

    public void Clear() =>
        values.Clear();

    public Task CommitAsync(
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task LoadAsync(
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public void Remove(string key) =>
        values.Remove(key: key);

    public void Set(string key, byte[] value) =>
        values[key] = value;

    public bool TryGetValue(
        string key,
        out byte[] value) =>
        values.TryGetValue(key: key, value: out value);
}