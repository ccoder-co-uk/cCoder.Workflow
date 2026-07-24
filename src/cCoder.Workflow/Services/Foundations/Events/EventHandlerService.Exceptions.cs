// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Workflow.Services.Foundations.Events;

internal sealed partial class EventHandlerService
{
    private static void TryCatch(Action operation)
    {
        try
        {
            operation();
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