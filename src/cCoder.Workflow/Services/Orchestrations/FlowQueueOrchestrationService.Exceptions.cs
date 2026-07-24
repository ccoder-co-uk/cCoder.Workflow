// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.Workflow.Models.Exceptions;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class FlowQueueOrchestrationService
{
    private static async ValueTask<TResult> TryCatch<TResult>(
        Func<ValueTask<TResult>> operation,
        bool isValueTask)
    {
        try
        {
            return await operation();
        }
        catch (WorkflowValidationException innerException)
        {
            throw new WorkflowValidationException(innerException: innerException);
        }
        catch (WorkflowDependencyException innerException)
        {
            throw new WorkflowDependencyException(innerException: innerException);
        }
        catch (ValidationException innerException)
        {
            throw new WorkflowValidationException(innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new WorkflowDependencyException(innerException: innerException);
        }
        catch (SecurityException)
        {
            throw;
        }
        catch (Exception innerException)
        {
            throw new WorkflowServiceException(innerException: innerException);
        }
    }
}