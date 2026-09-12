// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Workflow.Brokers.OData;

internal sealed class ODataResultBroker : IODataResultBroker
{
    public SingleResult<T> CreateSingleResult<T>(IQueryable<T> queryable) =>
        SingleResult.Create(queryable: queryable);
}