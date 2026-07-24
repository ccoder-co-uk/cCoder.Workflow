// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Workflow.Web.Services.Processings;

internal sealed partial class CoreAppProcessingService
{
    private static async ValueTask<TResult> TryCatch<TResult>(
        Func<ValueTask<TResult>> operation)
    {
        try
        {
            return await operation();
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