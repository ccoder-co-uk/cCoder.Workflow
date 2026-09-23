// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Models.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Workflow.Services.Foundations.WorkflowExecutions;

internal sealed partial class WorkflowExecutionService
{
    private static async Task TryCatch(Func<Task> operation)
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