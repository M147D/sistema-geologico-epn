// Module-level caches — survive across component mounts/unmounts
const urlCache = new Map();            // fotoId -> blobUrl (full-res)
const pendingRequests = new Map();     // fotoId -> Promise<string|null>

const thumbCache = new Map();          // fotoId -> blobUrl (thumbnail)
const pendingThumbRequests = new Map(); // fotoId -> Promise<string|null>

/**
 * Returns a blob URL for the full-resolution image.
 * Deduplicates concurrent requests for the same id.
 * @param {import('axios').AxiosInstance} api
 * @param {number|string} fotoId
 * @returns {Promise<string|null>}
 */
export function getImage(api, fotoId) {
  if (!fotoId) return Promise.resolve(null);
  if (urlCache.has(fotoId)) return Promise.resolve(urlCache.get(fotoId));
  if (pendingRequests.has(fotoId)) return pendingRequests.get(fotoId);

  const promise = api
    .get(`/foto-elementos/imagen/${fotoId}`, { responseType: 'blob' })
    .then(response => {
      const url = URL.createObjectURL(response.data);
      urlCache.set(fotoId, url);
      return url;
    })
    .catch(() => null)
    .finally(() => { pendingRequests.delete(fotoId); });

  pendingRequests.set(fotoId, promise);
  return promise;
}

/**
 * Returns a blob URL for the thumbnail (300×200) version of the image.
 * Falls back to null (not to full-res) to avoid unexpected large downloads.
 * @param {import('axios').AxiosInstance} api
 * @param {number|string} fotoId
 * @returns {Promise<string|null>}
 */
export function getImageThumbnail(api, fotoId) {
  if (!fotoId) return Promise.resolve(null);
  if (thumbCache.has(fotoId)) return Promise.resolve(thumbCache.get(fotoId));
  if (pendingThumbRequests.has(fotoId)) return pendingThumbRequests.get(fotoId);

  const promise = api
    .get(`/foto-elementos/imagen/${fotoId}?thumb=true`, { responseType: 'blob' })
    .then(response => {
      const url = URL.createObjectURL(response.data);
      thumbCache.set(fotoId, url);
      return url;
    })
    .catch(() => null)
    .finally(() => { pendingThumbRequests.delete(fotoId); });

  pendingThumbRequests.set(fotoId, promise);
  return promise;
}

/**
 * Removes a cached image from both full-res and thumbnail caches.
 * Call after uploading/updating/deleting a photo so the next render fetches fresh bytes.
 * @param {number|string} fotoId
 */
export function invalidateImage(fotoId) {
  const url = urlCache.get(fotoId);
  if (url) URL.revokeObjectURL(url);
  urlCache.delete(fotoId);
  pendingRequests.delete(fotoId);

  const thumbUrl = thumbCache.get(fotoId);
  if (thumbUrl) URL.revokeObjectURL(thumbUrl);
  thumbCache.delete(fotoId);
  pendingThumbRequests.delete(fotoId);
}

/** Releases all cached blob URLs. Call on logout to free browser memory. */
export function clearAllCache() {
  urlCache.forEach(url => URL.revokeObjectURL(url));
  urlCache.clear();
  pendingRequests.clear();

  thumbCache.forEach(url => URL.revokeObjectURL(url));
  thumbCache.clear();
  pendingThumbRequests.clear();
}
