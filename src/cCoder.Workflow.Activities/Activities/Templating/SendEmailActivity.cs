// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Support;
using cCoder.Data.Models.Mail;
using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Activities.Templating;

public class SendEmailActivity : TemplatingActivity<dynamic>
{
    public string Subject { get; set; }
    public string MailServerName { get; set; }
    public string CC { get; set; }
    public string To { get; set; }

    public override async Task ExecuteAsync()
    {
        using System.Net.Http.HttpClient api = GetHttpClient();
        Log(level: WorkflowLogLevel.Info, message: $"Building Email ...");
        QueuedEmail email = await BuildEmailTo(emailAddress: To, subject: Subject, serverName: MailServerName, api: api);

        if (email != null)
        {
            Log(level: WorkflowLogLevel.Info, message: $"Email built, sending ...");
            _ = await api.AddAsync(query: $"Mail/QueuedEmail", entity: email);
            Log(level: WorkflowLogLevel.Info, message: $"Email Sent!");
        }
    }
}