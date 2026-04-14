/* Manifest version: Geh2Xy24 */
// Production service worker — caches app shell for offline/installable PWA support.
self.importScripts('./service-worker-assets.js');

self.addEventListener('message', event => {
    if (event.data && event.data.type === 'SKIP_WAITING') self.skipWaiting();
});

const cacheName = 'hmghip-cache-v1';
const offlineAssetsInclude = [/\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.ico$/];
const offlineAssetsExclude = [/^service-worker\.js$/];

async function onInstall() {
    self.skipWaiting();
    const assetsRequests = self.assetsManifest.assets
        .filter(a => offlineAssetsInclude.some(p => p.test(a.url)))
        .filter(a => !offlineAssetsExclude.some(p => p.test(a.url)))
        .map(a => new Request(a.url, { integrity: a.hash, cache: 'no-cache' }));
    await caches.open(cacheName).then(c => c.addAll(assetsRequests));
}

async function onActivate() {
    const keys = await caches.keys();
    await Promise.all(keys.filter(k => k.startsWith('hmghip-cache-') && k !== cacheName).map(k => caches.delete(k)));
}

async function onFetch(event) {
    if (event.request.method !== 'GET') return fetch(event.request);
    const shouldServeIndex = event.request.mode === 'navigate';
    const request = shouldServeIndex ? 'index.html' : event.request;
    const cached = await caches.match(request);
    return cached || fetch(event.request);
}

self.addEventListener('install', e => e.waitUntil(onInstall()));
self.addEventListener('activate', e => e.waitUntil(onActivate()));
self.addEventListener('fetch', e => e.respondWith(onFetch(e)));
