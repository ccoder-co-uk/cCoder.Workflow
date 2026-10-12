// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using cCoder.Workflow.Activities.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cCoder.Workflow.Models;

public sealed class WorkflowAttemptContext
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? ExecutionAttemptId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? ExecutionOwnerId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ExecutionState { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<JsonElement> ExecutionLog { get; set; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement> Properties { get; set; }
}