// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Security.Objects;
using cCoder.Workflow.Models;

namespace Workflow.Web.Models;

public sealed class WorkflowWebConfiguration
{
    public WorkflowWebConfiguration()
    {
        Data = new DataConfiguration();
        Eventing = new EventingConfiguration();
        Security = new SecurityConfiguration();
        Workflow = new WorkflowConfiguration();
    }

    public DataConfiguration Data { get; set; }
    public EventingConfiguration Eventing { get; set; }
    public SecurityConfiguration Security { get; set; }
    public WorkflowConfiguration Workflow { get; set; }
}