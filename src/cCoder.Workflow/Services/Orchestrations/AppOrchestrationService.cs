using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Orchestrations;

internal class AppOrchestrationService(IFlowDefinitionOrchestrationService flowDefinitionOrchestrationService)
    : IAppOrchestrationService
{
    public async ValueTask AddAsync(App app)
    {
        StampFlows(app);
        _ = await flowDefinitionOrchestrationService.AddOrUpdate(app.Flows ?? []);
    }

    public async ValueTask UpdateAsync(App app)
    {
        StampFlows(app);
        _ = await flowDefinitionOrchestrationService.AddOrUpdate(app.Flows ?? []);
    }

    public async ValueTask DeleteAsync(int appId)
    {
        await flowDefinitionOrchestrationService.DeleteByAppIdAsync(appId);
    }

    private static void StampFlows(App app)
    {
        foreach (FlowDefinition flow in app.Flows ?? [])
            flow.AppId = app.Id;
    }
}

