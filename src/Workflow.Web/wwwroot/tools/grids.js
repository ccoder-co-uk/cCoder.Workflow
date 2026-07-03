window.WorkflowGrids = {
    apiRoot: "/Api/Core",
    initialized: false,
    context: {
        calendarId: null,
        flowId: null
    },
    calendarRows: [],
    flowRows: [],

    workspaces: [
        {
            name: "Calendar",
            title: "Calendars",
            description: "Calendars owned by workflow apps",
            key: "Id",
            keyType: "number",
            fields: {
                Id: { type: "number" },
                AppId: { type: "number" },
                Name: { type: "string" },
                Description: { type: "string" }
            },
            columns: ["Id", "AppId", "Name", "Description"],
            selectable: true,
            details: [
                {
                    name: "CalendarEvent",
                    title: "Calendar Events",
                    description: "Events that make up this calendar",
                    key: "Id",
                    keyType: "number",
                    parent: { field: "CalendarId", parentKey: "Id", label: "Calendar" },
                    fields: {
                        Id: { type: "number" },
                        CalendarId: { type: "number" },
                        Name: { type: "string" },
                        Description: { type: "string" },
                        Start: { type: "date" },
                        DurationInTicks: { type: "number" }
                    },
                    columns: ["Id", "CalendarId", "Name", "Description", "Start", "DurationInTicks"]
                }
            ]
        },
        {
            name: "ScheduledTask",
            title: "Scheduled Tasks",
            description: "Scheduled executions owned by the selected Flow Definition",
            key: "Id",
            keyType: "number",
            context: { type: "flow", field: "FlowId" },
            stamp: "scheduledTask",
            fields: {
                Id: { type: "number" },
                AppId: { type: "number" },
                FlowId: { type: "string" },
                Name: { type: "string" },
                Description: { type: "string" },
                ExecutionArgs: { type: "string" },
                ScheduleInTicks: { type: "number" },
                ExcludedEventsCalendarId: { type: "number" },
                ExcludedEventsName: { type: "string" },
                ExecuteAs: { type: "string" },
                CreatedBy: { type: "string" },
                UpdatedBy: { type: "string" },
                Created: { type: "date" },
                LastUpdated: { type: "date" },
                LastExecuted: { type: "date" },
                NextExecution: { type: "date" }
            },
            columns: [
                "Id",
                "AppId",
                "FlowId",
                "Name",
                "Description",
                "ExecutionArgs",
                "ScheduleInTicks",
                "ExcludedEventsCalendarId",
                "ExcludedEventsName",
                "ExecuteAs",
                "Created",
                "LastUpdated",
                "LastExecuted",
                "NextExecution"
            ]
        },
        {
            name: "FlowDefinition",
            title: "Flow Definitions",
            description: "Aggregate roots for workflow execution",
            key: "Id",
            keyType: "guid",
            stamp: "standard",
            fields: {
                Id: { type: "string" },
                AppId: { type: "number" },
                Name: { type: "string" },
                Description: { type: "string" },
                DefinitionJson: { type: "string" },
                ConfigJson: { type: "string" },
                ReportingComponentName: { type: "string" },
                InstanceReportingComponentName: { type: "string" }
            },
            columns: [
                "Id",
                "AppId",
                "Name",
                "Description",
                "ReportingComponentName",
                "InstanceReportingComponentName"
            ],
            selectable: true,
            details: [
                {
                    name: "FlowInstanceData",
                    title: "Flow Instances",
                    description: "Queued, running, and historical executions of this flow definition",
                    key: "Id",
                    keyType: "guid",
                    parent: { field: "FlowDefinitionId", parentKey: "Id", label: "Flow Definition" },
                    stamp: "instance",
                    fields: {
                        Id: { type: "string" },
                        FlowDefinitionId: { type: "string" },
                        Name: { type: "string" },
                        ContextString: { type: "string" },
                        State: { type: "string" },
                        ReportingComponentName: { type: "string" },
                        Caller: { type: "string" },
                        Start: { type: "date" },
                        End: { type: "date" }
                    },
                    columns: [
                        "Id",
                        "FlowDefinitionId",
                        "Name",
                        "State",
                        "ReportingComponentName",
                        "Caller",
                        "Start",
                        "End"
                    ],
                    details: [
                        {
                            type: "json",
                            title: "Context JSON",
                            description: "Execution context captured for this flow instance",
                            fields: ["ContextString"]
                        }
                    ]
                },
                {
                    type: "json",
                    title: "Definition JSON",
                    description: "Flow activity and link definition",
                    fields: ["DefinitionJson"]
                },
                {
                    type: "json",
                    title: "Config JSON",
                    description: "Flow execution configuration",
                    fields: ["ConfigJson"]
                }
            ]
        },
        {
            name: "WorkflowEvent",
            title: "Workflow Events",
            description: "Events owned by the selected Flow Definition",
            key: "Id",
            keyType: "guid",
            context: { type: "flow", field: "FlowId" },
            stamp: "event",
            fields: {
                Id: { type: "string" },
                FlowId: { type: "string" },
                Type: { type: "string" },
                EventContext: { type: "string" },
                ExecuteAs: { type: "string" },
                CreatedBy: { type: "string" },
                CreatedOn: { type: "date" }
            },
            columns: ["Id", "FlowId", "Type", "ExecuteAs", "CreatedBy", "CreatedOn", "EventContext"]
        },
        {
            name: "FlowOperations",
            title: "Flow Operations",
            description: "Execute flows and inspect workflow metadata",
            custom: true
        }
    ],

    init: function () {
        if (this.initialized || !WorkflowApi.isAuthenticated()) {
            return;
        }

        this.initialized = true;
        this.buildWorkspaces();
        this.workspaces
            .filter(config => !config.custom)
            .forEach(config => this.createGrid(config));
        this.bindOperations();
    },

    buildWorkspaces: function () {
        const nav = document.getElementById("workspace-nav");
        const surfaces = document.getElementById("workspace-surfaces");

        this.workspaces.forEach((config, index) => {
            const surfaceId = this.surfaceId(config);
            const button = document.createElement("button");
            button.className = `wf-nav-item${index === 0 ? " active" : ""}`;
            button.type = "button";
            button.dataset.workspaceTarget = surfaceId;
            button.textContent = config.title;
            button.addEventListener("click", () => this.showSurface(button));
            nav.appendChild(button);

            const section = document.createElement("section");
            section.id = surfaceId;
            section.className = `wf-surface${index === 0 ? " active" : ""}`;
            section.innerHTML = config.custom
                ? this.operationsHtml(config)
                : this.gridHtml(config);
            surfaces.appendChild(section);
        });

        document
            .querySelectorAll("[data-context-type='flow']")
            .forEach(select => select.addEventListener("change", event => this.setFlowContext(event.target.value)));

        document
            .querySelectorAll("[data-context-type='calendar']")
            .forEach(select => select.addEventListener("change", event => this.setCalendarContext(event.target.value)));
    },

    gridHtml: function (config) {
        return `<div class="wf-toolbar">` +
            `<div><h2>${config.title}</h2><span>${config.description}</span></div>` +
            this.contextHtml(config) +
            `</div>` +
            `<div id="${this.gridId(config)}" class="wf-grid"></div>`;
    },

    operationsHtml: function (config) {
        return `<div class="wf-toolbar">` +
            `<div><h2>${config.title}</h2><span>${config.description}</span></div>` +
            this.contextHtml({ context: { type: "flow" } }) +
            `</div>` +
            `<div class="wf-operations">` +
            `<section class="wf-operation">` +
            `<h3>Execute Flow</h3>` +
            `<textarea id="execute-payload" spellcheck="false">{}</textarea>` +
            `<button id="execute-flow" class="k-button k-button-md k-rounded-md k-button-solid k-button-solid-primary" type="button">` +
            `<span class="k-icon k-i-play"></span>Execute</button>` +
            `</section>` +
            `<section class="wf-operation">` +
            `<h3>Execute Script</h3>` +
            `<textarea id="script-payload" spellcheck="false">return \"OK\";</textarea>` +
            `<button id="execute-script" class="k-button k-button-md k-rounded-md k-button-solid k-button-solid-primary" type="button">` +
            `<span class="k-icon k-i-play"></span>Execute Script</button>` +
            `</section>` +
            `<section class="wf-operation">` +
            `<h3>Metadata</h3>` +
            `<div class="wf-button-row">` +
            `<button id="load-activity-types" class="k-button k-button-md k-rounded-md k-button-solid k-button-solid-base" type="button">Activity Types</button>` +
            `<button id="load-system-types" class="k-button k-button-md k-rounded-md k-button-solid k-button-solid-base" type="button">System Types</button>` +
            `</div>` +
            `</section>` +
            `<pre id="operation-result" class="wf-result" aria-live="polite"></pre>` +
            `</div>`;
    },

    contextHtml: function (config) {
        if (!config.context) {
            return "";
        }

        const contextType = config.context.type;
        const label = contextType === "calendar" ? "Calendar" : "Flow";

        return `<label class="wf-context">` +
            `<span>${label}</span>` +
            `<select class="form-select form-select-sm ${contextType}-context" data-context-type="${contextType}">` +
            `<option value="">Select ${label}</option>` +
            `</select>` +
            `</label>`;
    },

    showSurface: function (button) {
        const target = button.dataset.workspaceTarget;

        document
            .querySelectorAll("[data-workspace-target]")
            .forEach(item => item.classList.toggle("active", item === button));

        document
            .querySelectorAll(".wf-surface")
            .forEach(surface => surface.classList.toggle("active", surface.id === target));
    },

    createGrid: function (config) {
        const gridOptions = {
            dataSource: {
                transport: {
                    read: options => this.read(config, options),
                    create: options => this.create(config, options),
                    update: options => this.update(config, options),
                    destroy: options => this.destroy(config, options)
                },
                schema: {
                    model: {
                        id: config.key,
                        fields: this.modelFields(config)
                    }
                },
                pageSize: 20
            },
            toolbar: [{ name: "create", text: `Create ${config.title}` }],
            editable: {
                mode: "popup",
                confirmation: false,
                window: {
                    width: "760px"
                }
            },
            pageable: true,
            sortable: true,
            filterable: true,
            resizable: true,
            reorderable: true,
            scrollable: true,
            selectable: "row",
            columns: this.columns(config),
            noRecords: true,
            messages: {
                noRecords: this.noRecordsMessage(config)
            },
            change: () => this.onSelectionChanged(config),
            edit: event => this.onEdit(config, event),
            save: () => WorkflowApi.notify("Saving..."),
            remove: () => WorkflowApi.notify("Deleting..."),
            dataBound: () => this.onDataBound(config)
        };

        if (config.details?.length) {
            gridOptions.detailInit = event => this.onDetailInit(config, event);
        }

        $(`#${this.gridId(config)}`).kendoGrid(gridOptions);
    },

    onDetailInit: function (config, event) {
        const parentRow = event.data;
        const detailId = `${this.gridId(config)}-details-${this.keyFragment(parentRow[config.key])}`;
        const tabs = config.details.map((detail, index) =>
            `<button class="wf-detail-tab${index === 0 ? " active" : ""}" type="button" data-detail-target="${detailId}-${index}">` +
            `${detail.title}</button>`).join("");
        const surfaces = config.details.map((detail, index) =>
            `<section id="${detailId}-${index}" class="wf-detail-surface${index === 0 ? " active" : ""}">` +
            `<div class="wf-detail-heading"><strong>${detail.title}</strong><span>${detail.description}</span></div>` +
            this.detailContentHtml(config, detail, parentRow) +
            `</section>`).join("");

        event.detailCell.html(
            `<div class="wf-detail-panel">` +
            `<nav class="wf-detail-tabs" aria-label="${config.title} child tabs">${tabs}</nav>` +
            surfaces +
            `</div>`);

        event.detailCell
            .find("[data-detail-target]")
            .on("click", clickEvent => this.showDetailSurface(clickEvent.currentTarget));

        event.detailCell
            .find("[data-json-save]")
            .on("click", clickEvent => this.saveJsonDetail(config, parentRow, clickEvent.currentTarget));

        config.details
            .filter(detail => detail.type !== "json")
            .forEach(detail => this.createChildGrid(detail, parentRow[detail.parent.parentKey]));
    },

    detailContentHtml: function (config, detail, parentRow) {
        if (detail.type === "json") {
            return this.jsonDetailHtml(config, detail, parentRow);
        }

        return `<div id="${this.childGridId(detail, parentRow[detail.parent.parentKey])}" class="wf-grid wf-child-grid"></div>`;
    },

    jsonDetailHtml: function (config, detail, row) {
        const editors = detail.fields.map(field => {
            const value = this.formatJson(row[field]);

            return `<label class="wf-json-editor">` +
                `<span>${this.label(field)}</span>` +
                `<textarea spellcheck="false" data-json-field="${field}">${this.encode(value)}</textarea>` +
                `</label>`;
        }).join("");

        return `<div class="wf-json-panel" data-json-config="${config.name}">` +
            editors +
            `<div class="wf-button-row">` +
            `<button class="k-button k-button-sm k-rounded-md k-button-solid k-button-solid-primary" type="button" data-json-save="${config.name}">` +
            `<span class="k-icon k-i-save"></span>Save JSON</button>` +
            `</div>` +
            `</div>`;
    },

    showDetailSurface: function (button) {
        const panel = button.closest(".wf-detail-panel");
        const target = button.dataset.detailTarget;

        panel
            .querySelectorAll("[data-detail-target]")
            .forEach(item => item.classList.toggle("active", item === button));

        panel
            .querySelectorAll(".wf-detail-surface")
            .forEach(surface => surface.classList.toggle("active", surface.id === target));
    },

    createChildGrid: function (config, parentValue) {
        $(`#${this.childGridId(config, parentValue)}`).kendoGrid({
            dataSource: {
                transport: {
                    read: options => this.read(config, options, parentValue),
                    create: options => this.create(config, options, parentValue),
                    update: options => this.update(config, options, parentValue),
                    destroy: options => this.destroy(config, options)
                },
                schema: {
                    model: {
                        id: config.key,
                        fields: this.modelFields(config)
                    }
                },
                pageSize: 10
            },
            toolbar: [{ name: "create", text: `Create ${config.title}` }],
            editable: {
                mode: "popup",
                confirmation: false,
                window: {
                    width: "760px"
                }
            },
            pageable: true,
            sortable: true,
            filterable: true,
            resizable: true,
            reorderable: true,
            scrollable: true,
            columns: this.columns(config),
            noRecords: true,
            messages: {
                noRecords: this.noRecordsMessage(config)
            },
            detailInit: config.details?.length
                ? event => this.onDetailInit(config, event)
                : undefined,
            edit: event => this.onEdit(config, event, parentValue),
            save: () => WorkflowApi.notify("Saving..."),
            remove: () => WorkflowApi.notify("Deleting..."),
            dataBound: () => WorkflowApi.notify("Ready")
        });
    },

    saveJsonDetail: async function (config, row, button) {
        try {
            const panel = button.closest(".wf-json-panel");
            const rowData = row.toJSON ? row.toJSON() : row;
            const payload = {};

            Object.keys(config.fields).forEach(field => {
                payload[field] = rowData[field];
            });

            panel.querySelectorAll("[data-json-field]").forEach(textarea => {
                payload[textarea.dataset.jsonField] = textarea.value;
            });

            const result = await WorkflowApi.put(
                `${this.apiRoot}/${config.name}(${this.formatKey(row[config.key])})`,
                payload);

            Object.keys(payload).forEach(field => {
                if (row.set) {
                    row.set(field, payload[field]);
                } else {
                    row[field] = payload[field];
                }
            });

            WorkflowApi.notify(`${config.title} JSON updated`);
            return result;
        } catch (error) {
            WorkflowApi.notify(error.message || error, true);
            throw error;
        }
    },

    modelFields: function (config) {
        const fields = Object.assign({}, config.fields);

        fields[config.key] = Object.assign({}, fields[config.key], {
            editable: config.keyType !== "guid"
        });

        if (config.context) {
            fields[config.context.field] = Object.assign({}, fields[config.context.field], { editable: false });
        }

        if (config.parent) {
            fields[config.parent.field] = Object.assign({}, fields[config.parent.field], { editable: false });
        }

        return fields;
    },

    columns: function (config) {
        const columns = config.columns.map(field => ({
            field: field,
            title: this.label(field),
            width: this.widthFor(field),
            format: this.formatFor(config.fields[field])
        }));

        const commands = [];

        if (config.name === "ScheduledTask") {
            commands.push({
                name: "execute",
                text: "Execute",
                click: event => this.executeScheduledTask(event)
            });
        }

        commands.push(
            { name: "edit", text: "Edit" },
            { name: "destroy", text: "Delete" });

        columns.push({
            command: commands,
            title: "Actions",
            width: config.name === "ScheduledTask" ? 260 : 180
        });

        return columns;
    },

    read: async function (config, options, parentValue = null) {
        try {
            if (config.context && !this.contextValue(config.context.type)) {
                options.success([]);
                return;
            }

            if (config.parent && parentValue == null) {
                options.success([]);
                return;
            }

            const body = await WorkflowApi.get(this.readUrl(config, parentValue));
            options.success(WorkflowApi.unwrapCollection(body));
        } catch (error) {
            options.error(error);
        }
    },

    readUrl: function (config, parentValue = null) {
        let url = `${this.apiRoot}/${config.name}?$top=500`;

        if (config.context) {
            const filter = `${config.context.field} eq ${this.formatFilterValue(this.contextValue(config.context.type))}`;
            url += `&$filter=${encodeURIComponent(filter)}`;
        }

        if (config.parent) {
            const filter = `${config.parent.field} eq ${this.formatFilterValue(parentValue)}`;
            url += `&$filter=${encodeURIComponent(filter)}`;
        }

        return url;
    },

    create: async function (config, options, parentValue = null) {
        try {
            if (config.context && !this.contextValue(config.context.type)) {
                throw new Error(`Select a ${config.context.type} before creating ${config.title}.`);
            }

            if (config.parent && parentValue == null) {
                throw new Error(`Expand a ${config.parent.label} before creating ${config.title}.`);
            }

            const payload = this.preparePayload(config, options.data, true, parentValue);
            const result = await WorkflowApi.post(`${this.apiRoot}/${config.name}`, payload);
            options.success(result ?? payload);
            WorkflowApi.notify(`${config.title} created`);
        } catch (error) {
            options.error(error);
        }
    },

    update: async function (config, options, parentValue = null) {
        try {
            const payload = this.preparePayload(config, options.data, false, parentValue);
            const result = await WorkflowApi.put(
                `${this.apiRoot}/${config.name}(${this.formatKey(options.data[config.key])})`,
                payload);

            options.success(result ?? payload);
            WorkflowApi.notify(`${config.title} updated`);
        } catch (error) {
            options.error(error);
        }
    },

    destroy: async function (config, options) {
        try {
            await WorkflowApi.delete(
                `${this.apiRoot}/${config.name}(${this.formatKey(options.data[config.key])})`);

            options.success(options.data);
            WorkflowApi.notify(`${config.title} deleted`);
        } catch (error) {
            options.error(error);
        }
    },

    preparePayload: function (config, data, isCreate, parentValue = null) {
        const payload = {};

        Object.keys(config.fields).forEach(field => {
            const value = data[field];

            if (value !== undefined) {
                payload[field] = value;
            }
        });

        if (config.context) {
            payload[config.context.field] = this.contextPayloadValue(config);
        }

        if (config.parent) {
            payload[config.parent.field] = this.parentPayloadValue(config, parentValue);
        }

        if (config.keyType === "guid" && isCreate && !payload[config.key]) {
            payload[config.key] = crypto.randomUUID();
        }

        this.applyStamp(config, payload, isCreate);

        return payload;
    },

    applyStamp: function (config, payload, isCreate) {
        const now = new Date().toISOString();
        const userId = WorkflowApi.currentUserId();

        if (config.stamp === "standard") {
            if (isCreate) {
                payload.CreatedOn = now;
                payload.CreatedBy = userId;
            }

            payload.LastUpdated = now;
            payload.LastUpdatedBy = userId;
            payload.DefinitionJson = payload.DefinitionJson || JSON.stringify(this.defaultFlowDefinition(payload.Name), null, 2);
            payload.ConfigJson = payload.ConfigJson || "{}";
        }

        if (config.stamp === "event") {
            if (isCreate) {
                payload.CreatedOn = now;
                payload.CreatedBy = userId;
            }

            payload.ExecuteAs = payload.ExecuteAs || userId;
            payload.Type = payload.Type || "Manual";
            payload.EventContext = payload.EventContext || "{}";
        }

        if (config.stamp === "instance") {
            if (isCreate) {
                payload.Start = now;
            }

            payload.State = payload.State || "Queued";
            payload.Name = payload.Name || "Manual instance";
            payload.ContextString = payload.ContextString || "{}";
            payload.Caller = payload.Caller || userId;
        }

        if (config.stamp === "scheduledTask") {
            if (isCreate) {
                payload.Created = now;
                payload.CreatedBy = userId;
                payload.NextExecution = payload.NextExecution || now;
            }

            const flow = this.selectedFlow();
            payload.AppId = payload.AppId || flow?.AppId;
            payload.LastUpdated = now;
            payload.UpdatedBy = userId;
            payload.ExecuteAs = payload.ExecuteAs || userId;
            payload.ExecutionArgs = payload.ExecutionArgs || "{}";
            payload.ScheduleInTicks = payload.ScheduleInTicks || 864000000000;
        }
    },

    onEdit: function (config, event, parentValue = null) {
        if (config.context) {
            event.model.set(config.context.field, this.contextValue(config.context.type));
        }

        if (config.parent) {
            event.model.set(config.parent.field, this.parentPayloadValue(config, parentValue));
        }
    },

    onSelectionChanged: function (config) {
        if (config.name === "Calendar") {
            const grid = $(`#${this.gridId(config)}`).data("kendoGrid");
            const row = grid.dataItem(grid.select());

            if (row) {
                this.setCalendarContext(row.Id);
            }
        }

        if (config.name === "FlowDefinition") {
            const grid = $(`#${this.gridId(config)}`).data("kendoGrid");
            const row = grid.dataItem(grid.select());

            if (row) {
                this.setFlowContext(row.Id);
            }
        }
    },

    onDataBound: function (config) {
        if (config.name === "Calendar") {
            this.calendarRows = this.gridRows(config);
            this.refreshCalendarSelectors();

            if (!this.context.calendarId && this.calendarRows.length > 0) {
                this.setCalendarContext(this.calendarRows[0].Id);
            }
        }

        if (config.name === "FlowDefinition") {
            this.flowRows = this.gridRows(config);
            this.refreshFlowSelectors();

            if (!this.context.flowId && this.flowRows.length > 0) {
                this.setFlowContext(this.flowRows[0].Id);
            }
        }

        this.updateCreateButtons();
        WorkflowApi.notify("Ready");
    },

    gridRows: function (config) {
        const grid = $(`#${this.gridId(config)}`).data("kendoGrid");
        return grid?.dataSource?.data()?.toJSON?.() ?? [];
    },

    setCalendarContext: function (value, refresh = true) {
        const calendarId = value || null;

        if (this.context.calendarId === calendarId) {
            return;
        }

        this.context.calendarId = calendarId;
        this.refreshCalendarSelectors();

        if (refresh) {
            this.refreshContextGrids("calendar");
        }
    },

    setFlowContext: function (value, refresh = true) {
        const flowId = value || null;

        if (this.context.flowId === flowId) {
            return;
        }

        this.context.flowId = flowId;
        this.refreshFlowSelectors();

        if (refresh) {
            this.refreshContextGrids("flow");
        }
    },

    refreshContextGrids: function (contextType) {
        this.workspaces
            .filter(config => config.context?.type === contextType)
            .forEach(config => {
                const grid = $(`#${this.gridId(config)}`).data("kendoGrid");

                if (grid) {
                    grid.dataSource.read();
                }
            });
    },

    refreshCalendarSelectors: function () {
        document.querySelectorAll(".calendar-context").forEach(select => {
            const current = this.context.calendarId ?? "";
            select.innerHTML = `<option value="">Select Calendar</option>`;

            this.calendarRows.forEach(calendar => {
                const option = document.createElement("option");
                option.value = calendar.Id;
                option.textContent = `${calendar.Name ?? "Calendar"} - ${calendar.Id}`;
                select.appendChild(option);
            });

            select.value = current;
        });
    },

    refreshFlowSelectors: function () {
        document.querySelectorAll(".flow-context").forEach(select => {
            const current = this.context.flowId ?? "";
            select.innerHTML = `<option value="">Select Flow</option>`;

            this.flowRows.forEach(flow => {
                const option = document.createElement("option");
                option.value = flow.Id;
                option.textContent = `${flow.Name ?? "Flow"} - ${flow.Id}`;
                select.appendChild(option);
            });

            select.value = current;
        });
    },

    updateCreateButtons: function () {
        this.workspaces
            .filter(config => !config.custom)
            .forEach(config => {
                const grid = $(`#${this.gridId(config)}`).data("kendoGrid");
                const button = grid?.wrapper?.find(".k-grid-add");

                if (!button?.length || !config.context) {
                    return;
                }

                const enabled = Boolean(this.contextValue(config.context.type));
                button.toggleClass("k-disabled", !enabled);
                button.attr("aria-disabled", String(!enabled));
            });
    },

    bindOperations: function () {
        document.getElementById("execute-flow").addEventListener("click", () => this.executeFlow());
        document.getElementById("execute-script").addEventListener("click", () => this.executeScript());
        document.getElementById("load-activity-types").addEventListener("click", () => this.loadMetadata("KnownActivityTypes"));
        document.getElementById("load-system-types").addEventListener("click", () => this.loadMetadata("KnownSystemTypes"));
    },

    executeFlow: async function () {
        try {
            const flowId = this.context.flowId;

            if (!flowId) {
                throw new Error("Select a Flow before executing.");
            }

            const payload = document.getElementById("execute-payload").value || "{}";
            const result = await WorkflowApi.post(
                `${this.apiRoot}/FlowDefinition(${this.formatKey(flowId)})/Execute`,
                payload,
                "text/plain");

            this.showOperationResult(result);
            this.refreshContextGrids("flow");
            WorkflowApi.notify("Flow queued");
        } catch (error) {
            this.showOperationResult(error.message || error);
            WorkflowApi.notify(error.message || error, true);
        }
    },

    executeScheduledTask: async function (event) {
        event.preventDefault();

        try {
            const grid = $(event.currentTarget).closest(".k-grid").data("kendoGrid");
            const row = grid.dataItem($(event.currentTarget).closest("tr"));

            if (!row) {
                throw new Error("Select a scheduled task before executing.");
            }

            await WorkflowApi.post(`${this.apiRoot}/ScheduledTask(${row.Id})/Execute`);
            grid.dataSource.read();
            WorkflowApi.notify("Scheduled task executed");
        } catch (error) {
            WorkflowApi.notify(error.message || error, true);
        }
    },

    executeScript: async function () {
        try {
            const script = document.getElementById("script-payload").value || "";
            const result = await WorkflowApi.post(
                `${this.apiRoot}/FlowDefinition/ExecuteScript`,
                script,
                "text/plain");

            this.showOperationResult(result);
            WorkflowApi.notify("Script executed");
        } catch (error) {
            this.showOperationResult(error.message || error);
            WorkflowApi.notify(error.message || error, true);
        }
    },

    loadMetadata: async function (actionName) {
        try {
            const result = await WorkflowApi.get(`${this.apiRoot}/FlowDefinition/${actionName}()`);
            this.showOperationResult(result);
            WorkflowApi.notify(`${actionName} loaded`);
        } catch (error) {
            this.showOperationResult(error.message || error);
            WorkflowApi.notify(error.message || error, true);
        }
    },

    showOperationResult: function (result) {
        const target = document.getElementById("operation-result");
        target.textContent = typeof result === "string"
            ? result
            : JSON.stringify(result, null, 2);
    },

    contextValue: function (contextType) {
        if (contextType === "calendar") {
            return this.context.calendarId;
        }

        return contextType === "flow" ? this.context.flowId : null;
    },

    contextPayloadValue: function (config) {
        const value = this.contextValue(config.context.type);
        const field = config.fields[config.context.field];

        return field?.type === "number" && value !== null
            ? Number(value)
            : value;
    },

    parentPayloadValue: function (config, parentValue) {
        const field = config.fields[config.parent.field];

        return field?.type === "number" && parentValue != null
            ? Number(parentValue)
            : parentValue;
    },

    selectedFlow: function () {
        return this.flowRows.find(flow => flow.Id === this.context.flowId) ?? null;
    },

    noRecordsMessage: function (config) {
        if (config.parent) {
            return `No ${config.title} found for this ${config.parent.label}.`;
        }

        if (!config.context) {
            return `No ${config.title} found.`;
        }

        const label = config.context.type === "calendar" ? "Calendar" : "Flow";
        return `Select a ${label} to manage ${config.title}.`;
    },

    defaultFlowDefinition: function (name) {
        return {
            Name: name || "New Flow",
            RequiredRoles: null,
            Activities: [
                {
                    "$type": "cCoder.Workflow.Activities.Start, cCoder.Workflow.Activities",
                    Ref: "Start",
                    State: 0
                }
            ],
            Links: []
        };
    },

    formatFilterValue: function (value) {
        return value;
    },

    formatKey: function (value) {
        return value;
    },

    formatFor: function (field) {
        return field?.type === "date" ? "{0:yyyy-MM-dd HH:mm:ss}" : undefined;
    },

    surfaceId: function (config) {
        return `surface-${config.name.toLowerCase()}`;
    },

    gridId: function (config) {
        return `grid-${config.name.toLowerCase()}`;
    },

    childGridId: function (config, parentValue) {
        return `${this.gridId(config)}-${this.keyFragment(parentValue)}`;
    },

    keyFragment: function (value) {
        return String(value ?? "none").replace(/[^a-zA-Z0-9_-]/g, "-");
    },

    label: function (field) {
        return field.replace(/([a-z])([A-Z])/g, "$1 $2");
    },

    formatJson: function (value) {
        if (!value) {
            return "{}";
        }

        try {
            return JSON.stringify(JSON.parse(value), null, 2);
        } catch {
            return value;
        }
    },

    encode: function (value) {
        return String(value ?? "")
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll("\"", "&quot;")
            .replaceAll("'", "&#39;");
    },

    widthFor: function (field) {
        if (field === "Id") {
            return 280;
        }

        if (field.endsWith("Id")) {
            return 280;
        }

        if (["DefinitionJson", "ConfigJson", "EventContext", "ContextString"].includes(field)) {
            return 380;
        }

        return 190;
    }
};

document.addEventListener("workflow-auth-changed", event => {
    if (event.detail.isAuthenticated) {
        window.WorkflowGrids.init();
    }
});

document.addEventListener("DOMContentLoaded", () => window.WorkflowGrids.init());
