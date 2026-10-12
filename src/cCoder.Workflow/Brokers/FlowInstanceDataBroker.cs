// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;

using cCoder.Data;
using cCoder.Data.Models.Workflow;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Brokers;

internal sealed class FlowInstanceDataBroker(ICoreContextFactory coreContextFactory)
    : IFlowInstanceDataBroker
{

    public IQueryable<FlowInstanceData> SelectAllFlowInstanceData() =>
        coreContextFactory.CreateCoreContext().FlowInstances;

    public IQueryable<FlowInstanceData> SelectAllFlowInstanceDataIgnoringQueryFilters() =>
        coreContextFactory.CreateCoreContext()
            .FlowInstances
            .IgnoreQueryFilters();

    public async ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData newFlowInstanceData)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FlowInstanceData result = (await coreDataContext.FlowInstances.AddAsync(entity: newFlowInstanceData)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<FlowInstanceData> SelectFlowInstanceDataByIdAsync(System.Guid instanceId)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return await core.FlowInstances
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate: instance => instance.Id == instanceId);
    }

    public async ValueTask<int> CompareExchangeFlowInstanceDataAsync(cCoder.Workflow.Models.WorkflowInstanceExchange workflowInstanceExchange)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return await core.FlowInstances
            .IgnoreQueryFilters()
            .Where(predicate: instance => instance.Id == workflowInstanceExchange.Expected.Id
                && instance.FlowDefinitionId == workflowInstanceExchange.Expected.FlowDefinitionId
                && instance.Caller == workflowInstanceExchange.Expected.Caller
                && instance.State == workflowInstanceExchange.Expected.State
                && instance.Start == workflowInstanceExchange.Expected.Start
                && instance.End == workflowInstanceExchange.Expected.End
                && instance.ContextJson == workflowInstanceExchange.Expected.ContextJson)
            .ExecuteUpdateAsync(setPropertyCalls: setters => setters
                .SetProperty(propertyExpression: instance => instance.Name, valueExpression: workflowInstanceExchange.Replacement.Name)
                .SetProperty(propertyExpression: instance => instance.FlowDefinitionId, valueExpression: workflowInstanceExchange.Replacement.FlowDefinitionId)
                .SetProperty(propertyExpression: instance => instance.Caller, valueExpression: workflowInstanceExchange.Replacement.Caller)
                .SetProperty(propertyExpression: instance => instance.ReportingComponentName, valueExpression: workflowInstanceExchange.Replacement.ReportingComponentName)
                .SetProperty(propertyExpression: instance => instance.ContextJson, valueExpression: workflowInstanceExchange.Replacement.ContextJson)
                .SetProperty(propertyExpression: instance => instance.State, valueExpression: workflowInstanceExchange.Replacement.State)
                .SetProperty(propertyExpression: instance => instance.Start, valueExpression: workflowInstanceExchange.Replacement.Start)
                .SetProperty(propertyExpression: instance => instance.End, valueExpression: workflowInstanceExchange.Replacement.End));
    }

    public async ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData updatedFlowInstanceData)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FlowInstanceData result = coreDataContext.FlowInstances.Update(entity: updatedFlowInstanceData).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFlowInstanceDataAsync(FlowInstanceData deletedFlowInstanceData)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FlowInstances.Remove(entity: deletedFlowInstanceData);
        return await coreDataContext.SaveChangesAsync();
    }

    public int? SelectAppId(FlowInstanceData flowInstanceData)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.FlowDefinitions

            .Where(predicate: flowDefinition => flowDefinition.Id == flowInstanceData.FlowDefinitionId)
            .Select(selector: flowDefinition => (int?)flowDefinition.AppId)
            .FirstOrDefault();

    }
}