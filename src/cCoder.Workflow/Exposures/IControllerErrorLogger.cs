// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;

namespace cCoder.Workflow.Exposures;

public interface IControllerErrorLogger
{
    void LogError(Exception exception, string message);

    object CreateSingleResult<T>(IQueryable<T> queryable);
}