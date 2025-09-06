// Version der PWA (ändert sich bei neuen Deployments)
const CACHE_VERSION = 'v1.0.0';
const CACHE_NAME = `blazor-cache-${CACHE_VERSION}`;
const OFFLINE_URL = '/index.html';

// Standard-Blazor-Assets (werden von service-worker-assets.js bereitgestellt)
self.importScripts('./service-worker-assets.js');

const ASSETS = self.assetsManifest.assets.map(asset => new URL(asset.url, self.location).toString());

// Bei Installation: Cache vorbereiten
self.addEventListener('install', event => {
    console.log('[SW] Install');
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                console.log('[SW] Caching offline page and assets');
                return cache.addAll([OFFLINE_URL, ...ASSETS]);
            })
            .catch(err => console.error('[SW] Caching failed:', err))
    );
    self.skipWaiting(); // sofort aktivieren
});

// Aktivierung: Alte Caches entfernen
self.addEventListener('activate', event => {
    console.log('[SW] Activate');
    event.waitUntil(
        caches.keys().then(keys => {
            return Promise.all(
                keys.filter(key => key !== CACHE_NAME)
                    .map(key => caches.delete(key))
            );
        })
    );
    self.clients.claim(); // sofortige Kontrolle übernehmen
});

// Netzwerkanfragen abfangen
self.addEventListener('fetch', event => {
    if (event.request.method !== 'GET') return;

    event.respondWith(
        fetch(event.request)
            .then(response => {
                return caches.open(CACHE_NAME).then(cache => {
                    cache.put(event.request, response.clone());
                    return response;
                });
            })
            .catch(() => {
                return caches.match(event.request)
                    .then(response => response || caches.match(OFFLINE_URL));
            })
    );
});

// Automatisches Update erkennen
self.addEventListener('message', event => {
    if (event.data === 'skipWaiting') {
        console.log('[SW] Skipping waiting and activating new version...');
        self.skipWaiting();
    }
});
