// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpDeleteFoldersActivity : SftpBaseActivity
{
    public string[] Folders { get; set; }

    public override async Task ExecuteAsync() => SftpDo(operation:client =>
    {
        foreach (string folder in Folders)
            client.DeleteDirectory(folder);
    });
}