const BASE_PATH = '/IstqbQuiz/';
const CACHE_NAME = 'istqbquiz-cache-v1';
const FALLBACK_PAGE = BASE_PATH + 'index.html';

let ASSETS = [
    FALLBACK_PAGE,
    BASE_PATH + 'css/app.css',
    BASE_PATH + '_framework/blazor.webassembly.js'
];

try {
    importScripts('service-worker-assets.js');
    if (self.assetsManifest && Array.isArray(self.assetsManifest.assets)) {
        const urls = self.assetsManifest.assets.map(a => BASE_PATH + a.url);
        ASSETS = Array.from(new Set(ASSETS.concat(urls)));
    }
} catch (e) {
    console.warn('service-worker-assets.js nicht gefunden, Fallback genutzt.');
}

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => cache.addAll(ASSETS))
    );
    self.skipWaiting();
});

self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(keys => Promise.all(
            keys.filter(k => k !== CACHE_NAME).map(k => caches.delete(k))
        ))
    );
    self.clients.claim();
});

self.addEventListener('fetch', event => {
    if (event.request.method !== 'GET') return;

    event.respondWith(
        caches.match(event.request).then(cached => {
            if (cached) return cached;
            return fetch(event.request).then(resp => {
                if (resp && resp.ok) {
                    const respClone = resp.clone();
                    caches.open(CACHE_NAME).then(cache => cache.put(event.request, respClone));
                }
                return resp;
            }).catch(() => {
                if (event.request.mode === 'navigate' || (event.request.headers.get('accept') || '').includes('text/html')) {
                    return caches.match(FALLBACK_PAGE);
                }
            });
        })
    );
});
