// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.Cultures);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.Pages);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.Components);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.Scripts);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.Roles);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.Templates);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.Resources);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.Tasks);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.Calendars);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.Folders);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.Layouts);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.Flows);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.MailServers);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.MailQueue);
        BuilderSetup.DisablePropertyNamingFor(func: (App app) => app.SentMail);
        BuilderSetup.DisablePropertyNamingFor(func: (Page page) => page.App);
        BuilderSetup.DisablePropertyNamingFor(func: (Page page) => page.Parent);
        BuilderSetup.DisablePropertyNamingFor(func: (Page page) => page.PageInfo);
        BuilderSetup.DisablePropertyNamingFor(func: (Page page) => page.Pages);
        BuilderSetup.DisablePropertyNamingFor(func: (Page page) => page.Contents);
        BuilderSetup.DisablePropertyNamingFor(func: (Page page) => page.Roles);
        BuilderSetup.DisablePropertyNamingFor(func: (Resource resource) => resource.App);
        BuilderSetup.DisablePropertyNamingFor(func: (Role role) => role.App);
        BuilderSetup.DisablePropertyNamingFor(func: (Role role) => role.Users);
        BuilderSetup.DisablePropertyNamingFor(func: (Role role) => role.Pages);
        BuilderSetup.DisablePropertyNamingFor(func: (Role role) => role.Folders);
        BuilderSetup.DisablePropertyNamingFor(func: (User user) => user.DefaultCulture);
        BuilderSetup.DisablePropertyNamingFor(func: (User user) => user.Roles);
        BuilderSetup.DisablePropertyNamingFor(func: (QueuedEmail queuedEmail) => queuedEmail.FailedSends);

        BuilderSetup.DisablePropertyNamingFor(
func: (FlowDefinition flowDefinition) => flowDefinition.App
        );

        BuilderSetup.DisablePropertyNamingFor(
func: (FlowDefinition flowDefinition) => flowDefinition.Instances
        );

        BuilderSetup.DisablePropertyNamingFor(func: (WorkflowEvent workflowEvent) => workflowEvent.Flow);

        BuilderSetup.DisablePropertyNamingFor(
func: (WorkflowEvent workflowEvent) => workflowEvent.ExecuteAsUser
        );

        BuilderSetup.DisablePropertyNamingFor(func: (FileContent fileContent) => fileContent.File);
        BuilderSetup.DisablePropertyNamingFor(func: (Submission submission) => submission.App);
        BuilderSetup.DisablePropertyNamingFor(func: (ScheduledTask scheduledTask) => scheduledTask.Flow);

        BuilderSetup.DisablePropertyNamingFor(
func: (ScheduledTask scheduledTask) => scheduledTask.ExecuteAsUser
        );

        BuilderSetup.DisablePropertyNamingFor(func: (MailServer mailServer) => mailServer.App);
        BuilderSetup.DisablePropertyNamingFor(func: (Layout layout) => layout.App);
        BuilderSetup.DisablePropertyNamingFor(func: (Script script) => script.App);
        BuilderSetup.DisablePropertyNamingFor(func: (Content content) => content.Page);

        BuilderSetup.DisablePropertyNamingFor(
func: (CalendarEvent calendarEvent) => calendarEvent.Calendar
        );

        BuilderSetup.DisablePropertyNamingFor(func: (PageRole pageRole) => pageRole.Page);
        BuilderSetup.DisablePropertyNamingFor(func: (PageRole pageRole) => pageRole.Role);
        BuilderSetup.DisablePropertyNamingFor(func: (UserRole userRole) => userRole.User);
        BuilderSetup.DisablePropertyNamingFor(func: (UserRole userRole) => userRole.Role);
    }
}