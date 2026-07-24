// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;

namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpGetFolderContentsActivity : SftpBaseActivity
{
    public string Path { get; set; }

    public Dictionary<string, string> Result { get; set; }

    public override Task ExecuteAsync() =>
        SftpDo(
            operation: client =>
            {
                Result = [];

                IEnumerable<Renci.SshNet.Sftp.ISftpFile> files = client
                    .ListDirectory(
                        path: Path)
                    .Where(
                        predicate: file => !file.IsDirectory);

                foreach (Renci.SshNet.Sftp.ISftpFile file in files)
                {
                    using MemoryStream stream = new();

                    client.DownloadFile(
                        path: file.FullName,
                        output: stream);

                    byte[] content = stream.ToArray();

                    Result[file.Name] = Encoding.UTF8.GetString(
                        bytes: content);
                }
            });
}