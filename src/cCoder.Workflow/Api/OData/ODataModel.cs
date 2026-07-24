// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OData.Edm;


namespace cCoder.Workflow.Api.OData;

public class ODataModel
{
    public ODataModel()
    {
        Context = string.Empty;
        Description = string.Empty;
        EDMModel = null!;
    }

    public string Context { get; set; }

    public string Description { get; set; }

    public IEdmModel EDMModel { get; set; }
}