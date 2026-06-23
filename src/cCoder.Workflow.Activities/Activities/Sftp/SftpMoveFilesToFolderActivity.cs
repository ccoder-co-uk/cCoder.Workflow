using System;
using System.Collections.Generic;
using System.Text;

namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpMoveFilesToFolderActivity : SftpBaseActivity
{
    public string FromFolder { get; set; }

    public string ToFolder { get; set; }

    public List<string> Files { get; set; }

    public override async Task ExecuteAsync() => SftpDo(client =>
    {
        FromFolder = FromFolder.Trim('/');
        ToFolder = ToFolder.Trim('/');

        if (!client.Exists(ToFolder))
            client.CreateDirectory(ToFolder);

        foreach (string item in Files)
            client.RenameFile($"{FromFolder}/{item}", $"{ToFolder}/{item}");
    });
}
