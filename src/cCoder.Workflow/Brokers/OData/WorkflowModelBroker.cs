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
    : IWorkflowModelBroker
{
    private readonly ODataConventionModelBuilder builder;

    public WorkflowModelBroker(
        ODataConventionModelBuilder builder = null)
    {
        this.builder = builder ?? new ODataConventionModelBuilder();
    }

    public ODataModel Build()
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
        return builder.GetEdmModel();
    }

    private void ConfigureModel()
    {
        builder.ComplexType<MetadataContainerSet>();
        builder.ComplexType<MetadataContainer>();
        builder.ComplexType<PropertyContainer>();
        builder.ComplexType<AuditResultsByUser>();
        builder.ComplexType<AuditResultByProperty>();

        builder.EntityType<App>()
            .Ignore(propertyExpression: i => i.Config);

        builder.EntityType<FlowInstanceData>()
            .Ignore(propertyExpression: i => i.ContextJson);

        builder.EntitySet<Calendar>(name: nameof(Calendar));
        builder.EntitySet<CalendarEvent>(name: nameof(CalendarEvent));
        builder.EntitySet<WorkflowEvent>(name: nameof(WorkflowEvent));
        builder.EntitySet<FlowDefinition>(name: nameof(FlowDefinition));
        builder.EntitySet<FlowInstanceData>(name: nameof(FlowInstanceData));
        builder.EntitySet<ScheduledTask>(name: nameof(ScheduledTask));
        builder.Namespace = "";

        builder.EntityType<FlowDefinition>().Collection.Function(name: "KnownActivityTypes")
            .Returns<MetadataContainerSet>();

        builder.EntityType<FlowDefinition>().Collection.Function(name: "KnownSystemTypes")
            .Returns<MetadataContainerSet[]>();

        builder.EntityType<FlowInstanceData>()
            .Action(name: "Raw");

        builder.EntityType<ScheduledTask>()
            .Action(name: "Execute");

        builder.EntityType<FlowDefinition>()
            .Action(name: "Execute")
            .Returns<Guid>();

        builder.EntityType<FlowDefinition>().Collection.Action(name: "ExecuteScript")
            .Returns<string>();
    }
}