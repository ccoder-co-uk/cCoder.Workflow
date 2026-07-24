// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Workflow.Activities.Support;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Activities.Api;

namespace cCoder.Workflow.Activities.Activities.Templating;

public abstract class TemplatingActivity<T> : ApiActivity
{
    public int AppId { get; set; }

    public string Culture { get; set; }

    public string TemplateName { get; set; }

    public T Data { get; set; }

    public string Result { get; set; }

    public User User { get; set; }

    public override Task ExecuteInternal(IWorkflowContext context)
    {
        AppId = (int)context.Variables["AppId"];
        return base.ExecuteInternal(context:context);
    }

    protected async Task<App> GetApp(HttpClient api)
        => await api.Get<App>(query:$"ContentManagement/App({AppId})?$expand=MailServers");

    protected string BuildRenderQuery() =>
        $"ContentManagement/Template/Render()?appId={AppId}&name={Uri.EscapeDataString(stringToEscape:TemplateName ?? string.Empty)}&culture={Uri.EscapeDataString(stringToEscape:Culture ?? string.Empty)}";

    protected async Task<string> Render(HttpClient api)
    {
        try
        {
            using HttpResponseMessage response = await api.PostAsync(
requestUri:                BuildRenderQuery(),
content:                new StringContent(Data.ToJson(), Encoding.UTF8, "application/json"));

            _ = response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Log(level:WorkflowLogLevel.Error, message:"Template could not be rendered.\n" + ex.Message);
        }

        return string.Empty;
    }

    protected async Task<QueuedEmail> BuildEmailTo(string emailAddress, string subject, string serverName, HttpClient api)
    {
        try
        {
            App app = await GetApp(api:api);
            MailServer serverInfo = app.MailServers.FirstOrDefault(predicate:s => s.Name == serverName);

            if (serverInfo == null)
                throw new InvalidOperationException("Mail Server configuration could not be found.");

            string content = await Render(api:api);

            QueuedEmail result = new()
            {
                MailServerName = serverInfo.Name,
                To = emailAddress,
                Subject = subject,
                Content = content,
                IsBodyHtml = true,
                AppId = AppId
            };

            result.Content = result.Content.Replace(oldValue:"[email[subject]]", newValue:subject);
            result.Content = result.Content.Replace(oldValue:"[email[from]]", newValue:serverInfo.User);
            result.Content = result.Content.Replace(oldValue:"[email[to]]", newValue:emailAddress);

            return result;
        }
        catch (Exception ex)
        {
            Log(level:WorkflowLogLevel.Warning, message:"Template could not be rendered.\n" + ex.Message + "\n - " + ex.StackTrace);
            return null;
        }
    }
}