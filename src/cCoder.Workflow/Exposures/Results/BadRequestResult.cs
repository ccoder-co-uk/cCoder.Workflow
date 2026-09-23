// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace cCoder.Workflow.Exposures.Results;

public sealed class BadRequestResult : BadRequestObjectResult
{
    public BadRequestResult(ModelStateDictionary modelState)
        : base(modelState) =>
        Value = JsonBroker.SerializeForODataValue(
            value: modelState
                .Select(selector: item => new ModelStateError
                {
                    Key = item.Key,
                    Value = item.Value?.RawValue,
                    Errors = item.Value?.Errors?
                        .Select(selector: error =>
                            $"{error.ErrorMessage} - {error.Exception?.Message}")
                        .ToArray(),
                })
                .ToArray());
}