// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.Workflow.Exposures.Setup;

public static partial class UIBaseline
{
    private static Package CreateResourcesPackage() =>
        new()
    {
        Name = "Workflow Resources",
        Category = "Workflow",
        Description = "Workflow Resources.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "Culture",
  "Key": "Workflow",
  "Name": "Name",
  "DisplayName": "DisplayName",
  "ShortDisplayName": "ShortDisplayName",
  "Description": "Description",
  "LastUpdated": "2022-03-18T10:41:54.1889948+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "id",
  "DisplayName": "ID",
  "ShortDisplayName": "ID",
  "Description": "ID",
  "LastUpdated": "2022-03-18T10:41:54.1890337+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "description",
  "DisplayName": "Description",
  "ShortDisplayName": "Description",
  "Description": "description",
  "LastUpdated": "2024-09-06T15:45:40.1634209+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "description",
  "DisplayName": "Description",
  "ShortDisplayName": "Description",
  "Description": "description",
  "LastUpdated": "2022-03-18T10:41:54.189098+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "lastupdated",
  "DisplayName": "Last Updated",
  "ShortDisplayName": "Last Updated",
  "Description": "Last Updated",
  "LastUpdated": "2022-03-18T10:41:54.1891519+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "Workflow",
  "Name": "close",
  "DisplayName": "Close",
  "ShortDisplayName": "Close",
  "Description": "Close",
  "LastUpdated": "2022-03-18T10:41:54.1895464+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "Workflow",
  "Name": "delete",
  "DisplayName": "Effacer",
  "ShortDisplayName": "Effacer",
  "Description": "Effacer",
  "LastUpdated": "2022-03-18T10:41:54.1895566+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "Workflow",
  "Name": "name",
  "DisplayName": "Nom",
  "ShortDisplayName": "Nom",
  "Description": "Nom",
  "LastUpdated": "2022-03-18T10:41:54.1897403+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "Workflow",
  "Name": "id",
  "DisplayName": "ID",
  "ShortDisplayName": "ID",
  "Description": "ID",
  "LastUpdated": "2022-03-18T10:41:54.1898098+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "Workflow",
  "Name": "description",
  "DisplayName": "Description",
  "ShortDisplayName": "Description",
  "Description": "Description",
  "LastUpdated": "2022-03-18T10:41:54.1898791+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "Workflow",
  "Name": "new",
  "DisplayName": "New",
  "ShortDisplayName": "New",
  "Description": "New",
  "LastUpdated": "2022-03-18T10:41:54.1900156+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "close",
  "DisplayName": "Close",
  "ShortDisplayName": "Close",
  "Description": "Close",
  "LastUpdated": "2022-03-18T10:41:54.1900661+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "Workflow",
  "Name": "save",
  "DisplayName": "Sauvegarder",
  "ShortDisplayName": "Sauvegarder",
  "Description": "Sauvegarder",
  "LastUpdated": "2022-03-18T10:41:54.1901743+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "confirm",
  "DisplayName": "Confirm",
  "ShortDisplayName": "Confirm",
  "Description": "Confirm",
  "LastUpdated": "2022-03-18T10:41:54.1902974+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "name",
  "DisplayName": "Name",
  "ShortDisplayName": "Name",
  "Description": "Name",
  "LastUpdated": "2022-03-18T10:41:54.1903765+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "en-GB",
  "Key": "Workflow",
  "Name": "new",
  "DisplayName": "New",
  "ShortDisplayName": "new",
  "Description": "new",
  "LastUpdated": "2022-10-12T16:58:03.4590576+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "new",
  "DisplayName": "New",
  "ShortDisplayName": "New",
  "Description": "New",
  "LastUpdated": "2022-03-18T10:41:54.1904706+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "save",
  "DisplayName": "Save",
  "ShortDisplayName": "Save",
  "Description": "Save",
  "LastUpdated": "2022-03-18T10:41:54.1904808+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "delete",
  "DisplayName": "Delete",
  "ShortDisplayName": "Delete",
  "Description": "delete",
  "LastUpdated": "2022-03-18T10:41:54.1904978+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-2",
  "DisplayName": "February",
  "ShortDisplayName": "February",
  "Description": "February",
  "LastUpdated": "2022-03-18T10:41:54.1906142+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-3",
  "DisplayName": "March",
  "ShortDisplayName": "March",
  "Description": "March",
  "LastUpdated": "2022-03-18T10:41:54.1906192+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-4",
  "DisplayName": "April",
  "ShortDisplayName": "April",
  "Description": "April",
  "LastUpdated": "2022-03-18T10:41:54.1906243+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-5",
  "DisplayName": "May",
  "ShortDisplayName": "May",
  "Description": "May",
  "LastUpdated": "2022-03-18T10:41:54.1906293+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-6",
  "DisplayName": "June",
  "ShortDisplayName": "June",
  "Description": "June",
  "LastUpdated": "2022-03-18T10:41:54.1906343+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-7",
  "DisplayName": "July",
  "ShortDisplayName": "July",
  "Description": "July",
  "LastUpdated": "2022-03-18T10:41:54.1906393+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-1",
  "DisplayName": "January",
  "ShortDisplayName": "January",
  "Description": "January",
  "LastUpdated": "2022-03-18T10:41:54.190871+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-7",
  "DisplayName": "July",
  "ShortDisplayName": "July",
  "Description": "July",
  "LastUpdated": "2022-03-18T10:41:54.190876+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-6",
  "DisplayName": "June",
  "ShortDisplayName": "June",
  "Description": "June",
  "LastUpdated": "2022-03-18T10:41:54.1908828+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-5",
  "DisplayName": "May",
  "ShortDisplayName": "May",
  "Description": "May",
  "LastUpdated": "2022-03-18T10:41:54.1908878+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-4",
  "DisplayName": "April",
  "ShortDisplayName": "April",
  "Description": "April",
  "LastUpdated": "2022-03-18T10:41:54.1908928+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-3",
  "DisplayName": "March",
  "ShortDisplayName": "March",
  "Description": "March",
  "LastUpdated": "2022-03-18T10:41:54.1908978+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-2",
  "DisplayName": "February",
  "ShortDisplayName": "February",
  "Description": "February",
  "LastUpdated": "2022-03-18T10:41:54.1909029+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "Month-1",
  "DisplayName": "January",
  "ShortDisplayName": "January",
  "Description": "January",
  "LastUpdated": "2022-03-18T10:41:54.1909078+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "add",
  "DisplayName": "Add",
  "ShortDisplayName": "Add",
  "Description": "Add",
  "LastUpdated": "2022-03-18T10:41:54.1909194+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "close",
  "DisplayName": "Close",
  "ShortDisplayName": "Close",
  "Description": "Close",
  "LastUpdated": "2022-03-18T10:41:54.1909446+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "saved",
  "DisplayName": "Saved",
  "ShortDisplayName": "Saved",
  "Description": "Saved",
  "LastUpdated": "2022-03-18T10:41:54.1909661+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "created",
  "DisplayName": "Created",
  "ShortDisplayName": "Created",
  "Description": "Created",
  "LastUpdated": "2022-03-18T10:41:54.1909762+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "deleted",
  "DisplayName": "Deleted",
  "ShortDisplayName": "Deleted",
  "Description": "Deleted",
  "LastUpdated": "2023-03-13T15:35:35.019476+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "context",
  "DisplayName": "Context",
  "ShortDisplayName": "Context",
  "Description": "Context",
  "LastUpdated": "2023-03-23T15:54:18.6532785+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "name",
  "DisplayName": "Name",
  "ShortDisplayName": "Name",
  "Description": "Name",
  "LastUpdated": "2023-03-23T16:00:00.7315076+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "description",
  "DisplayName": "Description",
  "ShortDisplayName": "Description",
  "Description": "Description",
  "LastUpdated": "2023-03-23T16:02:37.7283901+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "add",
  "DisplayName": "Add",
  "ShortDisplayName": "Add",
  "Description": "Add",
  "LastUpdated": "2023-07-04T13:23:00.1450217+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Workflow",
  "Name": "delete",
  "DisplayName": "Delete",
  "ShortDisplayName": "Delete",
  "Description": "Delete",
  "LastUpdated": "2023-07-04T13:25:27.3691088+01:00"
}
"""
            }
        ]
    };
}