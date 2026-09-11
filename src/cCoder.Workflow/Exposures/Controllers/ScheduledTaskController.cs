// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Brokers.OData;
using cCoder.Workflow.Extensions.OData;
using cCoder.Workflow.Models.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.Workflow.Exposures.Controllers;

public partial class ScheduledTaskController : ODataController
{
    private readonly ILoggingBroker loggingBroker;
    private readonly IScheduledTaskManager service;

    public ScheduledTaskController(
        IScheduledTaskManager service,
        ILoggingBroker loggingBroker
    )
    {
        this.service = service;
        this.loggingBroker = loggingBroker;
    }

    [HttpPost]
    [ActionName("Execute")]
    public async Task<IActionResult> PostAsync(
        [FromRoute] int key,
        bool incrementNextExecution = true
    )
    {
        try
        {
            await service.ExecuteAsync(scheduledTaskId: key, incrementNextExecution: incrementNextExecution);
            return Ok();
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<ScheduledTask> queryOptions)
    {
        try
        {
            return Ok(value: service.GetAll());
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 3,
        MaxExpansionDepth = 3
    )]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            IQueryable<ScheduledTask> result = service.GetAll()
                .Where(predicate: scheduledTask => scheduledTask.Id == key);

            ScheduledTask scheduledTask = result.FirstOrDefault();

            if (scheduledTask is null)
            {
                return NotFound();
            }

            return Ok(value: new ODataResultBroker().CreateSingleResult(queryable: result));
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Post([FromBody] ScheduledTask newScheduledTask)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new cCoder.Workflow.Models.OData.BadRequestResult(ModelState);
            }

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await service.AddScheduledTaskAsync(newScheduledTask: newScheduledTask));
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] ScheduledTask updatedScheduledTask)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new cCoder.Workflow.Models.OData.BadRequestResult(ModelState);
            }

            return Ok(value: await service.UpdateScheduledTaskAsync(updatedScheduledTask: updatedScheduledTask));
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        try
        {
            await service.DeleteAsync(scheduledTaskId: key);
            return NoContent();
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}