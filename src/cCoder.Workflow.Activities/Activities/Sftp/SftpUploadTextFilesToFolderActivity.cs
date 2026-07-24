// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;

namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpUploadTextFilesToFolderActivity : SftpBaseActivity
{
    public string Path { get; set; }

    public Dictionary<string, string> Files { get; set; }

    public override async Task ExecuteAsync() => SftpDo(operation:client =>
    {
        if(!client.Exists(Path))
            client.CreateDirectory(Path);

        foreach (var item in Files)
            client.UploadFile(new MemoryStream(Encoding.UTF8.GetBytes(item.Value)), $"{Path}/{item.Key}");
    });
}