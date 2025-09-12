// Name und Version des Cache
const CACHE_NAME = "istqbquiz-cache-v1";

// Wichtige Assets, die beim Installieren gecacht werden
const CORE_ASSETS = [
    "/",
    "index.html",
    "manifest.json",
    "icon-192.png",
    "icon-512.png"
];

// Beim Installieren: Assets ins Cache legen
self.addEventListener("install", event => {
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => cache.addAll(CORE_ASSETS))
    );
});

// Aktivieren: alte Caches löschen + Client über neue Version informieren
self.addEventListener("activate", event => {
    event.waitUntil(
        (async () => {
            const keys = await caches.keys();
            await Promise.all(
                keys.map(key => {
                    if (key !== CACHE_NAME) {
                        return caches.delete(key);
                    }
                })
            );

            // Clients benachrichtigen (Update verfügbar)
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

    // Sonderfall: questions.json → immer frisch vom Server holen
    if (url.endsWith("questions.json")) {
        event.respondWith(
            fetch(event.request)
                .then(response => {
                    // Optional auch ins Cache legen
                    const clone = response.clone();
                    caches.open(CACHE_NAME).then(cache => cache.put(event.request, clone));
                    return response;
                })
                .catch(() => caches.match(event.request)) // Fallback, wenn offline
        );
        return;
    }

    // Standard: Cache first, fallback Netzwerk
    event.respondWith(
        caches.match(event.request).then(cached => cached || fetch(event.request))
    );
});
/* Manifest version: WzqIvlJ2 */
