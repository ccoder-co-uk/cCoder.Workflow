namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpDeleteFoldersActivity : SftpBaseActivity
{
    public string[] Folders { get; set; }

    public override async Task ExecuteAsync() => SftpDo(client =>
    {
        foreach (string folder in Folders)
            client.DeleteDirectory(folder);
    });
}
