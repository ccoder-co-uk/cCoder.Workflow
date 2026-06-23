namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpDeleteFilesActivity : SftpBaseActivity
{
    public string[] Files { get; set; }

    public override async Task ExecuteAsync() => SftpDo(client =>
    {
        foreach (string file in Files)
            client.DeleteFile(file);
    });
}
