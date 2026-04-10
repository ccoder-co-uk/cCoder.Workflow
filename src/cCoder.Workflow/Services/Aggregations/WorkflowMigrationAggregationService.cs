using cCoder.Workflow.Api.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;
using IJsonBroker = cCoder.Workflow.Brokers.IJsonBroker;


namespace cCoder.Workflow.Services.Aggregations;

internal class WorkflowMigrationAggregationService(
    IFlowDefinitionOrchestrationService flowDefinitionOrchestrationService,
    ILogger<WorkflowMigrationAggregationService> logger,
    IJsonBroker jsonBroker
) : IWorkflowMigrationAggregationService
{
    public async ValueTask ImportPackageAsync(int appId, WorkflowPackage package)
    {
        if (package.Items is null || package.Items.Count == 0)
            return;

        foreach (WorkflowPackageItem item in package.Items)
        {
            if (item.Type != "Core/FlowDefinition")
                continue;

            dynamic[] dynamicSet = item.Data.StartsWith("{")
                ? [jsonBroker.ParseJson<dynamic>(item.Data)]
                : jsonBroker.ParseJson<dynamic[]>(item.Data);
            FlowDefinition[] flowDefinitions = item.Data.StartsWith("{")
                ? [jsonBroker.ParseJson<FlowDefinition>(item.Data)]
                : jsonBroker.ParseJson<FlowDefinition[]>(item.Data);

            string[] names = flowDefinitions.Select(flowDefinition => flowDefinition.Name.ToLower()).ToArray();

            var existingFlowDefinitions = flowDefinitionOrchestrationService
                .GetAll()
                .Where(flowDefinition =>
                    flowDefinition.AppId == appId && names.Contains(flowDefinition.Name.ToLower()))
                .Select(flowDefinition => new
                {
                    flowDefinition.Id,
                    flowDefinition.Name,
                    ProcessName = flowDefinition.App.Name,
                })
                .ToArray();

            logger.LogDebug(
                "Existing Flow Definition Items:\n{ExistingFlowDefinitions}",
                existingFlowDefinitions.ToJsonForOdata()
            );

            for (int index = 0; index < flowDefinitions.Length; index++)
            {
                FlowDefinition flowDefinition = flowDefinitions[index];
                dynamic dynamicFlowDefinition = dynamicSet[index];
                string processName = (string)dynamicFlowDefinition.ProcessName;
                var existingFlowDefinition = existingFlowDefinitions.FirstOrDefault(existing =>
                    existing.ProcessName == processName
                    && existing.Name.Equals(flowDefinition.Name, StringComparison.OrdinalIgnoreCase));

                flowDefinition.AppId = appId;
                flowDefinition.Id = existingFlowDefinition?.Id ?? Guid.Empty;
            }

            _ = await flowDefinitionOrchestrationService.AddOrUpdate(flowDefinitions);
        }
    }

    public WorkflowPackage ExportPackage(int appId, string packageName)
    {
        var package = packageName == "Workflows"
            ? ExportFlowDefinitions(appId)
            : new Data.Models.Packaging.Package(packageName) { Items = [] };

        return new WorkflowPackage(package.Name)
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Category = package.Category,
            SourceApi = package.SourceApi,
            Items = package.Items?
                .Select(item => new WorkflowPackageItem
                {
                    Id = item.Id,
                    PackageId = item.PackageId,
                    Type = item.Type,
                    Data = item.Data,
                })
                .ToArray(),
        };
    }

    private cCoder.Data.Models.Packaging.Package ExportFlowDefinitions(int appId) =>
        new("Workflows")
        {
            Items =
            [
                new Data.Models.Packaging.PackageItem
                {
                    Type = "Core/FlowDefinition",
                    Data = jsonBroker.Serialize(
                        flowDefinitionOrchestrationService
                            .GetAll(false)
                            .Where(flowDefinition => flowDefinition.AppId == appId)
                            .Select(flowDefinition => new
                            {
                                ProcessName = flowDefinition.App.Name,
                                flowDefinition.Name,
                                flowDefinition.ReportingComponentName,
                                flowDefinition.InstanceReportingComponentName,
                                flowDefinition.Description,
                                flowDefinition.DefinitionJson,
                                flowDefinition.ConfigJson,
                                flowDefinition.LastUpdated,
                            })
                            .ToArray()
                    ),
                },
            ],
        };
}




