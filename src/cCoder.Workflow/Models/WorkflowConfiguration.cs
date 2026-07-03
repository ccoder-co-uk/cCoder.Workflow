using cCoder.Eventing.Models;

namespace cCoder.Workflow.Models;

public class WorkflowConfiguration
{
    public IDictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> Services { get; set; } = new Dictionary<string, string>();
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
    public string RootPath { get; set; } = "Api/Workflow";
    public bool IncludeLegacyCoreContext { get; set; } = true;
    public bool IsMigrating { get; set; }
    public EventProvider[] EventProviders { get; private set; } = [];

    public WorkflowConfiguration WithEventProviders(params EventProvider[] eventProviders)
    {
        EventProviders = eventProviders ?? [];
        return this;
    }
}
