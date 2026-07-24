// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Dynamic;
using cCoder.Workflow.Api.OData;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Activities.Sftp;
using cCoder.Workflow.Activities.Activities.DMS;
using cCoder.Workflow.Activities.Activities.Transformation;
using cCoder.Workflow.Activities.Activities.Api;
using cCoder.Workflow.Activities.Activities.Templating;


namespace cCoder.Workflow.Services.Foundations;

internal sealed class WorkflowMetadataTypeService : IWorkflowMetadataTypeService
{
    public MetadataContainerSet GetCoreMetadata() =>
        new()
        {
            Name = "Workflow",
            UriBase = "Workflow",
            Types =
            [
                Entity<Calendar>(),
                Entity<CalendarEvent>(),
                Entity<FlowDefinition>(),
                Entity<FlowInstanceData>(),
                Entity<ScheduledTask>(),
                Entity<WorkflowEvent>(),
            ],
        };

    public MetadataContainerSet[] GetKnownActivityTypes() =>
    [
        Set(name:"ApiActivity",
types:        [
            typeof(ApiPostBatch),
            typeof(ApiDelete<object>),
            typeof(ApiGet<object>),
            typeof(ApiGetCollection<object>),
            typeof(ApiPost<object, object>),
            typeof(ApiPut<object, object>),
            typeof(AuthenticateActivity),
        ]),
        Set(name:"DMSActivity",
types:        [
            typeof(CsvFileActivity),
            typeof(CSVFolderContentActivity),
            typeof(DMSCreateBinaryFilesActivity),
            typeof(DMSCreateTextFilesActivity),
            typeof(DMSDeleteActivity),
            typeof(FolderImportActivity),
            typeof(JsonFileActivity),
            typeof(JsonFolderContentActivity),
            typeof(MoveActivity),
            typeof(MoveAllActivity),
            typeof(TextFileContentActivity),
            typeof(XmlFileActivity),
            typeof(XmlFolderContentActivity),
        ]),
        Set(name:"LogActivity",
types:        [
            typeof(DebugActivity),
            typeof(ErrorActivity),
            typeof(InfoActivity),
            typeof(WarningActivity),
        ]),
        Set(name:"SftpActivity",
types:        [
            typeof(SftpDeleteFilesActivity),
            typeof(SftpDeleteFoldersActivity),
            typeof(SftpGetFolderContentsActivity),
            typeof(SftpGetListOfFilesActivity),
            typeof(SftpGetListOfFoldersActivity),
            typeof(SftpMoveFilesToFolderActivity),
            typeof(SftpUploadTextFilesToFolderActivity),
        ]),
        Set(name:"TemplatingActivity",
types:        [
            typeof(PageBuilder),
            typeof(SendEmailActivity),
        ]),
        Set(name:"TransformationActivity",
types:        [
            typeof(ConvertActivity<object, object>),
            typeof(CsvXslActivity<object>),
            typeof(DynamicDataFlattenActivity),
            typeof(JsonXslActivity<object>),
            typeof(XmlXslActivity<object>),
            typeof(XslActivity),
        ]),
        Set(name:"Workflow",
types:        [
            typeof(EventTrigger<object>),
            typeof(ExecuteFlow),
            typeof(FormSubmissionActivity<object>),
            typeof(Start),
            typeof(Flow),
            typeof(Link),
            typeof(WorkflowLogEntry),
            typeof(WorkflowLogLevel),
        ]),
    ];

    public MetadataContainerSet[] GetKnownSystemTypes() =>
    [
        new MetadataContainerSet
        {
            Name = "System",
            Types =
            [
                Metadata(type:typeof(int), category:"System"),
                Metadata(type:typeof(string), category:"System"),
                Metadata(type:typeof(decimal), category:"System"),
                Metadata(type:typeof(float), category:"System"),
                Metadata(type:typeof(bool), category:"System"),
                Metadata(type:typeof(DateTime), category:"System"),
                Metadata(type:typeof(DateTimeOffset), category:"System"),
                Metadata(type:typeof(TimeSpan), category:"System"),
                Metadata(type:typeof(object), category:"System"),
                Metadata(type:typeof(ExpandoObject), category:"System"),
                Metadata(type:typeof(IEnumerable<object>), category:"System"),
            ],
        },
    ];

    public MetadataContainerSet GetSharedMetadata() =>
        new()
        {
            Name = "Workflow",
            Types = GetKnownActivityTypes()
                .SelectMany(selector: set => set.Types)
                .OrderBy(keySelector: type => type.Name)
                .ToArray(),
        };

    private static MetadataContainerSet Set(string name, Type[] types) =>
        new()
        {
            Name = name,
            Types = types
                .Select(selector: type => Metadata(type: type, category: name))
                .OrderBy(keySelector: type => type.Name)
                .ToArray(),
        };

    private static ExtendedMetadataContainer Metadata(Type type, string category) =>
        new(type)
        {
            Category = category,
        };

    private static ExtendedMetadataContainer Entity<T>() =>
        new(typeof(T), isEntity: true, hasEndpoint: true)
        {
            Category = "Workflow",
        };
}