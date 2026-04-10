using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal class FlowDefinitionOrchestrationService(IFlowDefinitionProcessingService processingService, IFlowDefinitionEventProcessingService eventService, IFlowDefinitionService flowDefinitionService, IFlowInstanceDataOrchestrationService flowInstanceDataOrchestrationService, IAuthorizationBroker authorizationBroker, IJsonBroker jsonBroker) : IFlowDefinitionOrchestrationService
{
    public FlowDefinition Get(Guid id)
    {
        return processingService.Get(id);
    }

    public IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<FlowDefinition> AddAsync(FlowDefinition entity)
    {
        FlowDefinition result = await processingService.AddAsync(entity);
        await eventService.RaiseFlowDefinitionAddEventAsync(result);
        return result;
    }

    public async ValueTask<FlowDefinition> UpdateAsync(FlowDefinition entity)
    {
        FlowDefinition result = await processingService.UpdateAsync(entity);
        await eventService.RaiseFlowDefinitionUpdateEventAsync(result);
        return result;
    }

    public ValueTask<Guid> QueueAsync(Guid id, string args)
    {
        return QueueAsync(id, ToExternalUser(authorizationBroker.GetCurrentUser()), args);
    }

    public async ValueTask<Guid> QueueAsync(Guid id, cCoder.Data.Models.Security.User asUser, string args)
    {
        FlowDefinition flowDefinition = flowDefinitionService.Get(id);
        FlowInstanceData flowInstance = CreateFlowInstanceData(flowDefinition, asUser, args, jsonBroker);
        return (await flowInstanceDataOrchestrationService.AddAsync(flowInstance)).Id;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        FlowDefinition entity = processingService.GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (entity is null)
            return;

        await eventService.RaiseFlowDefinitionDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        FlowDefinition[] flows = [.. processingService.GetAll(ignoreFilters: true).Where(item => item.AppId == appId)];

        foreach (FlowDefinition flow in flows)
            await DeleteAsync(flow.Id);
    }

    public ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdate(IEnumerable<FlowDefinition> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<FlowDefinition> items)
    {
        return processingService.DeleteAllAsync(items);
    }

    private static FlowInstanceData CreateFlowInstanceData(FlowDefinition flowDefinition, cCoder.Data.Models.Security.User asUser, string args, IJsonBroker jsonBroker)
    {
        if (flowDefinition == null)
        {
            throw new SecurityException("Access Denied!");
        }
        if (!asUser.IsAdminOfApp(flowDefinition.AppId) && !asUser.Can(flowDefinition.AppId, "flowdefinition_execute"))
        {
            throw new SecurityException("Access Denied!");
        }
        return FlowInstanceDataFactory.Create(flowDefinition, asUser.Id, args, jsonBroker);
    }

    private static cCoder.Data.Models.Security.User ToExternalUser(cCoder.Data.Models.Security.User item)
    {
        return new cCoder.Data.Models.Security.User
        {
            Id = item.Id,
            DisplayName = item.DisplayName,
            Email = item.Email,
            Roles = item.Roles?.Select((cCoder.Data.Models.Security.UserRole role) => new cCoder.Data.Models.Security.UserRole
            {
                RoleId = role.RoleId,
                UserId = role.UserId,
                Role = ((role.Role == null) ? null : new cCoder.Data.Models.Security.Role
                {
                    Id = role.Role.Id,
                    AppId = role.Role.AppId,
                    Name = role.Role.Name,
                    Description = role.Role.Description,
                    Privs = role.Role.Privs
                })
            }).ToArray()
        };
    }
}
