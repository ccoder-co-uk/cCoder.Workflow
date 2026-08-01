// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Models.Results;

public class Result
{
    public virtual string Id { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}