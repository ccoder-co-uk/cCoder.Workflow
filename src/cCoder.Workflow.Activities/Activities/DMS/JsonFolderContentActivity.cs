// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Activities.Models;
using Newtonsoft.Json;
using DmsFile = cCoder.Data.Models.DMS.File;


namespace cCoder.Workflow.Activities.Activities.DMS;

public class JsonFolderContentActivity : DMSActivity
{
    [IgnoreWhenFlowComplete]
    public string[] RawData { get; private set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    [JsonIgnore]
    public dynamic[] ParsedData => RawData?.Select(selector: i => cCoder.Workflow.Activities.Support.Data.ParseJson<dynamic>(data: i))
            .ToArray() ?? Array.Empty<dynamic>();

    [JsonIgnore]
    public dynamic[] FlattenedData => ParsedData?.Select(selector: i => cCoder.Workflow.Activities.Support.Data.Flatten(source: i))
            .ToArray() ?? Array.Empty<dynamic>();

    [IgnoreWhenFlowComplete]
    public DmsFile[] Files { get; set; }

    public override async Task ExecuteAsync()
    {
        using HttpClient api = GetHttpClient();

        Files = (await GetFilesWithContents(api: api)).ToArray();

        RawData = Files.Select(selector: f => ConvertToString(raw: f.Contents.OrderByDescending(c => c.Version)
            .FirstOrDefault()?.RawData))
            .ToArray();
    }

    protected async Task<IEnumerable<DmsFile>> GetFilesWithContents(HttpClient api)
    {
        string query = $"DocumentManagement/File?$filter=Folder/AppId eq {AppId} AND Folder/Path eq '{Path.Trim()
            .TrimEnd(trimChars: "/".ToCharArray())}' AND endswith(Name, '.json')&$expand=Contents";

        if (Page != null && PageSize != null)
        {
            query += $"&$top={PageSize}&$skip={(Page - 1) * PageSize}";
        }

        return await api.GetODataCollection<DmsFile>(query: query);
    }

    private string ConvertToString(byte[] raw) =>
        Encoding.UTF8.GetString(bytes: raw);
}