/**
 * Compresses an image file before upload.
 * - Skips files already under 200KB.
 * - Resizes to maxWidth maintaining aspect ratio.
 * - Converts to JPEG at the specified quality.
 * - Fills transparent backgrounds with white (PNG → JPEG safe conversion).
 * - Falls back to the original File if canvas is unavailable or an error occurs.
 *
 * @param {File} file
 * @param {number} quality  JPEG quality 0–1 (default 0.80)
 * @param {number} maxWidth Maximum width in pixels (default 1920)
 * @returns {Promise<File>}
 */
export function compressForUpload(file, quality = 0.80, maxWidth = 1920) {
  return new Promise((resolve) => {
    if (file.size < 200_000) { resolve(file); return; }

    const canvas = document.createElement('canvas');
    if (typeof canvas.toBlob !== 'function') { resolve(file); return; }

    const img = new Image();
    const objectUrl = URL.createObjectURL(file);

    img.onload = () => {
      URL.revokeObjectURL(objectUrl);

      let { width, height } = img;
      if (width > maxWidth) {
        height = Math.round(height * maxWidth / width);
        width = maxWidth;
      }

      canvas.width = width;
      canvas.height = height;

      const ctx = canvas.getContext('2d');
      if (!ctx) { resolve(file); return; }
      ctx.fillStyle = '#ffffff';
      ctx.fillRect(0, 0, width, height);
      ctx.drawImage(img, 0, 0, width, height);

      canvas.toBlob((blob) => {
        if (!blob) { resolve(file); return; }
        resolve(new File([blob], file.name, { type: 'image/jpeg' }));
      }, 'image/jpeg', quality);
    };

    img.onerror = () => {
      URL.revokeObjectURL(objectUrl);
      resolve(file);
    };

    img.src = objectUrl;
  });
}
