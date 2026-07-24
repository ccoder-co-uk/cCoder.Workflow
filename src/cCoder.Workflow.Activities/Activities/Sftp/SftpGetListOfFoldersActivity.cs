// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Renci.SshNet.Sftp;

namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpGetListOfFoldersActivity : SftpBaseActivity
{
    public string Path { get; set; }

    public string[] Result { get; set; }

    public override async Task ExecuteAsync() => SftpDo(operation:client =>
    {
        IEnumerable<ISftpFile> items = client.ListDirectory(Path).Where(i => i.IsDirectory);

        Result = items.Select(f => f.Name).ToArray();
    });
}