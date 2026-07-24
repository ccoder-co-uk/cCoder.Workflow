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
        Log(WorkflowLogLevel.Info, $"Building Email ...");
        QueuedEmail email = await BuildEmailTo(To, Subject, MailServerName, api);
        if (email != null)
        {
            Log(WorkflowLogLevel.Info, $"Email built, sending ...");
            _ = await api.AddAsync($"Mail/QueuedEmail", email);
            Log(WorkflowLogLevel.Info, $"Email Sent!");
        }
    }
}