// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Models.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Workflow.Services.Foundations.WorkflowFunctionStreams;

internal sealed partial class WorkflowFunctionStreamService
{
    private static async ValueTask<TResult> TryCatch<TResult>(
        Func<ValueTask<TResult>> operation,
        bool isValueTask)
    {
        try
        {
            return await operation();
        }
        catch (WorkflowEngineValidationException innerException)
        {
            throw new WorkflowEngineValidationException(
                innerException: innerException);
        }
        catch (WorkflowEngineDependencyException innerException)
        {
            throw new WorkflowEngineDependencyException(
                innerException: innerException);
        }
        catch (ValidationException innerException)
        {
            throw new WorkflowEngineValidationException(
                innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new WorkflowEngineDependencyException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new WorkflowEngineServiceException(
                innerException: innerException);
        }
    }
}