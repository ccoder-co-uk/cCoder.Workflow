window.flowManagement = (() => {
    const baseUrl = "/Api/Core/FlowDefinition";
    let selectedFlow = null;

    function elements() {
        return {
            list: document.getElementById("flow-list"),
            appId: document.getElementById("flow-app-id"),
            name: document.getElementById("flow-name"),
            description: document.getElementById("flow-description"),
            definition: document.getElementById("flow-definition"),
            payload: document.getElementById("execution-payload"),
            log: document.getElementById("operation-log"),
            save: document.getElementById("save-flow"),
            run: document.getElementById("run-flow"),
            delete: document.getElementById("delete-flow")
        };
    }

    async function load() {
        log("Loading flows...");
        const response = await workflowApi.get(`${baseUrl}?$top=50&$orderby=Name`);
        renderList(response.value || []);
        log(`Loaded ${(response.value || []).length} flow(s).`);
    }

    function renderList(flows) {
        const { list } = elements();
        list.replaceChildren();

        if (flows.length === 0) {
            const empty = document.createElement("p");
            empty.textContent = "No flows found.";
            list.appendChild(empty);
            return;
        }

        flows.forEach(flow => {
            const button = document.createElement("button");
            button.type = "button";
            button.textContent = flow.Name || flow.Id;
            button.dataset.id = flow.Id;
            button.className = selectedFlow && selectedFlow.Id === flow.Id ? "active" : "";
            button.addEventListener("click", () => select(flow));
            list.appendChild(button);
        });
    }

    function select(flow) {
        selectedFlow = flow;
        const target = elements();
        target.appId.value = flow.AppId || "";
        target.name.value = flow.Name || "";
        target.description.value = flow.Description || "";

        try {
            workflowDesigner.formatDefinition(JSON.parse(flow.DefinitionJson || "{}"));
        } catch {
            target.definition.value = flow.DefinitionJson || "{}";
            workflowDesigner.draw();
        }

        setButtons();
        document.querySelectorAll("#flow-list button").forEach(button => {
            button.classList.toggle("active", button.dataset.id === flow.Id);
        });
    }

    function createNew() {
        selectedFlow = null;
        const target = elements();
        target.name.value = "New Flow";
        target.description.value = "";
        workflowDesigner.formatDefinition(workflowDesigner.defaultDefinition("New Flow"));
        setButtons();
    }

    async function save() {
        const target = elements();
        const definition = workflowDesigner.parseDefinition();
        const model = {
            Id: selectedFlow?.Id,
            AppId: Number.parseInt(target.appId.value, 10),
            Name: target.name.value,
            Description: target.description.value,
            DefinitionJson: JSON.stringify(definition),
            ConfigJson: selectedFlow?.ConfigJson || "{}"
        };

        if (!model.AppId || !model.Name) {
            throw new Error("App Id and Name are required.");
        }

        selectedFlow = selectedFlow
            ? await workflowApi.put(`${baseUrl}(${selectedFlow.Id})`, model)
            : await workflowApi.post(baseUrl, model);

        log(`Saved ${selectedFlow.Name}.`);
        await load();
        select(selectedFlow);
    }

    async function remove() {
        if (!selectedFlow) {
            return;
        }

        await workflowApi.delete(`${baseUrl}(${selectedFlow.Id})`);
        log(`Deleted ${selectedFlow.Name}.`);
        selectedFlow = null;
        createNew();
        await load();
    }

    async function run() {
        if (!selectedFlow) {
            throw new Error("Save or select a flow before running it.");
        }

        const result = await workflowApi.postText(
            `${baseUrl}(${selectedFlow.Id})/Execute`,
            elements().payload.value || "{}");

        log(`Queued flow instance: ${result}`);
    }

    function setButtons() {
        const target = elements();
        target.run.disabled = !selectedFlow;
        target.delete.disabled = !selectedFlow;
    }

    function log(message) {
        const target = elements().log;
        target.textContent = `[${new Date().toLocaleTimeString()}] ${message}\n${target.textContent}`;
    }

    async function execute(action) {
        try {
            await action();
        } catch (error) {
            log(error.message || error);
        }
    }

    function init() {
        document.getElementById("refresh-flows").addEventListener("click", () => execute(load));
        document.getElementById("new-flow").addEventListener("click", createNew);
        document.getElementById("save-flow").addEventListener("click", () => execute(save));
        document.getElementById("run-flow").addEventListener("click", () => execute(run));
        document.getElementById("delete-flow").addEventListener("click", () => execute(remove));
        document.getElementById("flow-definition").addEventListener("input", workflowDesigner.draw);
        createNew();
        return execute(load);
    }

    return {
        init,
        load
    };
})();
