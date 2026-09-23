// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowRequestBodyService(
    IStreamBroker streamBroker) : IWorkflowRequestBodyService
{
    public ValueTask<string> ReadTextAsync(Stream stream) =>
        TryCatch(
            operation: async () =>
            {
                ValidateInputs(inputs: [stream]);

                return await streamBroker.ReadTextAsync(stream: stream);
            },
            isValueTask: true);
}