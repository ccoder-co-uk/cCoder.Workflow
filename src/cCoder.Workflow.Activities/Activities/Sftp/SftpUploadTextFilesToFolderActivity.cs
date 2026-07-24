// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;

namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpUploadTextFilesToFolderActivity : SftpBaseActivity
{
    public string Path { get; set; }

    public Dictionary<string, string> Files { get; set; }

    public override Task ExecuteAsync() =>
        SftpDo(
            operation: client =>
            {
                if (!client.Exists(
                    path: Path))
                {
                    client.CreateDirectory(
                        path: Path);
                }

                foreach (KeyValuePair<string, string> item in Files)
                {
                    byte[] content = Encoding.UTF8.GetBytes(
                        s: item.Value);

                    using MemoryStream stream = new(
                        buffer: content);

                    client.UploadFile(
                        input: stream,
                        path: $"{Path}/{item.Key}");
                }
            });
}