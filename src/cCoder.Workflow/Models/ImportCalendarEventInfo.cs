namespace cCoder.Workflow.Models;

internal class ImportCalendarEventInfo
{
    public string CalendarName { get; set; }

    public string Name { get; set; }

    public DateTime Start { get; set; }

    public long DurationInTicks { get; set; }

    public string Description { get; set; }
}

