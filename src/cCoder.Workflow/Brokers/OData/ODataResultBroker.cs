// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Workflow.Brokers.OData;

internal sealed class ODataResultBroker : IODataResultBroker, IUtilityBroker
{
    public SingleResult<T> CreateSingleResult<T>(IQueryable<T> queryable) =>
        SingleResult.Create(queryable: queryable);
}