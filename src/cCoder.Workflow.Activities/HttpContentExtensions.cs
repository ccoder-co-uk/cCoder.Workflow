// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Activities.Support;

public static class HttpContentExtensions
{
    public static async Task<T> ReadAsAsync<T>(this HttpContent content) => cCoder.Workflow.Activities.Support.Data.ParseJson<T>(await content.ReadAsStringAsync());
}