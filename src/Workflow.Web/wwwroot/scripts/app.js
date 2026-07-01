document.addEventListener("DOMContentLoaded", async () => {
    const health = document.getElementById("health-status");

    try {
        const status = await fetch("/Health");
        health.textContent = status.ok
            ? "Service online"
            : `Service returned ${status.status}`;
    } catch {
        health.textContent = "Service unavailable";
    }

    flowManagement.init();
});
