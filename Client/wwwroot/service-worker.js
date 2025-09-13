// Name und Version des Cache
const CACHE_NAME = "istqbquiz-cache-v2";

// Wichtige Assets, die beim Installieren gecacht werden
const CORE_ASSETS = [
    "/",
    "index.html",
    "manifest.json",
    "icons/icon-192.png",
    "icons/icon-512.png",
    "css/bootstrap/bootstrap.min.css",
    "css/app.css",
    "offline.html"
];

// Installieren: Assets ins Cache legen
self.addEventListener("install", event => {
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => {
            return cache.addAll(CORE_ASSETS).catch(err => {
                console.error("Cache addAll Fehler:", err);
            });
        })
    );
});

// Aktivieren: alte Caches löschen + Client benachrichtigen
self.addEventListener("activate", event => {
    event.waitUntil(
        (async () => {
            const keys = await caches.keys();
            await Promise.all(
                keys.map(key => key !== CACHE_NAME && caches.delete(key))
            );

            // Clients über neue Version informieren
            const clientsList = await self.clients.matchAll({ type: "window" });
            for (const client of clientsList) {
                client.postMessage({ type: "NEW_VERSION" });
            }
        })()
    );
});

// Fetch-Handler
self.addEventListener("fetch", event => {
    const url = event.request.url;

    // Sonderfall: questions.json → immer frisch vom Server
    if (url.endsWith("questions.json")) {
        event.respondWith(
            fetch(event.request)
                .then(response => {
                    const clone = response.clone();
                    caches.open(CACHE_NAME).then(cache => cache.put(event.request, clone));
                    return response;
                })
                .catch(() => caches.match(event.request))
        );
        return;
    }

    // Standard: Cache first, fallback Netzwerk, offline.html wenn alles fehlschlägt
    event.respondWith(
        caches.match(event.request).then(cached => {
            return cached || fetch(event.request).catch(() => {
                if (event.request.mode === "navigate") {
                    return caches.match("offline.html");
                }
            });
        })
    );
});
