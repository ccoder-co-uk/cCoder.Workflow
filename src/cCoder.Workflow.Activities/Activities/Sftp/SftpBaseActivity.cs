// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using Renci.SshNet;

namespace cCoder.Workflow.Activities.Activities.Sftp;

public abstract class SftpBaseActivity : Activity
{
    public string Host { get; set; }

    public int Port { get; set; } = 22;

    public string Username { get; set; }

    public string Password { get; set; }

    public string PrivateKey { get; set; }

    public string PrivateKeyEncryptionKey { get; set; }

    protected void SftpDo(Action<SftpClient> operation)
    {
        ConnectionInfo auth = null;
        MemoryStream keyStream = null;
        PrivateKeyFile keyFile = null;

        if(string.IsNullOrEmpty(PrivateKey))
            auth = new(Host, Port, Username, new PasswordAuthenticationMethod(Username, Encoding.UTF8.GetBytes(Password)));
        else
        {
            keyStream = new(Encoding.UTF8.GetBytes(PrivateKey));
            keyFile = string.IsNullOrEmpty(PrivateKeyEncryptionKey)
                ? new(keyStream)
                : new(keyStream, PrivateKeyEncryptionKey);

            auth = new(Host, Port, Username, new PrivateKeyAuthenticationMethod(Username, keyFile));
        }

        SftpClient client = new(auth);

        try
        {
            client.Connect();
            Log(WorkflowLogLevel.Info, $"Connected to Server @ {Host} as User {Username}");
            operation(client);
        }
        catch(Exception ex)
        {
            Log(WorkflowLogLevel.Error, $"Error: {ex.Message}\n{ex.StackTrace}");

            if (ex.InnerException is not null)
                Log(WorkflowLogLevel.Error, $"Inner exception: {ex.InnerException.Message}\n{ex.InnerException.StackTrace}");

            State = ActivityState.Failed;
        }
        finally
        {
            if (client.IsConnected)
                client.Disconnect();

            client.Dispose();
            keyFile?.Dispose();
            keyStream?.Dispose();
            Log(WorkflowLogLevel.Info, $"Disconnected from Server @ {Host}");
        }
    }

    protected T SftpDo<T>(Func<SftpClient, T> operation)
    {
        ConnectionInfo auth = null;
        MemoryStream keyStream = null;
        PrivateKeyFile keyFile = null;

        if (string.IsNullOrEmpty(PrivateKey))
            auth = new(Host, Port, Username, new PasswordAuthenticationMethod(Username, Encoding.UTF8.GetBytes(Password)));
        else
        {
            keyStream = new(Encoding.UTF8.GetBytes(PrivateKey));
            keyFile = string.IsNullOrEmpty(PrivateKeyEncryptionKey)
                ? new(keyStream)
                : new(keyStream, PrivateKeyEncryptionKey);

            auth = new(Host, Port, Username, new PrivateKeyAuthenticationMethod(Username, keyFile));
        }

        SftpClient client = new(auth);

        try
        {
            client.Connect();
            Log(WorkflowLogLevel.Info, $"Connected to Server @ {Host} as User {Username}");
            return operation(client);
        }
        catch (Exception ex)
        {
            Log(WorkflowLogLevel.Error, $"Error: {ex.Message}\n{ex.StackTrace}");

            if (ex.InnerException is not null)
                Log(WorkflowLogLevel.Error, $"Inner exception: {ex.InnerException.Message}\n{ex.InnerException.StackTrace}");

            State = ActivityState.Failed;
        }
        finally
        {
            if (client.IsConnected)
                client.Disconnect();

            client.Dispose();
            keyFile?.Dispose();
            keyStream?.Dispose();
            Log(WorkflowLogLevel.Info, $"Disconnected from Server @ {Host}");
        }

        return default;
    }
}