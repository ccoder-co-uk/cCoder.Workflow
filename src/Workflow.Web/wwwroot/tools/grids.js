window.WorkflowGrids = {
    apiRoot: "/Api/Core",
    initialized: false,
    context: {
        flowId: null
    },
    flowRows: [],

    workspaces: [
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
                "InstanceReportingComponentName",
                "DefinitionJson",
                "ConfigJson"
            ],
            selectable: true
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
            name: "FlowInstanceData",
            title: "Flow Instances",
            description: "Execution instance rows owned by the selected Flow Definition",
            key: "Id",
            keyType: "guid",
            context: { type: "flow", field: "FlowDefinitionId" },
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
                "End",
                "ContextString"
            ]
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
            button.innerHTML = `<span class="k-icon k-i-table"></span>${config.title}`;
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

        return `<label class="wf-context">` +
            `<span>Flow</span>` +
            `<select class="form-select form-select-sm flow-context" data-context-type="flow">` +
            `<option value="">Select Flow</option>` +
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
        $(`#${this.gridId(config)}`).kendoGrid({
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
        });
    },

    modelFields: function (config) {
        const fields = Object.assign({}, config.fields);

        fields[config.key] = Object.assign({}, fields[config.key], {
            editable: config.keyType !== "guid"
        });

        if (config.context) {
            fields[config.context.field] = Object.assign({}, fields[config.context.field], { editable: false });
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

        columns.push({
            command: [
                { name: "edit", text: "Edit" },
                { name: "destroy", text: "Delete" }
            ],
            title: "Actions",
            width: 180
        });

        return columns;
    },

    read: async function (config, options) {
        try {
            if (config.context && !this.contextValue(config.context.type)) {
                options.success([]);
                return;
            }

            const body = await WorkflowApi.get(this.readUrl(config));
            options.success(WorkflowApi.unwrapCollection(body));
        } catch (error) {
            options.error(error);
        }
    },

    readUrl: function (config) {
        let url = `${this.apiRoot}/${config.name}?$top=500`;

        if (config.context) {
            const filter = `${config.context.field} eq ${this.formatFilterValue(this.contextValue(config.context.type))}`;
            url += `&$filter=${encodeURIComponent(filter)}`;
        }

        return url;
    },

    create: async function (config, options) {
        try {
            if (config.context && !this.contextValue(config.context.type)) {
                throw new Error(`Select a ${config.context.type} before creating ${config.title}.`);
            }

            const payload = this.preparePayload(config, options.data, true);
            const result = await WorkflowApi.post(`${this.apiRoot}/${config.name}`, payload);
            options.success(result ?? payload);
            WorkflowApi.notify(`${config.title} created`);
        } catch (error) {
            options.error(error);
        }
    },

    update: async function (config, options) {
        try {
            const payload = this.preparePayload(config, options.data, false);
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

    preparePayload: function (config, data, isCreate) {
        const payload = {};

        Object.keys(config.fields).forEach(field => {
            const value = data[field];

            if (value !== undefined) {
                payload[field] = value;
            }
        });

        if (config.context) {
            payload[config.context.field] = this.contextValue(config.context.type);
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
    },

    onEdit: function (config, event) {
        if (config.context) {
            event.model.set(config.context.field, this.contextValue(config.context.type));
        }
    },

    onSelectionChanged: function (config) {
        if (config.name !== "FlowDefinition") {
            return;
        }

        const grid = $(`#${this.gridId(config)}`).data("kendoGrid");
        const row = grid.dataItem(grid.select());

        if (row) {
            this.setFlowContext(row.Id);
        }
    },

    onDataBound: function (config) {
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
        return contextType === "flow" ? this.context.flowId : null;
    },

    noRecordsMessage: function (config) {
        if (!config.context) {
            return `No ${config.title} found.`;
        }

        return `Select a Flow to manage ${config.title}.`;
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

    label: function (field) {
        return field.replace(/([a-z])([A-Z])/g, "$1 $2");
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
