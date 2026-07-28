// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Extensions.OData;
using cCoder.Workflow.Models;
using cCoder.Workflow.Models.OData;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Workflow.Brokers.OData;

internal sealed class WorkflowModelBroker
    : ODataModelBroker,
      IWorkflowModelBroker
{
    public WorkflowModelBroker(
        ODataConventionModelBuilder builder = null)
        : base(builder)
    {
    }

    public override ODataModel Build()
    {
        return new ODataModel
        {
            Context = "Core",
            Description = "Workflow endpoints for the platform.",
            EDMModel = BuildEdmModel()
        };
    }

    public void Configure()
    {
        ConfigureModel();
    }

    private IEdmModel BuildEdmModel()
    {
        ConfigureModel();
        return base.builder.GetEdmModel();
    }

    private void ConfigureModel()
    {
        AddCommonComplextypes();

        base.builder.EntityType<App>()
            .Ignore(propertyExpression: i => i.Config);

        base.builder.EntityType<FlowInstanceData>()
            .Ignore(propertyExpression: i => i.ContextJson);

        AddSet<Calendar, int>();
        AddSet<CalendarEvent, int>();
        AddSet<WorkflowEvent, Guid>();
        AddSet<FlowDefinition, Guid>();
        AddSet<FlowInstanceData, Guid>();
        AddSet<ScheduledTask, int>();
        base.builder.Namespace = "";

        base.builder.EntityType<FlowDefinition>().Collection.Function(name: "KnownActivityTypes")
            .Returns<MetadataContainerSet>();

        base.builder.EntityType<FlowDefinition>().Collection.Function(name: "KnownSystemTypes")
            .Returns<MetadataContainerSet[]>();

        base.builder.EntityType<FlowInstanceData>()
            .Action(name: "Raw");

        base.builder.EntityType<ScheduledTask>()
            .Action(name: "Execute");

        base.builder.EntityType<FlowDefinition>()
            .Action(name: "Execute")
            .Returns<Guid>();

        base.builder.EntityType<FlowDefinition>().Collection.Action(name: "ExecuteScript")
            .Returns<string>();
    }
}