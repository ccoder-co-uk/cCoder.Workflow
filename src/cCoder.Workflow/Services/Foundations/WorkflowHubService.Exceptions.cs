// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Workflow.Models.Exceptions;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowHubService
{
    private static async Task TryCatch(Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (WorkflowValidationException innerException)
        {
            throw new WorkflowValidationException(
                innerException: innerException);
        }
        catch (WorkflowDependencyException innerException)
        {
            throw new WorkflowDependencyException(
                innerException: innerException);
        }
        catch (ValidationException innerException)
        {
            throw new WorkflowValidationException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new WorkflowServiceException(
                innerException: innerException);
        }
    }
}