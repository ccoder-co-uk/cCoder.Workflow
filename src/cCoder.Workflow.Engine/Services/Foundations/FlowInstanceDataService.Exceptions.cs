// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Models.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal sealed partial class FlowInstanceDataService
{
    private static async Task<TResult> TryCatch<TResult>(
        Func<Task<TResult>> operation)
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

    private static async ValueTask TryCatch(
        Func<ValueTask> operation)
    {
        try
        {
            await operation();
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