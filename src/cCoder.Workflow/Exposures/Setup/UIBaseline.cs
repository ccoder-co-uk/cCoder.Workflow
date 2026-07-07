using cCoder.Data.Models.Packaging;

namespace cCoder.Workflow.Exposures.Setup;

public static class UIBaseline
{
    public static Package[] Packages => [
        Components,
        Pages,
        FlowDefinitions,
        Calendars,
        PageRoles
    ];

    static Package Components => new()
    {
        Name = "Workflow Components",
        Category = "Workflow",
        Description = "Workflow Components.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "FlowInstanceDetails",
  "Key": "Workflow",
  "ResourceKey": "Workflow",
  "Script": "FlowInstanceDetails = {\n\tinit: async function (app, container, instance, reportComponent) {\n\t\tapp = app || session.app;\n\t\tcontainer = container || $(\".component[name=FlowInstanceDetails]\");\n\t\tif(!instance)\n            return;\n        \n        var id = FlowInstanceDetails.initNavTabIds(app, container);\n\n\t\tif (instance.State !== \"Executing\") {\n\t\t\tif (!reportComponent) {\n\t\t\t\t$(`#flow-instance-details-report-tab-${id}`, container).hide();\n\t\t\t}\n            \n\t\t\tawait api.get(\"Workflow/FlowInstanceData(\" + instance.Id + \")\").then(function (i) {\n\t\t\t\tif (reportComponent) {\n\t\t\t\t\tvar reportTab = $(\"[name=report]\", container);\n\t\t\t\t\tloadComponent(reportTab, reportComponent, function (c) {\n                        window[reportComponent].init(app, reportTab, i);\n                    });\n\t\t\t\t}\n\t\t\t\tif (i.ContextString) {\n\t\t\t\t\tvar details = JSON.parse(i.ContextString);\n\t\t\t\t\tFlowInstanceDetails.initLog(app, $(\"[name=log]\", container), details.ExecutionLog);\n\t\t\t\t\tFlowInstanceDetails.initFlow(details, $(\"[name=flow] > .flow\", container));\n\t\t\t\t\tfor (var v in details.Variables) {\n\t\t\t\t\t\tvar item = \"<li><label>\" + v + \"</label><div class='value'>\" + details.Variables[v] + \"</div></li>\";\n\n\t\t\t\t\t\tif (item.includes(\"[object Object]\")) {\n\t\t\t\t\t\t\titem = \"<li><label>\" + v + \"</label><div class='value'>\";\n\t\t\t\t\t\t\titem += \"<textarea name='\" + v + \"'>\" + JSON.stringify(details.Variables[v], null, 4) + \"</textarea>\";\n\t\t\t\t\t\t\titem += \"</div></li>\";\n\t\t\t\t\t\t}\n\n\t\t\t\t\t\t$(\"[name=variables]\", container).append(item);\n\t\t\t\t\t}\n\t\t\t\t}\n\t\t\t}).catch((err) => {\n\t\t\t\tnotification.error(err);\n\t\t\t});\n\t\t} else {\n            // If the workflow is executing, connect to it.\n\t\t\tawait FlowInstanceDetails.connectToWorkflow(app, container,container, instance, reportComponent);\n\t\t}\n\t},\n\n    initNavTabIds: function(app, container) {\n        var id = Guid();\n        var template = $('script[name=flow-instance-details-tabs]', container).html();\n        template = template.replaceAll('{ID}', id);\n\n        container.append(template);\n\n\t\treturn id;\n    },\n\n\tconnectToHub: async function(console, callback, completeCallback) {\n\t\tvar connection = new signalR.HubConnectionBuilder().withUrl(session.apiRoot + \"Hubs/Workflow\").build();\n\t\tconnection.on(\"ConsoleReceive\", function (level, message, instance) {\n\t\t\tvar d = new Date();\n\t\t\tvar time = d.getHours() + \":\" + d.getMinutes() + \":\" + d.getSeconds();\n\t\t\tvar logHtml = FlowInstanceDetails.buildMessage({\n\t\t\t\tLevel: level,\n\t\t\t\tTimestamp: new Date(),\n\t\t\t\tMessage: message\n\t\t\t});\n\t\t\t$(\".execConsole > [name=flowConsole]\", console).append(logHtml);\n\t\t\tif (level == \"info\" && message == \"Done!\") {\n\t\t\t\tconnection.invoke(\"leave\", instance);\n\t\t\t\tnotification.info(\"Disconnected from workflow hub.\");\n\t\t\t\tif (completeCallback)\n                    completeCallback(instance, console, connection);\n\t\t\t}\n\t\t});\n\t\tawait connection.start();\n\t\tcallback(console, connection);\n\t},\n\n\tconnectToWorkflow: async function (app, container, console, instanceData, reportComponent) {\n\t\tawait FlowInstanceDetails.connectToHub(console, (console, connection) => {\n\t\t\tconnection.invoke(\"join\", instanceData.Id);\n\t\t\tconnection.invoke(\"ConsoleSend\", \"info\", \"Client Connected to instance \" + instanceData.Id, instanceData.Id);\n\t\t}, async (finalInstanceData,cconsole,connection) => {\n\t\t\tawait FlowInstanceDetails.init(app, container, finalInstanceData, reportComponent);\n\t\t});\n\t},\n\n\tinitLog: function (app, container, log) {\n\t\tvar entries = log.map(i => FlowInstanceDetails.buildMessage(i));\n\t\t$(\".execConsole > [name=flowConsole]\", container).append(entries);\n\t},\n\n\tinitFlow: function (instance, container) {\n\t\tvar executionOrder = instance.ExecutionLog\n\t\t\t.filter(l => l.Message.indexOf(\"::\") !== -1 && l.Message.endsWith(\"Activity Execution started\"))\n\t\t\t.map(l => l.Message.split(\":\")[0]);\n\n\t\texecutionOrder.map(ex => {\n\t\t\tvar activity = instance.Flow.Activities.filter(a => a.Ref === ex)[0];\n\t\t\tvar log = instance.ExecutionLog.filter(l => l.Message.startsWith(activity.Ref + \"::\"));\n\t\t\tFlowInstanceDetails.initActivity(container, instance.Flow, activity, log);\n\t\t});\n\n\t\t$(\".activity > .in > \").on(\"hover\", function (a) {\n\t\t\t$(\".tip[name=\" + $(a).attr(\"name\") + \"]\").css(\"display\", \"block\");\n\t\t});\n\t},\n\n\tinitActivity: function (container, flow, activity, log) {\n\t\tvar aRef = activity.Ref.replaceAll(' ', '').replaceAll(\"'\", \"\");\n\t\tvar links = flow.Links.filter(l => l.Source === activity.Ref);\n\t\tvar linkCode = links.map(l => l.Expression).join(\"\\n\");\n\n\t\tvar element =\n\t\t\t\"<div name='\" + aRef + \"' class='activity state-\" + activity.State + \"'>\" +\n\t\t\t\"<h4><span class='k-icon k-i-gears'></span>\" + activity.Ref + \"</h4><div class='in'>\" +\n\t\t\t\"<a href='' class='state'><span class='k-icon k-i-css'></span>View State</a>\" +\n\t\t\t\"<a href='' class='log'><span class='k-icon k-i-txt'></span>View Log</a>\" +\n\t\t\t\"</div></div><div class='sep'><span class='k-icon k-i-forward'></span></div>\";\n\n\t\tcontainer.append(element);\n\n\t\tvar console = \"<div name='\" + aRef + \"'>\" +\n\t\t\t\"<div class='execConsole' name='execConsole'>\" +\n\t\t\t\"<div class='console' name='flowConsole'>\";\n\n\t\tlog.map(i => console += FlowInstanceDetails.buildMessage(i));\n\n\t\tconsole += \" </div></div></div>\";\n\n\t\t$(document.body).append($(\"<div name='\" + aRef + \"State'><textarea class='activityJson'>\" + JSON.stringify(activity, null, 4) + \"</textarea></div>\"));\n\t\tvar dialog1 = $(\"[name='\" + aRef + \"State']\");\n\t\tvar d1 = dialog1.kendoWindow({\n\t\t\tvisible: false,\n\t\t\tmodal: true,\n\t\t\tresizable: false,\n\t\t\twidth: 800,\n\t\t\theight: 700,\n\t\t\ttitle: \"[resource_displayname[AcitvityLog]]\"\n\t\t}).data(\"kendoWindow\");\n\n\t\t$(\".activity[name=\" + aRef + \"] a.state\").on(\"click\", function (e) {\n\t\t\te.preventDefault();\n\t\t\td1.center().open();\n\t\t});\n\n\t\t$(document.body).append($(\"<div name='\" + aRef + \"Log'>\" + console + \"</div>\"));\n\t\tvar dialog2 = $(\"[name='\" + aRef + \"Log']\");\n\t\tvar d2 = dialog2.kendoWindow({\n\t\t\tvisible: false,\n\t\t\tmodal: true,\n\t\t\tresizable: false,\n\t\t\twidth: 800,\n\t\t\theight: 700,\n\t\t\ttitle: \"[resource_displayname[AcitvityLog]]\"\n\t\t}).data(\"kendoWindow\");\n\n\t\t$(\".activity[name=\" + aRef + \"] a.log\").on(\"click\", function (e) {\n\t\t\te.preventDefault();\n\t\t\td2.center().open();\n\t\t});\n\t},\n\n\tbuildMessage: function (logEntry) {\n\t\tvar time = new Date(logEntry.Timestamp);\n\t\treturn \"<div class='message \" + logEntry.Level.toLowerCase() + \"'>\" +\n\t\t\t\"<span class='time'>\" + time.getHours() + \":\" + time.getMinutes() + \":\" + time.getSeconds() + \"</span>\" +\n\t\t\t\"<pre class='message'>\" + html.encode(logEntry.Message) + \"</pre>\" +\n\t\t\t\"</div>\";\n\t}\n}",
  "Content": "<script type=\"text/template\" name=\"flow-instance-details-tabs\">\n<div class=\"tab-control\" name=\"tabs\">\n\t<nav>\n\t\t<div class=\"nav nav-tabs\" id=\"flow-instance-details-nav-tab-{ID}\" role=\"tablist\">\n\t\t\t<button class=\"nav-link bg active\" id=\"flow-instance-details-log-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"#flow-instance-details-log-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"flow-instance-details-proposed-{ID}\" aria-selected=\"true\">\n\t\t\t\t[resource_displayname[log]]\n\t\t\t</button>\n\t\t\t<button class=\"nav-link bg\" id=\"flow-instance-details-context-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"#flow-instance-details-context-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"flow-instance-details-accepted-{ID}\" aria-selected=\"false\" tabindex=\"-1\">\n\t\t\t\t[resource_displayname[context]]\n\t\t\t</button>\n\t\t\t<button class=\"nav-link bg\" id=\"flow-instance-details-flow-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"#flow-instance-details-flow-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"flow-instance-details-accepted-{ID}\" aria-selected=\"false\" tabindex=\"-1\">\n\t\t\t\t[resource_displayname[flow]]\n\t\t\t</button>\n\t\t\t<button class=\"nav-link bg\" id=\"flow-instance-details-report-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"#flow-instance-details-report-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"flow-instance-details-accepted-{ID}\" aria-selected=\"false\" tabindex=\"-1\">\n\t\t\t\t[resource_displayname[report]]\n\t\t\t</button>\n\t\t</div>\n\t</nav>\n\n\t<div class=\"tab-content\" id=\"flow-instance-details-nav-tab-{ID}Content\">\n\t\t<div class=\"tab-pane fade active show\" id=\"flow-instance-details-log-{ID}\" role=\"tabpanel\" aria-labelledby=\"flow-instance-details-log-tab-{ID}\" name=\"log\">\n            <div class=\"execConsole\" name=\"execConsole\">\n                <div class=\"console\" name=\"flowConsole\"></div>\n            </div>\n        </div>\n\t\t<div class=\"tab-pane fade\" id=\"flow-instance-details-context-{ID}\" role=\"tabpanel\" aria-labelledby=\"flow-instance-details-context-tab-{ID}\" name=\"context\">\n            <h4>[resource_displayname[variables]]</h4>\n            <ul class=\"fieldList\" name=\"variables\"></ul>\n        </div>\n\t\t<div class=\"tab-pane fade\" id=\"flow-instance-details-flow-{ID}\" role=\"tabpanel\" aria-labelledby=\"flow-instance-details-flow-tab-{ID}\" name=\"flow\">\n\t\t\t<div class=\"flow\"></div>\n\t\t</div>\n\t\t<div class=\"tab-pane fade\" id=\"flow-instance-details-report-{ID}\" role=\"tabpanel\" aria-labelledby=\"flow-instance-details-report-tab-{ID}\" name=\"report\"></div>\n\t</div>\n</div>\n</script>",
  "LastUpdated": "2024-11-19T18:18:31.2467939+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "FlowInstanceManagement",
  "Key": "Workflow",
  "ResourceKey": "Workflow",
  "Script": "FlowInstanceManagement = {\n    states: {\n        \"Not Run\": \"[resource_displayname[notrun]]\",\n        \"Executing\": \"[resource_displayname[executing]]\",\n        \"Complete\": \"[resource_displayname[complete]]\",\n        \"Suspended\": \"[resource_displayname[suspended]]\",\n        \"Queued\": \"[resource_displayname[queued]]\",\n        \"Failed\": \"[resource_displayname[failed]]\"\n    },\n\n    init: async function (app, container, definitionId, reportComponentName) {\n        app = app || session.app;\n        container = container || $('.component[name=FlowInstanceManagement]');\n        \n        if(!definitionId)\n            return;\n        \n        api.addToMetaCache([\n            {\n                \"Name\": \"Workflow\",\n                \"Types\": [\n                    [meta[Workflow/FlowInstanceData]]\n                ]\n            }\n        ]);\n        var config = {\n            endpoint: \"Workflow/FlowInstanceData\",\n            odataAppend: \"?$filter=FlowDefinitionId eq \" + definitionId + \"&$select=Id,Name,State,Start,End,Caller\",\n            pageSize: 20\n        };\n        var ds = await model.getDatasource(config);\n        var grid = new GridWidget(container, ds);\n        grid.toolbar = \"<button class='btn btn-sm btn-primary' name='deleteAll'><span class='k-icon k-i-trash'></span>[resource_displayname[deleteall]]</button>\";\n        grid.scrollable = false;\n        grid.columns = [\n            { field: \"Id\", title: \"[resource_displayname[id]]\" },\n            { field: \"State\", title: \"[resource_displayname[state]]\", template: \"#=FlowInstanceManagement.states[State]#\"},\n            { field: \"Caller\", title: \"[resource_displayname[caller]]\" },\n            { field: \"Start\", width: 130, format: \"{0:\" + type.dateFormat + \" HH:mm}\", title: \"[resource_displayname[start]]\" },\n            { field: \"End\", width: 130, format: \"{0:\" + type.dateFormat + \" HH:mm}\", title: \"[resource_displayname[end]]\" },\n        ];\n        grid.commands.push({name: \"delete\", icon: \"k-i-trash\", text: \"[resource_displayname[delete]]\"});\n        grid.dataBound = function (e) {\n            $('[name=delete]', grid.gridElement).off(\"click\").on(\"click\", async (e) => await FlowInstanceManagement.deleteInstance(e, grid));\n        };\n        grid.detailTemplate = \"<div name='details'></div>\";\n        grid.detailExpand = function (e) {\n            var instance = grid.dataItem(e.masterRow);\n            if ($(\"[name=details]\", e.detailRow)[0].childElementCount === 0) {\n                var subContainer = $(\"[name=details]\", e.detailRow);\n                subContainer.html($(\"[name=flowInstanceDetailsComponent]\").first().html());\n                FlowInstanceDetails.init(app, $(\".component[name=FlowInstanceDetails]\", subContainer), instance, reportComponentName);\n            }\n        };\n        grid.groupable = false;\n        await grid.init();\n        grid.kendoObject.dataSource.sort({field: \"Start\", dir: \"desc\"});\n        $(\"[name=deleteAll]\", grid.gridElement).on(\"click\", async (e) => FlowInstanceManagement.deleteAll(e, definitionId, grid));\n    },\n\n    deleteAll: async function(e, definitionId, grid) {\n        e.preventDefault();\n        notification.info(\"[resource_displayname[deleting]]\");\n        await api.get(\"Workflow/FlowInstanceData?$filter=FlowDefinitionId eq \" + definitionId+\"&$select=Id\").then(async (data) => {\n            var flowInstances = data.value.map(c => c.Id);\n\n            for(var i in flowInstances)\n                await api.destroy('Workflow/FlowInstanceData(' + flowInstances[i] + ')');\n            \n            notification.success(\"[resource_displayname[deleted]]\");\n            grid.refresh();\n        });\n    },\n\n    deleteInstance: async function (e, grid) {\n        var row = $(e.target).closest('tr');\n        var item = grid.dataItem(row);\n        await item.destroy(e).then(() => {\n            notification.success(\"[resource_description[deleted]]\");\n            grid.refresh();\n        });\n    }\n}",
  "Content": "<div name=\"flowInstanceDetailsComponent\" style=\"display:none;\">\n    [component[FlowInstanceDetails]]\n</div>",
  "LastUpdated": "2024-11-19T18:18:31.2280548+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "WorkflowManagement",
  "Key": "Workflow",
  "ResourceKey": "Workflow",
  "Script": "WorkflowManagement = {\n\tinit: async function (app, container, process) {\n\t\tapp = app || session.app;\n\t\tcontainer = container || $('.component[name=WorkflowManagement]');\n\n\t\tapi.addToMetaCache([\n\t\t\t{\n\t\t\t\t\"Name\": \"Workflow\",\n\t\t\t\t\"Types\": [\n\t\t\t\t\t[meta[Workflow/FlowDefinition]]\n\t\t\t\t]\n\t\t\t}\n\t\t]);\n\t\tvar config = {\n\t\t\tendpoint: \"Workflow/FlowDefinition\",\n\t\t\todataAppend: \"?$filter=\" + (process ? \"AppId eq \" + app.Id + \" and ProcessId eq \" + process.Id : \"AppId eq \" + app.Id)\n\t\t};\n\t\tvar flowDefinitions = await model.getDatasource(config);\n\t\tawait WorkflowManagement.setupGrid(app, container, flowDefinitions, process);\n\t\tif (!window.knownTypes) { window.knownTypes = await api.get('GetMetadata'); }\n\t},\n\n\tsetupGrid: async function (app, container, flowDefinitions, process) {\n\t\tvar grid = new GridWidget(container, flowDefinitions);\n\t\tgrid.toolbar = \"<button class='btn btn-sm btn-primary float-end' name='add'><span class='k-icon k-i-plus'></span>[resource_displayname[new]]</button>\";\n\t\tgrid.groupable = false;\n\t\tgrid.scrollable = false;\n\t\tgrid.pageable = false;\n\t\tgrid.columns = [\n\t\t\t{ field: 'Name', title: \"[resource_displayname[name]]\" },\n\t\t\t{ field: 'Description', title: \"[resource_displayname[description]]\" },\n\t\t\t{ field: 'CreatedBy', title: \"[resource_displayname[createdby]]\" },\n\t\t\t{ field: 'CreatedOn', format: \"{0:\" + type.dateFormat + \"}\", title: \"[resource_displayname[created]]\" },\n\t\t\t{ field: 'LastUpdatedBy', title: \"[resource_displayname[lastupdatedby]]\" },\n\t\t\t{ field: 'LastUpdated', format: \"{0:\" + type.dateFormat + \"}\", title: \"[resource_displayname[lastupdated]]\" },\n\t\t\t{ field: 'InstanceReportingComponentName', title: \"[resource_displayname[instancereportingcomponentname]]\" },\n\t\t\t{ field: 'Id', template: \"<a href='/Admin/WorkflowDesigner?id=#:Id#'>[resource_displayname[edit]]</a>\", name: \"edit\", icon: \"k-i-edit\", title: \" \" }\n\t\t];\n\t\tgrid.commands.push({ name: \"save\", icon: \"k-i-save\", text: \"[resource_displayname[save]]\" });\n\t\tgrid.commands.push({ name: \"run\", icon: \"k-i-play\", text: \"[resource_displayname[run]]\" });\n\t\tgrid.commands.push({ name: \"delete\", icon: \"k-i-trash\", text: \"[resource_displayname[delete]]\" });\n\t\tgrid.detailTemplate = \"\";\n\t\tgrid.dataBound = function () {\n\t\t\t$(grid.gridElement)\n\t\t\t\t.off(\"click.workflowManagement\", \"[name=delete]\")\n\t\t\t\t.on(\"click.workflowManagement\", \"[name=delete]\", (e) => WorkflowManagement.deleteFlow(e, grid))\n\t\t\t\t.off(\"click.workflowManagement\", \"[name=run]\")\n\t\t\t\t.on(\"click.workflowManagement\", \"[name=run]\", (e) => WorkflowManagement.runFlow(e, grid))\n\t\t\t\t.off(\"click.workflowManagement\", \"[name=save]\")\n\t\t\t\t.on(\"click.workflowManagement\", \"[name=save]\", (e) => WorkflowManagement.save(e, grid));\n\t\t};\n\t\tgrid.detailExpand = async function (e) {\n\t\t\tvar definition = grid.dataItem(e.masterRow);\n\t\t\tvar detailElement = $('.k-detail-cell', e.detailRow);\n\n\t\t\tif (detailElement[0].childElementCount === 0) {\n\t\t\t\tvar template = $('script[name=FlowDetails]').html();\n\t\t\t\tvar id = Guid();\n\t\t\t\ttemplate = template.replace(/{ID}/g, id);\n\t\t\t\tdetailElement.append(template);\n\t\t\t}\n\n\t\t\tif (definition.ReportingComponentName) {\n\t\t\t\t$('[name=tabs] > ul', container).append('<li>[resource_displayname[report]]</li>');\n\t\t\t\t$('[name=tabs]', container).append(\"<div class='tab' name='report'></div>\");\n\t\t\t}\n\t\t\tif (definition.ReportingComponentName) {\n\t\t\t\tvar reportTab = $('.tab[name=report]', container);\n\t\t\t\tvar reportComponent = await loadComponent(reportTab, definition.ReportingComponentName);\n\t\t\t\tawait reportComponent.init(app, $('.component[name=' + definition.ReportingComponentName + ']'), definition);\n\t\t\t}\n\t\t\tif ($('.tab[name=executions]', e.detailRow)[0].childElementCount === 0) {\n\t\t\t\t$('.tab[name=executions]', e.detailRow).html($('[name=flowInstanceManagementComponent]').first().html());\n\t\t\t\tFlowInstanceManagement.init(app, $('.component[name=FlowInstanceManagement]', $('[name=executions]', e.detailRow)), definition.Id, definition.InstanceReportingComponentName);\n\t\t\t}\n\t\t};\n\t\tawait grid.init();\n\t\t$('[name=add]', grid.gridElement).off('click').on('click', async (e) => await WorkflowManagement.addFlow(e, process, grid, app));\n\t},\n\n\tgetGridItemFromEvent: function (e, grid) {\n\t\tvar row = $(e.currentTarget).closest('tr[data-uid]');\n\t\treturn row.length ? grid.dataItem(row) : null;\n\t},\n\n\taddFlow: async function (e, process, grid, app) {\n\t\te.preventDefault();\n\t\tvar roles = await api.get('AppSecurity/Role?$filter=AppId eq ' + app.Id);\n\t\tvar d = new Dialog({ title: '[resource_displayname[NewFlow]]' });\n\t\td.template = $('[name=newFlow]').html();\n\t\td.events.add = async function () {\n\t\t\tvar flow = {\n\t\t\t\t'Name': $('input[name=Name]', d.element).val(),\n\t\t\t\t'AppId': app.Id,\n\t\t\t\t'Description': '[resource_displayname[NewFlow]]',\n\t\t\t\t'ReportingComponentName': null,\n\t\t\t\t'InstanceReportingComponentName': null,\n\t\t\t\t'DefinitionJson': JSON.stringify({\n\t\t\t\t\t'Name': $('input[name=Name]', d.element).val(),\n\t\t\t\t\t'RequiredRoles': $('input[name=RequireRoles]:checked', d.element)\n\t\t\t\t\t\t.map((i, o) => o.value)\n\t\t\t\t\t\t.toArray()\n\t\t\t\t\t\t.join(','),\n\t\t\t\t\t'Links': [],\n\t\t\t\t\t'Activities': [{\n\t\t\t\t\t\t'$type': 'Core.Objects.Workflow.Activities.Start, Core.Objects',\n\t\t\t\t\t\t'AuthToken': null,\n\t\t\t\t\t\t'Data': null,\n\t\t\t\t\t\t'Ref': 'Start',\n\t\t\t\t\t\t'State': 0\n\t\t\t\t\t}]\n\t\t\t\t})\n\t\t\t};\n\t\t\tif (process) {\n\t\t\t\tflow.ProcessId = process.Id;\n\t\t\t}\n\n\t\t\tawait api.add('Workflow/FlowDefinition', flow).then(() => {\n\t\t\t\td.events.close();\n\t\t\t\tgrid.refresh();\n\t\t\t}).catch((err) => error(err));\n\t\t};\n\t\td.init(() => {\n\t\t\tvar roleSet = $('[name=roles]', d.element);\n\t\t\troleSet.html('');\n\t\t\tfor (var i in roles.value) {\n\t\t\t\tvar role = roles.value[i];\n\t\t\t\troleSet.append(`\n\t<li class=\"list-group-item p-1\">\n\t\t<div class=\"form-check\">\n\t\t\t<input class=\"form-check-input\" type=\"checkbox\" name=\"RequireRoles\" value=\"${role.Name}\" id=\"role-${role.Name}\">\n\t\t\t<label class=\"form-check-label\" for=\"role-${role.Name}\">\n\t\t\t\t${role.Name}\n\t\t\t</label>\n\t\t</div>\n\t</li>\n`);\n\t\t\t}\n\t\t});\n\t},\n\n\trunFlow: function (e, grid) {\n\t\te.preventDefault();\n\t\tvar item = WorkflowManagement.getGridItemFromEvent(e, grid);\n\t\tif (!item) {\n\t\t\tnotification.error('Unable to resolve the selected business process.');\n\t\t\treturn;\n\t\t}\n\t\tdelete item.type;\n\t\tvar flow = new Flow(item);\n\t\tflow.run(false);\n\t},\n\n\tdeleteFlow: function (e, grid) {\n\t\te.preventDefault();\n\t\tvar item = WorkflowManagement.getGridItemFromEvent(e, grid);\n\t\tif (!item) {\n\t\t\tnotification.error('Unable to resolve the selected business process.');\n\t\t\treturn;\n\t\t}\n\n\t\tvar d = new ConfirmDialog({\n\t\t\tquestion: '[resource_description[thiscannotbeundone]]',\n\t\t\tconfirm: '[resource_displayname[confirm]]',\n\t\t\tclose: '[resource_displayname[close]]',\n\t\t\twidth: 400,\n\t\t\theight: 200,\n\t\t\ttitle: '[resource_description[areyousure]]'\n\t\t});\n\t\td.events.confirm = async function (e) {\n\t\t\te.preventDefault();\n\t\t\tawait api.destroy('Workflow/FlowDefinition(' + item.Id + ')').then(() => {\n\t\t\t\tnotification.success('[resource_description[deleted]]');\n\t\t\t\tgrid.refresh();\n\t\t\t\td.events.close();\n\t\t\t}).catch((err) => error(err));\n\t\t};\n\t\td.init();\n\t},\n\n\tsave: function (e, grid) {\n\t\te.preventDefault();\n\t\tvar flow = WorkflowManagement.getGridItemFromEvent(e, grid);\n\t\tif (!flow) {\n\t\t\tnotification.error('Unable to resolve the selected business process.');\n\t\t\treturn;\n\t\t}\n\t\tflow.save(e);\n\t\tnotification.success('[resource_displayname[saved]]');\n\t}\n}",
  "Content": "<script name=\"FlowDetails\" type=\"text/template\">\n    <div name=\"tabs\">\n        <ul>\n            <li class=\"k-active\">[resource_displayname[Instances]]</li>\n        </ul>\n        <div class=\"tab\" name=\"executions\"></div>\n   </div>\n</script>\n<script name=\"newFlow\" type=\"text/template\">\n\t<ul class=\"fieldList\">\n\t\t<li>\n\t\t\t<label>[resource_displayname[name]]</label>\n\t\t\t<div class=\"value\">\n\t\t\t\t<input name=\"Name\" />\n\t\t\t</div>\n\t\t</li>\n        <li>\n\t\t\t<label>[resource_displayname[executableby]]</label>\n\t\t\t<div class=\"value\">\n\t\t\t\t<ul name=\"roles\"></ul>\n\t\t\t</div>\n\t\t</li>\n\t</ul>\n\t<hr />\n    <button class=\"btn btn-sm btn-primary float-end\" name=\"add\">\n        <span class=\"k-icon k-i-plus\"></span>[resource_displayname[add]]\n    </button>\n</script>\n\n<div name=\"flowInstanceManagementComponent\" style=\"display:none;\">[component[FlowInstanceManagement]]</div>",
  "LastUpdated": "2026-05-05T17:42:00.4675233+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "MasterdataImportResults",
  "Key": "Workflow",
  "ResourceKey": "Workflow",
  "Script": "MasterdataImportResults = {\r\n    init: async function(app, container, instance) {\r\n        // extract the packaged data\r\n        var dataPath = \"DMS/Data/Masterdata/Processed/\" + instance.Start.split(\"T\")[0] + \"/\";\r\n        var app = await api.get(\"ContentManagement/App(\" + app.Id + \")\");\r\n        app.Config = JSON.parse(app.ConfigJson);\r\n        await api.get(dataPath +  \"Report-\" + instance.Id + \".json\").then((report) => {\r\n            MasterdataImportResults.setupFileList(dataPath, report, $(\".container[name=files]\", container));\r\n            MasterdataImportResults.setupImportSummary(app, report.Buyers, $(\".container[name=buyers]\", container));\r\n            MasterdataImportResults.setupImportSummary(app, report.Suppliers, $(\".container[name=suppliers]\", container));\r\n            MasterdataImportResults.setupImportSummary(app, report.Funders, $(\".container[name=funders]\", container));\r\n            MasterdataImportResults.setupImportSummary(app, report.BuyerBuckets, $(\".container[name=buyerBuckets]\", container));\r\n            MasterdataImportResults.setupImportSummary(app, report.SupplierBuckets, $(\".container[name=supplierBuckets]\", container));\r\n            MasterdataImportResults.setupImportSummary(app, report.FunderBuckets, $(\".container[name=funderBuckets]\", container));\r\n        }).catch((err) => {\r\n            if(err.message) { notification.error(err.message); }\r\n            else { notification.error(err.responseText); }\r\n        });\r\n        $(\"[name=tabs]\", container).kendoTabStrip({ animation: { open: { effects: \"fadeIn\" } } });\r\n    },\r\n\r\n    setupFileList: function(dataPath, report, container) {\r\n        var files = report.FilesProccessed.map(function(f) { return { name: f, link: api.apiRoot + dataPath + f + \"?t=\" + session.token }; });\r\n        var links = files.map(f => \"<a download='\" + f.name + \"' href='\" + f.link + \"'>\" + f.name + \"</a>\");\r\n        $(\"[name=list]\", container).append(links.join(\"<br>\"));\r\n    },\r\n\r\n    setupImportSummary: function(app, data, container) {\r\n        container.append($(\"template[name=importSetSummary]\").first().html());\r\n        $(\"[name=imported]\", container).append(data.Imported);\r\n        $(\"[name=updated]\", container).append(data.Updated);\r\n        $(\"[name=deleted]\", container).append(data.Deleted);\r\n        $(\"[name=failures] > a\", container).append(data.FailuresTotal);\r\n        $(\"[name=failures] > a\", container).on(\"click\", () => MasterdataImportResults.openFailureDialog(app, data.Failures));\r\n        $(\"[name=total]\", container).append(data.Total);\r\n    },\r\n\r\n    openFailureDialog: function(app, failures) {\r\n        // transform the source data to extract the key details\r\n        var itemData = failures.map(function(i) { \r\n            return  {\r\n                clientRef: MasterdataImportResults.getClientRef(i.Item, app.Config.B2B.SourceSystem),\r\n                number: i.Item.Number,\r\n                message: i.Message\r\n            };\r\n        });\r\n\r\n        // build the dialog\r\n        var dialog = new Dialog({ \r\n            name: \"failedItemList\", \r\n            title: \"Failures\", \r\n            width: \"1000px\", \r\n            height: \"600px\", \r\n            template: \"<div name='grid' style='height: 600px;'></div>\" \r\n        });\r\n\r\n        dialog.init(function() {\r\n            // build the grid\r\n            var grid = new GridWidget($(\"[name=grid]\", dialog.element), itemData);\r\n            grid.editable = false;\r\n            grid.groupable = false;\r\n            grid.columns = [\r\n                { field: \"clientRef\", type: \"string\", title:\"Client Reference\", width: 120 },\r\n                { field: \"number\", type: \"string\", title:\"Number\", width: 120 },\r\n                { field: \"message\", type: \"string\", title: \"Problem\" }\r\n            ];\r\n            grid.init();\r\n        });\r\n    },\r\n\r\n    getClientRef: function(item, source) {\r\n        return item;\r\n    }\r\n}",
  "Content": "<div name=\"tabs\">\r\n   <ul>\r\n         <li class=\"k-state-active\">[resource_displayname[files]]</li>\r\n         <li>[resource_displayname[companies]]</li>\r\n         <li>[resource_displayname[buckets]]</li>\r\n   </ul>\r\n   \r\n   <div class=\"tab\" name=\"files\">\r\n     <div class=\"value\" name=\"list\"></div>\r\n   </div>\r\n   \r\n   <div class=\"tab\" name=\"companies\">\r\n     <div class=\"container\" name=\"buyers\">\r\n         <h4>[resource_displayname[buyers]]</h4>\r\n     </div>\r\n     <div class=\"container\" name=\"suppliers\">\r\n         <h4>[resource_displayname[suppliers]]</h4>\r\n     </div>\r\n     <div class=\"container\" name=\"funders\">\r\n         <h4>[resource_displayname[funders]]</h4>\r\n     </div>\r\n   </div>\r\n   \r\n   <div class=\"tab\" name=\"buckets\" >\r\n     <div class=\"container\" name=\"buyerBuckets\">\r\n         <h4>[resource_displayname[buyerbuckets]]</h4>\r\n     </div>\r\n     <div class=\"container\" name=\"supplierBuckets\">\r\n         <h4>[resource_displayname[supplierbuckets]]</h4>\r\n     </div>\r\n     <div class=\"container\" name=\"funderBuckets\">\r\n         <h4>[resource_displayname[funderbuckets]]</h4>\r\n     </div>\r\n   </div>\r\n</div>\r\n\r\n<template name=\"importSetSummary\" type=\"text/template\">\r\n    <ul class=\"fieldList\">\r\n        <li>\r\n            <label>[resource_displayname[imported]]</label>\r\n            <div class=\"value\" name=\"imported\"></div>\r\n        </li>\r\n        <li>\r\n            <label>[resource_displayname[updated]]</label>\r\n            <div class=\"value\" name=\"updated\"></div>\r\n        </li>\r\n        <li>\r\n            <label>[resource_displayname[deleted]]</label>\r\n            <div class=\"value\" name=\"deleted\"></div>\r\n        </li>\r\n        <li>\r\n            <label>[resource_displayname[failures]]</label>\r\n            <div class=\"value\" name=\"failures\">\r\n                <a href=\"#\"></a>\r\n            </div>\r\n        </li>\r\n        <li>\r\n            <label>[resource_displayname[total]]</label>\r\n            <div class=\"value\" name=\"total\"></div>\r\n        </li>\r\n    </ul>\r\n</template>\r\n\r\n<style scoped>\r\n    .subHeader { margin: 5px; }\r\n    .tab { flex-direction: row; }\r\n    .container { padding: 2px; max-width: 250px; max-height: 200px; }\r\n    .component[name=MasterdataImportResults] .containerOfContainers { height: 240px; width: 760px; margin-left: 0; display: inline-block; vertical-align: top; }\r\n    .component[name=MasterdataImportResults] .fieldList > li > label { width: 100px; }\r\n    .component[name=MasterdataImportResults] .fieldList > li > .value { width: 100px; }\r\n    .component[name=MasterdataImportResults] .container[name=files] > .fieldList > li > .value { width: 140px; }\r\n    .component[name=MasterdataImportResults] .container[name=files] { height: 200px; width: 400px; margin-left: 0; display: inline-block; vertical-align: top; }\r\n    .component[name=MasterdataImportResults] .value[name=\"list\"] { max-width: 400px; overflow-y: auto; height: 194px; border: solid 1px #ccc; padding: 2px; padding-left:0px; }\r\n    .component[name=MasterdataImportResults] .value[name=\"failures\"] > a { width: 80px; }\r\n    .component[name=MasterdataImportResults] .value[name=\"failures\"] > a > span { vertical-align: right; text-align: right; }\r\n</style>",
  "LastUpdated": "2026-04-20T10:20:09.0578764+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "TransactionImportResults",
  "Key": "Workflow",
  "ResourceKey": "Workflow",
  "Script": "TransactionImportResults = {\n    init: async function(app, container, instance) {\n        // extract the packaged data\n        var dataPath = \"DMS/Data/Transactions/Processed/\" + instance.Start.split(\"T\")[0] + \"/\";\n        var app = await api.get(\"ContentManagement/App(\" + app.Id + \")\");\n        app.Config = JSON.parse(app.ConfigJson);\n        await api.get(dataPath +  \"Report-\" + instance.Id + \".json\").then((report) => {\n            TransactionImportResults.setupFileList(dataPath, report, $(\".container[name=files]\", container));\n            TransactionImportResults.setupSummary(report, $(\".container[name=summary]\", container));\n            TransactionImportResults.setupTSet(app, report.Invoices, $(\".container[name=invoices]\", container));\n            TransactionImportResults.setupTSet(app, report.Credits, $(\".container[name=credits]\", container));\n            TransactionImportResults.setupTSet(app, report.Payments, $(\".container[name=payments]\", container));\n        }).catch((err) => {\n            notification.error(err.responseText);\n        });\n    },\n\n    setupFileList: function(dataPath, report, container) {\n        var files = report.FilesProccessed.map(function(f) { return { name: f, link: api.apiRoot + dataPath + f + \"?t=\" + session.token }; });\n        var links = files.map(f => \"<a download='\" + f.name + \"' href='\" + f.link + \"'>\" + f.name + \"</a>\");\n        $(\"[name=list]\", container).append(links.join(\"<br>\"));\n    },\n\n    setupSummary: function(report, container) {\n        $(\"[name=added]\", container).append(report.Invoices.Imported + report.Credits.Imported + report.Payments.Imported);\n        $(\"[name=updated]\", container).append(report.Invoices.Updated + report.Credits.Updated + report.Payments.Updated);\n        $(\"[name=failures]\", container).append(report.Invoices.Failures.length + report.Credits.Failures.length + report.Payments.Failures.length);\n        $(\"[name=total]\", container).append(report.Invoices.Total + report.Credits.Total + report.Payments.Total);\n    },\n\n    setupTSet: function(app, data, container) {\n        container.append($(\"template[name=transactionStats]\").html());\n        $(\"[name=added]\", container).append(data.Imported);\n        $(\"[name=updated]\", container).append(data.Updated);\n        $(\"[name=failures] > a\", container).append(data.Failures.length);\n        $(\"[name=failures] > a\", container).on(\"click\", function(e) { TransactionImportResults.openFailureDialog(app, data.Failures); });\n        $(\"[name=total]\", container).append(data.Total);\n    },\n\n    openFailureDialog: function(app, failures) {\n        // transform the source data to extract the key details\n        var itemData = failures.map(function(i) { \n            return  {\n                clientRef: TransactionImportResults.getClientRef(i.Item, app.Config.B2B.SourceSystem),\n                number: i.Item.Number,\n                message: i.Message\n            };\n        });\n\n        // build the dialog\n        var dialog = new Dialog({ \n            name: \"failedItemList\", \n            title: \"Failures\", \n            width: \"1000px\", \n            height: \"600px\",\n        });\n\n        dialog.init(() => {\n            // build the grid\n            var grid = new GridWidget(dialog.element, {data: itemData, pageSize: 20 });\n            grid.editable = false;\n            grid.groupable = false;\n            grid.columns = [\n                { field: \"clientRef\", type: \"string\", title:\"Client Reference\", width: 120 },\n                { field: \"number\", type: \"string\", title:\"Number\", width: 120 },\n                { field: \"message\", type: \"string\", title: \"Problem\" }\n            ];\n            grid.init();\n        });\n    },\n\n    getClientRef: function(item, source) {\n        if(item && item.References) {\n            var clientRef = item.References.filter(r => r.SystemId == source)[0];\n            if(clientRef) { return clientRef.Value; } \n            else { return \"\"; }\n        } else {\n            return \"\";\n        }\n        \n    }\n}",
  "Content": "<div class=\"container\" name=\"summary\">\n\t<h4>Summary</h4>\n\t<ul class=\"fieldList\">\n\t\t<li><label>Total New</label>\n\t\t\t<div class=\"value\" name=\"added\"></div>\n\t\t</li>\n\t\t<li><label>Total Updated</label>\n\t\t\t<div class=\"value\" name=\"updated\"></div>\n\t\t</li>\n\t\t<li><label>Total Failures</label>\n\t\t\t<div class=\"value\" name=\"failures\"></div>\n\t\t</li>\n\t\t<li><label>Total</label>\n\t\t\t<div class=\"value\" name=\"total\"></div>\n\t\t</li>\n\t</ul>\n</div>\n<div class=\"container\" name=\"invoices\">\n\t<h4>Invoices</h4>\n\t<ul class=\"fieldList\">\n\t\t<li><label>Total New</label>\n\t\t\t<div class=\"value\" name=\"added\"></div>\n\t\t</li>\n\t\t<li><label>Total Updated</label>\n\t\t\t<div class=\"value\" name=\"updated\"></div>\n\t\t</li>\n\t\t<li><label>Total Failures</label>\n\t\t\t<div class=\"value\" name=\"failures\"></div>\n\t\t</li>\n\t\t<li><label>Total</label>\n\t\t\t<div class=\"value\" name=\"total\"></div>\n\t\t</li>\n\t</ul>\n</div>\n<div class=\"container\" name=\"credits\">\n\t<h4>Credits</h4>\n\t<ul class=\"fieldList\">\n\t\t<li><label>Total New</label>\n\t\t\t<div class=\"value\" name=\"added\"></div>\n\t\t</li>\n\t\t<li><label>Total Updated</label>\n\t\t\t<div class=\"value\" name=\"updated\"></div>\n\t\t</li>\n\t\t<li><label>Total Failures</label>\n\t\t\t<div class=\"value\" name=\"failures\"></div>\n\t\t</li>\n\t\t<li><label>Total</label>\n\t\t\t<div class=\"value\" name=\"total\"></div>\n\t\t</li>\n\t</ul>\n</div>\n<div class=\"container\" name=\"payments\">\n\t<h4>Payments</h4>\n\t<ul class=\"fieldList\">\n\t\t<li><label>Total New</label>\n\t\t\t<div class=\"value\" name=\"added\"></div>\n\t\t</li>\n\t\t<li><label>Total Updated</label>\n\t\t\t<div class=\"value\" name=\"updated\"></div>\n\t\t</li>\n\t\t<li><label>Total Failures</label>\n\t\t\t<div class=\"value\" name=\"failures\"></div>\n\t\t</li>\n\t\t<li><label>Total</label>\n\t\t\t<div class=\"value\" name=\"total\"></div>\n\t\t</li>\n\t</ul>\n</div>\n<div class=\"container\" name=\"files\">\n\t<h4>Files</h4>\n\t<div class=\"value\" name=\"list\"></div>\n</div>\n\n<template name=\"transactionStats\" type=\"text/template\">\n\t<ul class=\"fieldList\">\n\t\t<li><label>New</label>\n\t\t\t<div class=\"value\" name=\"added\"></div>\n\t\t</li>\n\t\t<li><label>Updated</label>\n\t\t\t<div class=\"value\" name=\"updated\"></div>\n\t\t</li>\n\t\t<li><label>Failures</label>\n\t\t\t<div class=\"value\" name=\"failures\"><a href=\"#\"></a></div>\n\t\t</li>\n\t\t<li><label>Total</label>\n\t\t\t<div class=\"value\" name=\"total\"></div>\n\t\t</li>\n\t</ul>\n</template>",
  "LastUpdated": "2026-04-20T10:20:09.0598806+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "FlowEditor",
  "Key": "Workflow",
  "ResourceKey": "Workflow",
  "Script": "var FlowEditor = {\n\tinit: async function(app, container) {\n\t\tcontainer.addClass(\"large\");\n\t\tlet id = getQueryParameter(\"id\");\n\t\twindow.knownTypes = await api.get('GetMetadata');\n\t\twindow.editor = new WorkflowDesigner($(\".component[name=FlowEditor]\"), await api.get(\"Workflow/FlowDefinition(\" + id + \")\"));\n\t}\n}",
  "Content": "<div name=\"splitter\">\n   <div class=\"flowmenu\">\n   \t  <div name=\"actions\" class=\"actions\">\n            <button class=\"btn btn-primary\" name=\"settings\"> <span class='k-icon k-i-gears'></span> [resource_displayname[settings]]</button>\n            <button class=\"btn btn-primary\" name=\"run\"> <span class='k-icon k-i-play'></span> [resource_displayname[run]]</button>\n            <button class=\"btn btn-primary\" name=\"save\"> <span class='k-icon k-i-save'></span> [resource_displayname[save]]</button>\n       </div>\n   </div>\n   <div class=\"workspace\"><canvas></canvas></div>\n</div>\n\n<script name=\"flowExecution\" type=\"template\">\n<div class=\"execution\" style=\"overflow-x:hidden;\">\n    <ul class=\"fieldList options\">\n      <li>\n         <label>On Execution Complete</label>\n         <ul class=\"value\">\n         \t<li><input type=\"checkbox\" name=\"loadInDesigner\" checked>[resource_displayname[loadindesigner]]</input></li>\n        \t<li><input type=\"checkbox\" name=\"autoClose\" checked>[resource_displayname[autoclose]]</input></li>\n   \t\t </ul>\n      </li>\n   \t</ul>\n    <div class=\"executionConsole\"></div>\n</div>\n</script>\n\n<script name=\"executionLog\" type=\"template\">\n<div class='execConsole' name='execConsole'>\n   <style>\n      .execConsole { overflow: hidden; }\n      .flowConsole { padding: 5px; height: 300px; width: 790px; overflow: auto; }\n      .flowConsole > .message { }\n      .flowConsole > .message > * { vertical-align: top; }\n      .flowConsole > .message > .message { display: inline-block; border: none; max-width: 90%; word-wrap: break-word; }\n      .flowConsole > .message > .time { margin-right: 10px; }\n      .flowConsole > .message.Success > .message { color: green; }\n      .flowConsole > .message.Info > .message { color: green; }\n      .flowConsole > .message.Debug > .message { color: blue; }\n      .flowConsole > .message.Warning > .message { color: #D8A700; }\n      .flowConsole > .message.Error > .message { color: red; }\n      .flowConsole > .message.Fatal > .message { color: red; }\n   </style>\n   <div class='flowConsole'></div>\n</div>\n</script>\n\n<style scoped>\n   .execution > .options { margin-left: 20px; }\n   .execution > .options li { margin-bottom: 10px; }\n   .component[name=FlowEditor], \n    .component[name=FlowEditor]>[name=splitter] { margin: 0; margin-bottom: -10px; border: none; box-shadow: none; width: 100%;  height: 100%; }\n\t.component[name=FlowEditor]>[name=splitter]>.panel { height: 100%; }\n   \n   .flowmenu{ overflow: auto; background: #eae8e8; width: 320px !important;}\n   .flowmenu > .actions { margin: 10px; margin-bottom: 5px; padding-bottom: 10px; border-bottom: [theme[border.style]]; }\n   .flowmenu .btn { margin: 8px 2px; }\n   .categorytypes { padding: 8px; margin-bottom: 0; }\n   .categorytypes > li { background: [theme[colours.primary]]; color: #fff; cursor: pointer; }\n   .categorytypes .k-icon { color: #E2721D; margin-right: 10px;}\n   .categorytypes .subtypes { background: [theme[colours.primary]]; border: [theme[border.style]]; }\n   .categorytypes .subtypes > li            { margin: 5px; margin-left: -3px; color: #fff; transition: all 0.3s; cursor: grab; }\n   .categorytypes .subtypes > li .k-icon { color: [theme[colours.secondary]]; }\n   .categorytypes .subtypes > li:hover  { background: [theme[colours.secondary]]; }\n   [data-role=listview] > li                \t{ border-bottom: [theme[border.style]]; }\n   body > div.k-window.k-focus > div.k-window-content {overflow: hidden; }\n</style>",
  "LastUpdated": "2026-03-06T09:56:10.9115674Z"
}
"""
            },
        ]
    };

    static Package Pages => new()
    {
        Name = "Workflow Pages",
        Category = "Workflow",
        Description = "Workflow Pages.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/WorkflowDesigner",
  "Name": "Workflow Designer",
  "ResourceKey": "",
  "ShowOnMenus": false,
  "Order": 0,
  "LastUpdated": "2024-04-04T15:46:42.121866+01:00",
  "Layout": "Workflow",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[floweditor]]"
    },
    {
      "CultureId": "en-GB",
      "Name": "body",
      "Html": "[component[floweditor]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Workflow Designer",
      "Keywords": "Workflow Designer",
      "Title": "Workflow Designer"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/BusinessProcesses/Editor",
  "Name": "Editor",
  "ResourceKey": "",
  "ShowOnMenus": false,
  "Order": 0,
  "LastUpdated": "2024-04-04T16:34:04.820373+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[WorkflowManagement]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Business Process Editor",
      "Keywords": "Business Process Editor",
      "Title": "Editor"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/BusinessProcesses",
  "Name": "Business Processes",
  "ResourceKey": "",
  "ShowOnMenus": true,
  "Order": 3,
  "LastUpdated": "2024-06-24T10:38:55.7061513+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[WorkflowManagement]]"
    },
    {
      "CultureId": "fr-FR",
      "Name": "body",
      "Html": "[component[WorkflowManagement]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Manage the app's import processes, including their offer generation processes.",
      "Keywords": "Business Process Management",
      "Title": "Business Processes"
    }
  ]
}
"""
            },
        ]
    };

    static Package FlowDefinitions => new()
    {
        Name = "Workflow Flow Definitions",
        Category = "Workflow",
        Description = "Workflow Flow Definitions.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/FlowDefinition",
                Data = """
{
  "Name": "Starter Workflow",
  "Description": "Starter workflow",
  "DefinitionJson": "{\"Name\":\"\",\"RequiredRoles\":\"\",\"Links\":[{\"Source\":\"Start\",\"Destination\":\"7c87b5ea-8b9d-4887-9911-76df43d48f97\",\"Expression\":\"\"}],\"Activities\":[{\"$type\":\"cCoder.Core.Objects.Workflow.Activities.Start, cCoder.Core.Objects, Version=2024.6.14.2247, Culture=neutral, PublicKeyToken=null\",\"AuthToken\":null,\"Data\":null,\"Ref\":\"Start\",\"State\":null,\"AppId\":null,\"AuthType\":null,\"BaseUrl\":null,\"Previous\":null,\"Next\":null,\"ScriptRunner\":null},{\"$type\":\"cCoder.Core.Objects.Workflow.Activities.InfoActivity, cCoder.Core.Objects, Version=2024.6.14.2247, Culture=neutral, PublicKeyToken=null\",\"Ref\":\"7c87b5ea-8b9d-4887-9911-76df43d48f97\",\"Message\":\"I did a thing!\",\"State\":null,\"Previous\":null,\"Next\":null,\"ScriptRunner\":null}]}",
  "LastUpdated": "2024-07-02T16:12:12.196358+01:00"
}
"""
            },
        ]
    };

    static Package Calendars => new()
    {
        Name = "Workflow Calendars",
        Category = "Workflow",
        Description = "Workflow Calendars.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendar",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "TestAdminCalendarAdmin",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "AsAdmin_CalendarEventCRUD_ReturnsNoErrors",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "AsAdmin_CalendarEventCRUD_ReturnsNoErrors",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "AsAdmin_CalendarEventCRUD_ReturnsNoErrors",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "AsAdmin_CalendarEventCRUD_ReturnsNoErrors",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "AsAdmin_CalendarEventCRUD_ReturnsNoErrors",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "AsAdmin_CalendarEventCRUD_ReturnsNoErrors",
  "Description": "Test Admin Calendar"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Calendar",
                Data = """
{
  "Name": "AsAdmin_CalendarEventCRUD_ReturnsNoErrors",
  "Description": "Test Admin Calendar"
}
"""
            },
        ]
    };

    static Package PageRoles => new()
    {
        Name = "Workflow Page Roles",
        Category = "Workflow",
        Description = "Workflow Page Roles.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/WorkflowDesigner",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/BusinessProcesses/Editor",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/BusinessProcesses",
  "Role": "Administrators"
}
"""
            },
        ]
    };
}
