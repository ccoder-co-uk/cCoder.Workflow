using cCoder.Data.Models.Packaging;

namespace cCoder.Workflow.Exposures.Setup;

public static partial class UIBaseline
{
    public static Package[] Packages => [
        Components,
        Pages,
        Resources,
        FlowDefinitions,
        Calendars,
        CalendarEvents,
        PageRoles
    ];
}
