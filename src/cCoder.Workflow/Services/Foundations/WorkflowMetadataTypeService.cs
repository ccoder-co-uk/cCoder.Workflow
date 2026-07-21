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
        Set("ApiActivity",
        [
            typeof(ApiPostBatch),
            typeof(ApiDelete<object>),
            typeof(ApiGet<object>),
            typeof(ApiGetCollection<object>),
            typeof(ApiPost<object, object>),
            typeof(ApiPut<object, object>),
            typeof(AuthenticateActivity),
        ]),
        Set("DMSActivity",
        [
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
        Set("LogActivity",
        [
            typeof(DebugActivity),
            typeof(ErrorActivity),
            typeof(InfoActivity),
            typeof(WarningActivity),
        ]),
        Set("SftpActivity",
        [
            typeof(SftpDeleteFilesActivity),
            typeof(SftpDeleteFoldersActivity),
            typeof(SftpGetFolderContentsActivity),
            typeof(SftpGetListOfFilesActivity),
            typeof(SftpGetListOfFoldersActivity),
            typeof(SftpMoveFilesToFolderActivity),
            typeof(SftpUploadTextFilesToFolderActivity),
        ]),
        Set("TemplatingActivity",
        [
            typeof(PageBuilder),
            typeof(SendEmailActivity),
        ]),
        Set("TransformationActivity",
        [
            typeof(ConvertActivity<object, object>),
            typeof(CsvXslActivity<object>),
            typeof(DynamicDataFlattenActivity),
            typeof(JsonXslActivity<object>),
            typeof(XmlXslActivity<object>),
            typeof(XslActivity),
        ]),
        Set("Workflow",
        [
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
                Metadata(typeof(int), "System"),
                Metadata(typeof(string), "System"),
                Metadata(typeof(decimal), "System"),
                Metadata(typeof(float), "System"),
                Metadata(typeof(bool), "System"),
                Metadata(typeof(DateTime), "System"),
                Metadata(typeof(DateTimeOffset), "System"),
                Metadata(typeof(TimeSpan), "System"),
                Metadata(typeof(object), "System"),
                Metadata(typeof(ExpandoObject), "System"),
                Metadata(typeof(IEnumerable<object>), "System"),
            ],
        },
    ];

    public MetadataContainerSet GetSharedMetadata() =>
        new()
        {
            Name = "Workflow",
            Types = GetKnownActivityTypes()
                .SelectMany(set => set.Types)
                .OrderBy(type => type.Name)
                .ToArray(),
        };

    private static MetadataContainerSet Set(string name, Type[] types) =>
        new()
        {
            Name = name,
            Types = types
                .Select(type => Metadata(type, name))
                .OrderBy(type => type.Name)
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


