// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apps.Shared.Hosting;

public sealed class NoOpSession : ISession
{
    public static NoOpSession Instance { get; } = new();

    public IEnumerable<string> Keys => [];
    public string Id => string.Empty;
    public bool IsAvailable => true;

    public Task LoadAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
    public bool TryGetValue(string key, out byte[] value)
    {
        value = [];
        return false;
    }

    public void Set(string key, byte[] value) { }
    public void Remove(string key) { }
    public void Clear() { }
}