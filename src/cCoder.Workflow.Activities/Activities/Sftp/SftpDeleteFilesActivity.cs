// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpDeleteFilesActivity : SftpBaseActivity
{
    public string[] Files { get; set; }

    public override async Task ExecuteAsync() => SftpDo(operation:client =>
    {
        foreach (string file in Files)
            client.DeleteFile(file);
    });
}