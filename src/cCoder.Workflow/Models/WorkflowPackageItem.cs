namespace cCoder.Workflow.Models;

public class WorkflowPackageItem
{
    public Guid Id { get; set; }

    public Guid PackageId { get; set; }

    public string Type { get; set; }

    public string Data { get; set; }

    public virtual WorkflowPackage Package { get; set; }
}

