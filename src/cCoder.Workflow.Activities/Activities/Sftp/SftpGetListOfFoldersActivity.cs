// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Renci.SshNet.Sftp;

namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpGetListOfFoldersActivity : SftpBaseActivity
{
    public string Path { get; set; }

    public string[] Result { get; set; }

    public override Task ExecuteAsync() =>
        SftpDo(
            operation: client =>
            {
                IEnumerable<ISftpFile> items = client
                    .ListDirectory(
                        path: Path)
                    .Where(
                        predicate: item => item.IsDirectory);

                Result = items
                    .Select(
                        selector: folder => folder.Name)
                    .ToArray();
            });
}