using PdfFlipBook.Utilities;
using System.Collections.Generic;

namespace PdfFlipBook.Helper.ImageCache
{
    public static class ImageCache
    {
        private static readonly Dictionary<string, DisposableImage> _imageCache = new Dictionary<string, DisposableImage>();
        private static readonly object _cacheLock = new object();

        public static DisposableImage GetOrAdd(string path)
        {
            lock (_cacheLock)
            {
                if (_imageCache.ContainsKey(path))
                    return _imageCache[path];

                var image = new DisposableImage(path);
                _imageCache[path] = image;
                return image;
            }
        }

        public static void ClearCache(bool forceClear = false)
        {
            lock (_cacheLock)
            {
                if (!forceClear && _imageCache.Count <= 100) 
                    return;

                foreach (var image in _imageCache.Values)
                {
                    image.Dispose();
                }
                _imageCache.Clear();
            }
        }

        public static int Count
        {
            get
            {
                lock (_cacheLock)
                {
                    return _imageCache.Count;
                }
            }
        }
    }


}
