// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Workflow.Models.Exceptions;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowExecutionEventService
{
    private static async ValueTask TryCatch(
        Func<ValueTask> operation,
        bool isValueTask)
    {
        try
        {
            await operation();
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
        catch (System.Security.SecurityException)
        {
            throw;
        }
        catch (Exception innerException)
        {
            throw new WorkflowServiceException(innerException: innerException);
        }
    }
}