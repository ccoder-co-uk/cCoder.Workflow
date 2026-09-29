// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace cCoder.Workflow.Engine.Exposures;

public interface IWorkflowScriptExecutionService
{
    Task<string> ExecuteAsync(string payload, bool useDetails);
}