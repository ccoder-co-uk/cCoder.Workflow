// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.Workflow.Exposures.Setup;

public static partial class UIBaseline
{
    private static Package CreatePageRolesPackage() =>
        new()
    {
        Name = "Workflow Page Roles",
        Category = "Workflow",
        Description = "Workflow Page Roles.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/WorkflowDesigner",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/BusinessProcesses/Editor",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/BusinessProcesses",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement/Scheduling",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement/Scheduling",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement/Scheduling",
  "Role": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/CalendarManagement",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/CalendarManagement",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/CalendarManagement",
  "Role": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/BusinessProcessManagement",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/BusinessProcessManagement",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/BusinessProcessManagement",
  "Role": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/BusinessProcessManagement/WorkflowDesigner",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/BusinessProcessManagement/WorkflowDesigner",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/BusinessProcessManagement/WorkflowDesigner",
  "Role": "Guests"
}
"""
            }
        ]
    };
}