// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Renci.SshNet.Sftp;

namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpGetListOfFilesActivity : SftpBaseActivity
{
    public string Path { get; set; }

    public string[] Result { get; set; }

    public override async Task ExecuteAsync() => SftpDo(client =>
    {
        IEnumerable<ISftpFile> items = client.ListDirectory(Path).Where(f => !f.IsDirectory);

        Result = items.Select(f => f.Name).ToArray();
    });
}