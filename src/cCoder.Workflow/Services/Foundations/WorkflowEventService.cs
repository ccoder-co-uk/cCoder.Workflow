using System.Security;
using cCoder.Data.Brokers;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using DataWorkflowEvent = cCoder.Data.Models.Workflow.WorkflowEvent;
using IAuthorizationBroker = cCoder.Workflow.Brokers.IAuthorizationBroker;


namespace cCoder.Workflow.Services.Foundations;

internal class WorkflowEventService(
    IWorkflowEventBroker workflowEventBroker,
    IAuthorizationBroker authorizationBroker
) : IWorkflowEventService
{
    public WorkflowEvent Get(Guid id)
    {
        WorkflowEvent workflowEvent = GetAll().FirstOrDefault(i => i.Id == id);
        if (workflowEvent is not null)
            return workflowEvent;

        WorkflowEvent unrestrictedWorkflowEvent = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedWorkflowEvent is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false) =>
        workflowEventBroker.GetAllWorkflowEvents(ignoreFilters);

    public async ValueTask<WorkflowEvent> AddAsync(WorkflowEvent workflowEvent)
    {
        authorizationBroker.Authorize(
            workflowEventBroker.GetAppId(ToExternalWorkflowEvent(workflowEvent)),
            $"{nameof(WorkflowEvent)}_create"
        );

        DataWorkflowEvent newWorkflowEvent = new()
        {
            Type = workflowEvent.Type,
            EventContext = workflowEvent.EventContext,
            CreatedBy = workflowEvent.CreatedBy,
            CreatedOn = workflowEvent.CreatedOn,
            FlowId = workflowEvent.FlowId,
            ExecuteAs = workflowEvent.ExecuteAs,
        };

        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        newWorkflowEvent.CreatedOn = now;
        newWorkflowEvent.CreatedBy = currentUserId;

        newWorkflowEvent = await workflowEventBroker.AddWorkflowEventAsync(newWorkflowEvent);
        return ToExternalWorkflowEvent(newWorkflowEvent, workflowEvent.Flow, workflowEvent.ExecuteAsUser);
    }

    public async ValueTask<WorkflowEvent> UpdateAsync(WorkflowEvent workflowEvent)
    {
        authorizationBroker.Authorize(
            workflowEventBroker.GetAppId(ToExternalWorkflowEvent(workflowEvent)),
            $"{nameof(WorkflowEvent)}_update"
        );

        DataWorkflowEvent updateWorkflowEvent = new()
        {
            Id = workflowEvent.Id,
            Type = workflowEvent.Type,
            EventContext = workflowEvent.EventContext,
            CreatedBy = workflowEvent.CreatedBy,
            CreatedOn = workflowEvent.CreatedOn,
            FlowId = workflowEvent.FlowId,
            ExecuteAs = workflowEvent.ExecuteAs,
        };

        updateWorkflowEvent = await workflowEventBroker.UpdateWorkflowEventAsync(
            updateWorkflowEvent
        );
        return ToExternalWorkflowEvent(updateWorkflowEvent, workflowEvent.Flow, workflowEvent.ExecuteAsUser);
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        WorkflowEvent workflowEvent = Get(id);
        authorizationBroker.Authorize(
            workflowEventBroker.GetAppId(ToExternalWorkflowEvent(workflowEvent)),
            $"{nameof(WorkflowEvent)}_delete"
        );
        _ = await workflowEventBroker.DeleteWorkflowEventAsync(ToExternalWorkflowEvent(workflowEvent));
    }

    private static WorkflowEvent ToExternalWorkflowEvent(
        DataWorkflowEvent item,
        FlowDefinition originalFlow = null,
        User originalExecuteAsUser = null
    ) =>
        new()
        {
            Id = item.Id,
            Type = item.Type,
            EventContext = item.EventContext,
            CreatedBy = item.CreatedBy,
            CreatedOn = item.CreatedOn,
            FlowId = item.FlowId,
            Flow = originalFlow ?? (item.Flow == null ? null : ToLocalFlowDefinitionShallow(item.Flow)),
            ExecuteAs = item.ExecuteAs,
            ExecuteAsUser = originalExecuteAsUser ?? (item.ExecuteAsUser == null ? null : ToLocalUser(item.ExecuteAsUser)),
        };

    static WorkflowEvent ToLocalWorkflowEvent(DataWorkflowEvent item) =>
        new()
        {
            Id = item.Id,
            Type = item.Type,
            EventContext = item.EventContext,
            CreatedBy = item.CreatedBy,
            CreatedOn = item.CreatedOn,
            FlowId = item.FlowId,
            Flow = item.Flow == null ? null : ToLocalFlowDefinitionShallow(item.Flow),
            ExecuteAs = item.ExecuteAs,
            ExecuteAsUser = item.ExecuteAsUser == null ? null : ToLocalUser(item.ExecuteAsUser),
        };

    static DataWorkflowEvent ToExternalWorkflowEvent(WorkflowEvent item) =>
        new()
        {
            Id = item.Id,
            Type = item.Type,
            EventContext = item.EventContext,
            CreatedBy = item.CreatedBy,
            CreatedOn = item.CreatedOn,
            FlowId = item.FlowId,
            Flow = item.Flow == null ? null : new cCoder.Data.Models.Workflow.FlowDefinition
            {
                Id = item.Flow.Id,
                Name = item.Flow.Name,
                Description = item.Flow.Description,
                LastUpdated = item.Flow.LastUpdated,
                LastUpdatedBy = item.Flow.LastUpdatedBy,
                CreatedOn = item.Flow.CreatedOn,
                CreatedBy = item.Flow.CreatedBy,
                AppId = item.Flow.AppId,
                DefinitionJson = item.Flow.DefinitionJson,
                ConfigJson = item.Flow.ConfigJson,
                ReportingComponentName = item.Flow.ReportingComponentName,
                InstanceReportingComponentName = item.Flow.InstanceReportingComponentName,
            },
            ExecuteAs = item.ExecuteAs,
            ExecuteAsUser = item.ExecuteAsUser == null ? null : new cCoder.Data.Models.Security.User
            {
                Id = item.ExecuteAsUser.Id,
                DisplayName = item.ExecuteAsUser.DisplayName,
                Email = item.ExecuteAsUser.Email,
            },
        };

    static FlowDefinition ToLocalFlowDefinitionShallow(cCoder.Data.Models.Workflow.FlowDefinition item) =>
        new()
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            LastUpdated = item.LastUpdated,
            LastUpdatedBy = item.LastUpdatedBy,
            CreatedOn = item.CreatedOn,
            CreatedBy = item.CreatedBy,
            AppId = item.AppId,
            DefinitionJson = item.DefinitionJson,
            ConfigJson = item.ConfigJson,
            ReportingComponentName = item.ReportingComponentName,
            InstanceReportingComponentName = item.InstanceReportingComponentName,
        };

    static User ToLocalUser(cCoder.Data.Models.Security.User item) =>
        new()
        {
            Id = item.Id,
            DisplayName = item.DisplayName,
            Email = item.Email,
            Roles = item.Roles?.Select(userRole => new UserRole
            {
                RoleId = userRole.RoleId,
                UserId = userRole.UserId,
                Role = userRole.Role == null ? null : new Role
                {
                    Id = userRole.Role.Id,
                    AppId = userRole.Role.AppId,
                    Name = userRole.Role.Name,
                    Description = userRole.Role.Description,
                    Privs = userRole.Role.Privs,
                },
            }).ToArray(),
        };
}











