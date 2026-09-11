// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Workflow.Brokers.OData;

internal interface IODataResultBroker
{
    SingleResult<T> CreateSingleResult<T>(IQueryable<T> queryable);
}