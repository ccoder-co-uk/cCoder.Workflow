// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Workflow.Dependencies.OData;

internal class WorkflowModelBuilder : ODataModelBuilder
{
    public WorkflowModelBuilder(ODataConventionModelBuilder builder = null)
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
        return base.Builder.GetEdmModel();
    }

    private void ConfigureModel()
    {
        AddCommonComplextypes();

        base.Builder.EntityType<App>()
            .Ignore(propertyExpression: i => i.Config);

        base.Builder.EntityType<FlowInstanceData>()
            .Ignore(propertyExpression: i => i.ContextJson);

        AddSet<Calendar, int>();
        AddSet<CalendarEvent, int>();
        AddSet<WorkflowEvent, Guid>();
        AddSet<FlowDefinition, Guid>();
        AddSet<FlowInstanceData, Guid>();
        AddSet<ScheduledTask, int>();
        base.Builder.Namespace = "";

        base.Builder.EntityType<FlowDefinition>().Collection.Function(name: "KnownActivityTypes")
            .Returns<MetadataContainerSet>();

        base.Builder.EntityType<FlowDefinition>().Collection.Function(name: "KnownSystemTypes")
            .Returns<MetadataContainerSet[]>();

        base.Builder.EntityType<FlowInstanceData>()
            .Action(name: "Raw");

        base.Builder.EntityType<ScheduledTask>()
            .Action(name: "Execute");

        base.Builder.EntityType<FlowDefinition>()
            .Action(name: "Execute")
            .Returns<Guid>();

        base.Builder.EntityType<FlowDefinition>().Collection.Action(name: "ExecuteScript")
            .Returns<string>();
    }
}
