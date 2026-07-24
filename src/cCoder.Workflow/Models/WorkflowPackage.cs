// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Models;

public class WorkflowPackage
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Category { get; set; }

    public string SourceApi { get; set; }

    public virtual ICollection<WorkflowPackageItem> Items { get; set; }

    public WorkflowPackage() { }

    public WorkflowPackage(string name)
    {
        Name = name;
    }
}