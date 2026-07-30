// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Extensions.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace cCoder.Workflow.Models.OData;

public sealed class BadRequestResult : BadRequestObjectResult
{
    public BadRequestResult(ModelStateDictionary modelState)
        : base(modelState) =>
        Value = modelState
            .Select(selector: item => new ModelStateError
            {
                Key = item.Key,
                Value = item.Value?.RawValue,
                Errors = item.Value?.Errors?
                    .Select(selector: error =>
                        $"{error.ErrorMessage} - {error.Exception?.Message}")
                    .ToArray(),
            })
            .ToArray()
            .ToJsonForOdata();
}