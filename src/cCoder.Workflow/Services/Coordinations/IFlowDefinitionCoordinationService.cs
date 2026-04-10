using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Coordinations;

public interface IFlowDefinitionCoordinationService
{
    ValueTask HandleFlowDefinitionAddAsync(FlowDefinition flowDefinition);

    ValueTask HandleFlowDefinitionUpdateAsync(FlowDefinition flowDefinition);

    ValueTask HandleFlowDefinitionDeleteAsync(FlowDefinition flowDefinition);
}








