window.workflowDesigner = (() => {
    const canvas = () => document.getElementById("flow-canvas");
    const definitionEditor = () => document.getElementById("flow-definition");

    function parseDefinition() {
        const value = definitionEditor().value.trim();
        return value ? JSON.parse(value) : defaultDefinition();
    }

    function defaultDefinition(name) {
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
    }

    function formatDefinition(definition) {
        definitionEditor().value = JSON.stringify(definition, null, 2);
        draw();
    }

    function draw() {
        const target = canvas();
        const context = target.getContext("2d");
        const bounds = target.getBoundingClientRect();
        target.width = Math.max(900, Math.floor(bounds.width));
        target.height = 420;
        context.clearRect(0, 0, target.width, target.height);
        drawGrid(context, target.width, target.height);

        let definition;

        try {
            definition = parseDefinition();
        } catch {
            drawMessage(context, "Definition JSON is invalid");
            return;
        }

        const activities = definition.Activities || [];
        const positions = new Map();

        activities.forEach((activity, index) => {
            const x = 36 + (index % 3) * 260;
            const y = 44 + Math.floor(index / 3) * 120;
            positions.set(activity.Ref, { x, y });
        });

        (definition.Links || []).forEach(link => drawLink(context, positions, link));
        activities.forEach(activity => drawActivity(context, positions.get(activity.Ref), activity));

        if (activities.length === 0) {
            drawMessage(context, "No activities");
        }
    }

    function drawGrid(context, width, height) {
        context.strokeStyle = "#e4ebf1";
        context.lineWidth = 1;

        for (let x = 0; x < width; x += 24) {
            context.beginPath();
            context.moveTo(x, 0);
            context.lineTo(x, height);
            context.stroke();
        }

        for (let y = 0; y < height; y += 24) {
            context.beginPath();
            context.moveTo(0, y);
            context.lineTo(width, y);
            context.stroke();
        }
    }

    function drawActivity(context, position, activity) {
        if (!position) {
            return;
        }

        const title = activity.Ref || "Activity";
        const type = activity["$type"] || "";
        context.fillStyle = "#ffffff";
        context.strokeStyle = "#117a8b";
        context.lineWidth = 2;
        context.fillRect(position.x, position.y, 210, 64);
        context.strokeRect(position.x, position.y, 210, 64);
        context.fillStyle = "#17202a";
        context.font = "600 14px Arial";
        context.fillText(title, position.x + 12, position.y + 24);
        context.fillStyle = "#5b6976";
        context.font = "12px Arial";
        context.fillText(type.split(",")[0].split(".").pop() || "Activity", position.x + 12, position.y + 46);
    }

    function drawLink(context, positions, link) {
        const source = positions.get(link.Source);
        const destination = positions.get(link.Destination);

        if (!source || !destination) {
            return;
        }

        context.strokeStyle = "#536d7a";
        context.lineWidth = 2;
        context.beginPath();
        context.moveTo(source.x + 210, source.y + 32);
        context.lineTo(destination.x, destination.y + 32);
        context.stroke();
    }

    function drawMessage(context, message) {
        context.fillStyle = "#5b6976";
        context.font = "16px Arial";
        context.fillText(message, 36, 52);
    }

    return {
        defaultDefinition,
        draw,
        formatDefinition,
        parseDefinition
    };
})();
