window.registerForSWUpdate = () => {
    if ("serviceWorker" in navigator) {
        navigator.serviceWorker.addEventListener("message", event => {
            if (event.data && event.data.type === "NEW_VERSION") {
                if (confirm("Es ist eine neue Version verfügbar. Jetzt aktualisieren?")) {
                    window.location.reload();
                }
            }
        });
    }
};
