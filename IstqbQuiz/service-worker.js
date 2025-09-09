/* Manifest version: ypDxMdRD */
// Service Worker für PWA
const cacheName = 'istqbquiz-cache-v1';
const basePath = '/IstqbQuiz/'; // GitHub Pages Repo-Name

self.addEventListener('install', event => {
    console.log('[SW] Install');
    event.waitUntil(
        caches.open(cacheName).then(cache => {
            return fetch('service-worker-assets.js')
                .then(response => response.json())
                .then(assetsManifest => {
                    const urlsToCache = assetsManifest.assets.map(asset => basePath + asset.url);
                    urlsToCache.push(basePath); // Startseite hinzufügen
                    return cache.addAll(urlsToCache);
                });
        })
    );
});

self.addEventListener('activate', event => {
    console.log('[SW] Activate');
    event.waitUntil(
        caches.keys().then(keys => {
            return Promise.all(keys.filter(k => k !== cacheName).map(k => caches.delete(k)));
        })
    );
});

self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request).then(response => {
            return response || fetch(event.request);
        })
    );
});
