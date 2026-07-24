// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;

namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpGetFolderContentsActivity : SftpBaseActivity
{
    public string Path { get; set; }

    public Dictionary<string, string> Result { get; set; }

    public override async Task ExecuteAsync() => SftpDo(operation:client =>
    {
        Result = new();

        foreach (var item in client.ListDirectory(Path).Where(f => !f.IsDirectory))
        {
            using MemoryStream stream = new();
            client.DownloadFile(item.FullName, stream);

            Result[item.Name] = Encoding.UTF8.GetString(stream.ToArray());
        }
    });
}