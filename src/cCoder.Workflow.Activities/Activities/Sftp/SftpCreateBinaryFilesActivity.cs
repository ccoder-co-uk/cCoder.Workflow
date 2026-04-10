using cCoder.Workflow.Activities.Models;
using ObjectPath = cCoder.Workflow.Activities.Support.Path;


namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpCreateBinaryFilesActivity : SftpActivity
{

    public string[] FullPaths { get; set; }

    public byte[] Contents { get; set; }

    public override Task ExecuteAsync()
    {
        SftpDo(client =>
        {
            for (int i = 0; i < FullPaths.Length; i++)
            {
                BuildPathAsync(client, new ObjectPath(FullPaths[i]).ParentPath);
                MemoryStream memoryStream = new(Contents[i]);
                client.UploadFile(memoryStream, FullPaths[i]);
                memoryStream.Dispose();
            }

            Log(WorkflowLogLevel.Info, "Upload Complete.");
        });

        return Task.FromResult(true);
    }
}




