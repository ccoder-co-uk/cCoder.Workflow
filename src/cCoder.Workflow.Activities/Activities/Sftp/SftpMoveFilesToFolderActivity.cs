// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpMoveFilesToFolderActivity : SftpBaseActivity
{
    public string FromFolder { get; set; }

    public string ToFolder { get; set; }

    public List<string> Files { get; set; }

    public override Task ExecuteAsync() =>
        SftpDo(
            operation: client =>
            {
                FromFolder = FromFolder.Trim(
                    trimChar: '/');

                ToFolder = ToFolder.Trim(
                    trimChar: '/');

                if (!client.Exists(
                    path: ToFolder))
                {
                    client.CreateDirectory(
                        path: ToFolder);
                }

                foreach (string item in Files)
                {
                    client.RenameFile(
                        oldPath: $"{FromFolder}/{item}",
                        newPath: $"{ToFolder}/{item}");
                }
            });
}