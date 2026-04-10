using System.Runtime.CompilerServices;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FizzWare.NBuilder;


namespace cCoder.Core.Services.Tests;

internal static class TestBuilderSetup
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        BuilderSetup.DisablePropertyNamingFor((App app) => app.Cultures);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.Pages);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.Components);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.Scripts);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.Roles);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.Templates);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.Resources);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.Tasks);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.Calendars);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.Folders);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.Layouts);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.Flows);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.MailServers);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.MailQueue);
        BuilderSetup.DisablePropertyNamingFor((App app) => app.SentMail);
        BuilderSetup.DisablePropertyNamingFor((Page page) => page.App);
        BuilderSetup.DisablePropertyNamingFor((Page page) => page.Parent);
        BuilderSetup.DisablePropertyNamingFor((Page page) => page.PageInfo);
        BuilderSetup.DisablePropertyNamingFor((Page page) => page.Pages);
        BuilderSetup.DisablePropertyNamingFor((Page page) => page.Contents);
        BuilderSetup.DisablePropertyNamingFor((Page page) => page.Roles);
        BuilderSetup.DisablePropertyNamingFor((Resource resource) => resource.App);
        BuilderSetup.DisablePropertyNamingFor((Role role) => role.App);
        BuilderSetup.DisablePropertyNamingFor((Role role) => role.Users);
        BuilderSetup.DisablePropertyNamingFor((Role role) => role.Pages);
        BuilderSetup.DisablePropertyNamingFor((Role role) => role.Folders);
        BuilderSetup.DisablePropertyNamingFor((User user) => user.DefaultCulture);
        BuilderSetup.DisablePropertyNamingFor((User user) => user.Roles);
        BuilderSetup.DisablePropertyNamingFor((QueuedEmail queuedEmail) => queuedEmail.FailedSends);
        BuilderSetup.DisablePropertyNamingFor(
            (FlowDefinition flowDefinition) => flowDefinition.App
        );
        BuilderSetup.DisablePropertyNamingFor(
            (FlowDefinition flowDefinition) => flowDefinition.Instances
        );
        BuilderSetup.DisablePropertyNamingFor((WorkflowEvent workflowEvent) => workflowEvent.Flow);
        BuilderSetup.DisablePropertyNamingFor(
            (WorkflowEvent workflowEvent) => workflowEvent.ExecuteAsUser
        );
        BuilderSetup.DisablePropertyNamingFor((FileContent fileContent) => fileContent.File);
        BuilderSetup.DisablePropertyNamingFor((Submission submission) => submission.App);
        BuilderSetup.DisablePropertyNamingFor((ScheduledTask scheduledTask) => scheduledTask.Flow);
        BuilderSetup.DisablePropertyNamingFor(
            (ScheduledTask scheduledTask) => scheduledTask.ExecuteAsUser
        );
        BuilderSetup.DisablePropertyNamingFor((MailServer mailServer) => mailServer.App);
        BuilderSetup.DisablePropertyNamingFor((Layout layout) => layout.App);
        BuilderSetup.DisablePropertyNamingFor((Script script) => script.App);
        BuilderSetup.DisablePropertyNamingFor((Content content) => content.Page);
        BuilderSetup.DisablePropertyNamingFor(
            (CalendarEvent calendarEvent) => calendarEvent.Calendar
        );
        BuilderSetup.DisablePropertyNamingFor((PageRole pageRole) => pageRole.Page);
        BuilderSetup.DisablePropertyNamingFor((PageRole pageRole) => pageRole.Role);
        BuilderSetup.DisablePropertyNamingFor((UserRole userRole) => userRole.User);
        BuilderSetup.DisablePropertyNamingFor((UserRole userRole) => userRole.Role);
    }
}





