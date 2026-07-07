using cCoder.Data.Models.Packaging;

namespace cCoder.Workflow.Exposures.Setup;

public static partial class UIBaseline
{
    static Package Pages => new()
    {
        Name = "Workflow Pages",
        Category = "Workflow",
        Description = "Workflow Pages.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/WorkflowDesigner",
  "Name": "Workflow Designer",
  "ResourceKey": "",
  "ShowOnMenus": false,
  "Order": 0,
  "LastUpdated": "2024-04-04T15:46:42.121866+01:00",
  "Layout": "Workflow",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[floweditor]]"
    },
    {
      "CultureId": "en-GB",
      "Name": "body",
      "Html": "[component[floweditor]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Workflow Designer",
      "Keywords": "Workflow Designer",
      "Title": "Workflow Designer"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/BusinessProcesses/Editor",
  "Name": "Editor",
  "ResourceKey": "",
  "ShowOnMenus": false,
  "Order": 0,
  "LastUpdated": "2024-04-04T16:34:04.820373+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[WorkflowManagement]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Business Process Editor",
      "Keywords": "Business Process Editor",
      "Title": "Editor"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/BusinessProcesses",
  "Name": "Business Processes",
  "ResourceKey": "",
  "ShowOnMenus": true,
  "Order": 3,
  "LastUpdated": "2024-06-24T10:38:55.7061513+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[WorkflowManagement]]"
    },
    {
      "CultureId": "fr-FR",
      "Name": "body",
      "Html": "[component[WorkflowManagement]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Manage the app's import processes, including their offer generation processes.",
      "Keywords": "Business Process Management",
      "Title": "Business Processes"
    }
  ]
}
"""
            },
        ]
    };
}