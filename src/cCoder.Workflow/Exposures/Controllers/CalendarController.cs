// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.Workflow.Extensions.OData;
using cCoder.Workflow.Models.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.Workflow.Exposures.Controllers;

public partial class CalendarController : ODataController, ICompositionExposure
{
    private readonly ICalendarManager service;

    public CalendarController(
        ICalendarManager service
    )
    {
        this.service = service;
    }

    [HttpGet]
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
            IQueryable<Calendar> result = service.GetAll()
                .Where(predicate: calendar => calendar.Id == key);

            Calendar calendar = result.FirstOrDefault();

            if (calendar is null)
            {
                return NotFound();
            }

            return Ok(value: service.CreateSingleResult(queryable: result));
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

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
    public IActionResult GetAll(ODataQueryOptions<Calendar> queryOptions)
    {
        try
        {
            return Ok(value: service.GetAll());
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

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
    public async Task<IActionResult> Post([FromBody] Calendar newCalendar)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new cCoder.Workflow.Exposures.Results.BadRequestResult(ModelState);
            }

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await service.AddCalendarAsync(newCalendar: newCalendar));
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

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
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Calendar updatedCalendar)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new cCoder.Workflow.Exposures.Results.BadRequestResult(ModelState);
            }

            return Ok(value: await service.UpdateCalendarAsync(updatedCalendar: updatedCalendar));
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        try
        {
            await service.DeleteAsync(calendarId: key);
            return NoContent();
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            service.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}