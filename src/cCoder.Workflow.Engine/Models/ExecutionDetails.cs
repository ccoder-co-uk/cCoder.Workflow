// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Newtonsoft.Json.Linq;

namespace cCoder.Workflow.Engine.Models;

public sealed class ExecutionDetails
{
    public string Script { get; set; }

    public JObject Model { get; set; }
}