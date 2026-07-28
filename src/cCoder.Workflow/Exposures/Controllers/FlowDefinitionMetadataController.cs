// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Foundations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace cCoder.Workflow.Exposures.Controllers;

[Route("Api/Workflow/FlowDefinition")]
public sealed class FlowDefinitionMetadataController(
    IWorkflowMetadataTypeService service)
    : ControllerBase
{
    [HttpGet("KnownActivityTypes()")]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.All,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 6,
        MaxExpansionDepth = 6)]
    public IActionResult GetKnownActivityTypes() =>
        Ok(value: service.GetKnownActivityTypes());

    [AllowAnonymous]
    [HttpGet("KnownSystemTypes()")]
    public IActionResult GetKnownSystemTypes() =>
        Ok(value: service.GetKnownSystemTypes());
}