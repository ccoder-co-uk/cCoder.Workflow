// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Brokers.Events;

public interface IEventHubBroker
{
    void ListenToEvent<T, TService>(string eventName, Func<TService, T, ValueTask> handler);
}