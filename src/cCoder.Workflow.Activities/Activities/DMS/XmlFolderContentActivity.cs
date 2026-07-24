// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO.Compression;
using System.Text;
using cCoder.Workflow.Activities.Models;
using Newtonsoft.Json;
using File = cCoder.Data.Models.DMS.File;


namespace cCoder.Workflow.Activities.Activities.DMS;

public class XmlFolderContentActivity : DMSActivity
{
    [IgnoreWhenFlowComplete]
    public string[] RawData { get; private set; }

    [IgnoreWhenFlowComplete]
    public IEnumerable<File> Files { get; set; }

    [JsonIgnore]
    public dynamic[] ParsedData => RawData?.Select(selector: i => cCoder.Workflow.Activities.Support.Data.ParseXml<dynamic>(data: i)).ToArray();

    [JsonIgnore]
    public dynamic[] FlattenedData => ParsedData?.Select(selector: i => cCoder.Workflow.Activities.Support.Data.Flatten(source: i)).ToArray();

    public override async Task ExecuteAsync()
    {
        using HttpClient api = GetHttpClient();

        api.Timeout = TimeSpan.FromMinutes(minutes: 10);

        (string file, string xml)[] data = (await GetFilesWithContents(api: api)).ToArray();

        Files = data.Select(selector: d => new File
        {
            Name = d.file,
            Path = $"{Path.Trim().TrimEnd(trimChars: "/".ToCharArray())}/{d.file}"
        });
        RawData = data.Select(selector: d => d.xml).ToArray();
    }

    private string ConvertToString(byte[] raw) =>
        Encoding.UTF8.GetString(bytes: raw);

    protected async Task<IEnumerable<(string, string)>> GetFilesWithContents(HttpClient api)
    {
        string path = Path.Trim().TrimEnd(trimChars: "/".ToCharArray());

        using Stream result = await api.GetStreamAsync(requestUri: $"DMS/{path}");
        using ZipArchive folderArchive = new(result);

        List<(string, string)> results = [];

        foreach (ZipArchiveEntry entry in folderArchive.Entries)
        {
            if (entry.Name.EndsWith(value: ".xml"))
            {
                using Stream entryStream = entry.Open();
                using StreamReader entryReader = new(entryStream);
                results.Add(item: (entry.Name, entryReader.ReadToEnd()));
            }
        }

        return results;
    }

    private string Now() => DateTimeOffset.UtcNow.ToString(format: "HH:mm:ss");
}