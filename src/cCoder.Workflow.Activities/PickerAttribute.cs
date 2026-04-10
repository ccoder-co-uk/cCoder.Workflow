namespace cCoder.Workflow.Activities.Support;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PickerAttribute(string path) : Attribute
{
    public string Path { get; } = path;
}