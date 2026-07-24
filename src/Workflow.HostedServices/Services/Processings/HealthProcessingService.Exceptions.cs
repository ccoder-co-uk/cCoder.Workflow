// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Workflow.HostedServices.Services.Processings;

internal sealed partial class HealthProcessingService
{
    private static TResult TryCatch<TResult>(
        Func<TResult> operation)
    {
        try
        {
            return operation();
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
        catch (InvalidOperationException innerException)
        {
            throw new WorkflowDependencyException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new WorkflowServiceException(
                innerException: innerException);
        }
    }
}