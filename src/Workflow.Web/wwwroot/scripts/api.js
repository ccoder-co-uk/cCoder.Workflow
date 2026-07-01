window.workflowApi = (() => {
    const jsonHeaders = {
        "Accept": "application/json;odata=minimalmetadata",
        "Content-Type": "application/json"
    };

    async function request(method, url, body, contentType) {
        const options = {
            method,
            headers: contentType === "text/plain"
                ? { "Accept": "application/json", "Content-Type": "text/plain" }
                : jsonHeaders
        };

        if (body !== undefined) {
            options.body = contentType === "text/plain"
                ? body
                : JSON.stringify(body);
        }

        const response = await fetch(url, options);
        const text = await response.text();

        if (!response.ok) {
            throw new Error(text || `${response.status} ${response.statusText}`);
        }

        if (!text) {
            return null;
        }

        try {
            return JSON.parse(text);
        } catch {
            return text;
        }
    }

    return {
        get: url => request("GET", url),
        post: (url, body) => request("POST", url, body),
        put: (url, body) => request("PUT", url, body),
        delete: url => request("DELETE", url),
        postText: (url, body) => request("POST", url, body, "text/plain")
    };
})();
