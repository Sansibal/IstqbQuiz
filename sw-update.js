window.registerForSWUpdate = (dotnetHelper) => {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.ready.then(registration => {
            registration.addEventListener("updatefound", () => {
                const newWorker = registration.installing;
                newWorker.addEventListener("statechange", () => {
                    if (newWorker.state === "installed" && navigator.serviceWorker.controller) {
                        console.log("Neue Version verfügbar.");
                        if (dotnetHelper) {
                            dotnetHelper.invokeMethodAsync("OnNewVersionAvailable");
                        }
                    }
                });
            });
        });
    }
};
