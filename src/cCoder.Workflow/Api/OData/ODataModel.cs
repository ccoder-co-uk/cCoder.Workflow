// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OData.Edm;


namespace cCoder.Workflow.Api.OData;

public class ODataModel
{
    public string Context { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEdmModel EDMModel { get; set; } = null!;
}