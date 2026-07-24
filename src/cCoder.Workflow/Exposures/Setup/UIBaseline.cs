// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.Workflow.Exposures.Setup;

public static partial class UIBaseline
{
    public static readonly Package[] Packages = [
        CreateComponentsPackage(),
        CreatePagesPackage(),
        CreateResourcesPackage(),
        CreateFlowDefinitionsPackage(),
        CreateCalendarsPackage(),
        CreateCalendarEventsPackage(),
        CreatePageRolesPackage()
    ];
}