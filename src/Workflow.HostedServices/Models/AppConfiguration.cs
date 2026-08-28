// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Security.Models;
using cCoder.Workflow.Models;

namespace Workflow.HostedServices.Models;

public sealed class AppConfiguration
{
    public CoreDataConfiguration CoreData { get; set; }
    public EventingConfiguration Eventing { get; set; }
    public SecurityConfiguration Security { get; set; }
    public SecurityDataConfiguration SecurityData { get; set; }
    public WorkflowConfiguration Workflow { get; set; }
}