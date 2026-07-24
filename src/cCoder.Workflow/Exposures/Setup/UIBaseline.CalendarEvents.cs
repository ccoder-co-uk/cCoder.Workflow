// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.Workflow.Exposures.Setup;

public static partial class UIBaseline
{
    static Package CalendarEvents => new()
    {
        Name = "Workflow Calendar Events",
        Category = "Workflow",
        Description = "Workflow Calendar Events.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/CalendarEvent",
                Data = """
{
  "CalendarName": "Default",
  "Name": "Summer bank holiday",
  "Start": "2026-08-31T00:00:00",
  "DurationInTicks": 864000000000,
  "Description": "UK bank holiday"
}
"""
            },
            new PackageItem
            {
                Type = "Core/CalendarEvent",
                Data = """
{
  "CalendarName": "Default",
  "Name": "Christmas Day",
  "Start": "2026-12-25T00:00:00",
  "DurationInTicks": 864000000000,
  "Description": "UK bank holiday"
}
"""
            },
            new PackageItem
            {
                Type = "Core/CalendarEvent",
                Data = """
{
  "CalendarName": "Default",
  "Name": "Boxing Day",
  "Start": "2026-12-28T00:00:00",
  "DurationInTicks": 864000000000,
  "Description": "UK bank holiday substitute day"
}
"""
            },
            new PackageItem
            {
                Type = "Core/CalendarEvent",
                Data = """
{
  "CalendarName": "Default",
  "Name": "New Year's Day",
  "Start": "2027-01-01T00:00:00",
  "DurationInTicks": 864000000000,
  "Description": "UK bank holiday"
}
"""
            },
            new PackageItem
            {
                Type = "Core/CalendarEvent",
                Data = """
{
  "CalendarName": "Default",
  "Name": "Good Friday",
  "Start": "2027-03-26T00:00:00",
  "DurationInTicks": 864000000000,
  "Description": "UK bank holiday"
}
"""
            },
            new PackageItem
            {
                Type = "Core/CalendarEvent",
                Data = """
{
  "CalendarName": "Default",
  "Name": "Easter Monday",
  "Start": "2027-03-29T00:00:00",
  "DurationInTicks": 864000000000,
  "Description": "UK bank holiday"
}
"""
            },
            new PackageItem
            {
                Type = "Core/CalendarEvent",
                Data = """
{
  "CalendarName": "Default",
  "Name": "Early May bank holiday",
  "Start": "2027-05-03T00:00:00",
  "DurationInTicks": 864000000000,
  "Description": "UK bank holiday"
}
"""
            },
            new PackageItem
            {
                Type = "Core/CalendarEvent",
                Data = """
{
  "CalendarName": "Default",
  "Name": "Spring bank holiday",
  "Start": "2027-05-31T00:00:00",
  "DurationInTicks": 864000000000,
  "Description": "UK bank holiday"
}
"""
            },
        ]
    };
}