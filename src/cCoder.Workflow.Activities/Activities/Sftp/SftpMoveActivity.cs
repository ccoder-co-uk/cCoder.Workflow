using ObjectPath = cCoder.Workflow.Activities.Support.Path;


namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpMoveActivity : SftpActivity
{
    public string[] SourcePaths { get; set; }
    public string[] DestinationPaths { get; set; }

    public override Task ExecuteAsync()
    {
        SftpDo(client =>
        {
            for (int i = 0; i < SourcePaths.Length; i++)
            {
                BuildPathAsync(client, new ObjectPath(DestinationPaths[i]).ParentPath);
                client.RenameFile(SourcePaths[i], DestinationPaths[i]);
            }
        });

        return Task.FromResult(true);
    }
}




