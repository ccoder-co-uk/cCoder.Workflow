// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.Workflow.Exposures.Setup;

public static partial class UIBaseline
{
    private static Package CreateCalendarsPackage() =>
        new()
    {
        Name = "Workflow Calendars",
        Category = "Workflow",
        Description = "Workflow Calendars.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "Default",
  "Description": "Default workflow calendar"
}
"""
            },
        ]
    };
}