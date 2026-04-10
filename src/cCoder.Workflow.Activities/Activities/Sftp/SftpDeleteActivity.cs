using Renci.SshNet;
using Renci.SshNet.Sftp;
using ObjectPath = cCoder.Workflow.Activities.Support.Path;


namespace cCoder.Workflow.Activities.Activities.Sftp;

public class SftpDeleteActivity : SftpActivity
{
    public string[] Paths { get; set; }
    public string Result { get; set; }

    public override Task ExecuteAsync()
    {
        SftpDo(client =>
        {
            for (int i = 0; i < Paths.Length; i++)
            {
                if (!new ObjectPath(Paths[i]).IsToFile)
                {
                    DeleteDirectory(client, Paths[i]);
                }
                else
                {
                    client.DeleteFile(Paths[i]);
                }
            }

            Result = "Success";
        });

        return Task.CompletedTask;
    }

    private static void DeleteDirectory(SftpClient client, string path)
    {
        System.Collections.Generic.IEnumerable<SftpFile> itemsInPath = client.ListDirectory(path);
        foreach (SftpFile file in itemsInPath)
        {
            if (file.Name is not "." and not "..")
            {
                if (file.IsDirectory)
                {
                    DeleteDirectory(client, file.FullName);
                }
                else
                {
                    client.DeleteFile(file.FullName);
                }
            }
        }

        client.DeleteDirectory(path);
    }
}




