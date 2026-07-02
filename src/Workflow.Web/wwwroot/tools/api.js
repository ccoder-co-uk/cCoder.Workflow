window.WorkflowApi = {
    tokenKey: "workflowAuthToken",
    token: null,
    currentUser: null,

    init: async function () {
        document.getElementById("health-check").addEventListener("click", () => this.checkHealth());
        document.getElementById("auth-login").addEventListener("click", () => this.login());
        document.getElementById("auth-logout").addEventListener("click", () => this.logout());

        this.token = localStorage.getItem(this.tokenKey);
        await this.refreshAuthState();
    },

    get: function (path) {
        return this.send("GET", path);
    },

    post: function (path, data, contentType) {
        return this.send("POST", path, data, contentType);
    },

    put: function (path, data) {
        return this.send("PUT", path, data);
    },

    delete: function (path) {
        return this.send("DELETE", path);
    },

    checkHealth: async function () {
        const result = await this.get("/Health");
        this.notify(`Health: ${result ?? "OK"}`);
    },

    login: async function () {
        const user = document.getElementById("auth-username").value.trim();
        const pass = document.getElementById("auth-password").value;

        if (!user || !pass) {
            this.notify("User and password are required.", true);
            return;
        }

        const token = await this.post("/Api/Account/Login", { user, pass });
        this.token = token?.id ?? token?.Id ?? null;

        if (this.token) {
            localStorage.setItem(this.tokenKey, this.token);
        }

        document.getElementById("auth-password").value = "";
        await this.refreshAuthState();
        this.notify("Logged in");
    },

    logout: async function () {
        try {
            await this.post("/Api/Account/Logout");
        } catch {
            // The local browser token is still cleared if the server-side logout fails.
        }

        this.token = null;
        localStorage.removeItem(this.tokenKey);
        this.setAuthState(null);
        this.notify("Logged out");
    },

    refreshAuthState: async function () {
        try {
            const user = await this.get("/Api/Core/User/Me()");
            this.setAuthState(user);
        } catch {
            this.setAuthState(null);
        }
    },

    setAuthState: function (user) {
        const userId = user?.id ?? user?.Id ?? null;
        const displayName = user?.displayName ?? user?.DisplayName ?? user?.email ?? user?.Email ?? userId;
        const isAuthenticated = Boolean(this.token && userId && userId !== "Guest");

        this.currentUser = isAuthenticated ? user : null;
        document.getElementById("auth-user").textContent = isAuthenticated ? displayName : "Guest";
        document.getElementById("auth-login").hidden = isAuthenticated;
        document.getElementById("auth-logout").hidden = !isAuthenticated;
        document.getElementById("auth-username").hidden = isAuthenticated;
        document.getElementById("auth-password").hidden = isAuthenticated;
        document.body.classList.toggle("is-authenticated", isAuthenticated);
        document.dispatchEvent(new CustomEvent("workflow-auth-changed", {
            detail: { isAuthenticated }
        }));
    },

    isAuthenticated: function () {
        return Boolean(this.currentUser);
    },

    currentUserId: function () {
        return this.currentUser?.id ?? this.currentUser?.Id ?? "Guest";
    },

    send: async function (method, path, data, contentType) {
        const options = {
            method: method,
            headers: {
                "Accept": "application/json"
            }
        };

        if (data !== undefined) {
            if (contentType === "text/plain") {
                options.headers["Content-Type"] = "text/plain";
                options.body = data;
            } else {
                options.headers["Content-Type"] = "application/json";
                options.body = JSON.stringify(data);
            }
        }

        if (this.token) {
            options.headers["Authorization"] = `Bearer ${this.token}`;
        }

        const response = await fetch(path, options);
        const text = await response.text();
        const body = this.parse(text);

        if (!response.ok) {
            const message = body?.error ?? body?.message ?? body ?? `${method} ${path} returned ${response.status}`;
            this.notify(message, true);
            throw new Error(message);
        }

        return body;
    },

    parse: function (text) {
        if (!text) {
            return null;
        }

        try {
            return JSON.parse(text);
        } catch {
            return text;
        }
    },

    unwrapCollection: function (body) {
        if (Array.isArray(body)) {
            return body;
        }

        return body?.value ?? body?.Value ?? [];
    },

    notify: function (message, isError) {
        const status = document.getElementById("status-message");
        status.textContent = message;
        status.classList.toggle("wf-status-error", Boolean(isError));
    }
};

document.addEventListener("DOMContentLoaded", () => window.WorkflowApi.init());
