namespace Glass.Imaging.Core
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public static class Extensions
    {
        public static BitmapImage LoadFromPath(string path)
        {
            return OnMemoryBitmap(new Uri(path, UriKind.Absolute));
        }

        private static BitmapImage OnMemoryBitmap(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        public static BitmapImage TryLoadFromPath(string path)
        {
            // No aseguramos de que la imagen se ha podido cargar. Esperamos si algo falla.
            var attempts = 0;
            const int loadAttempts = 15;

            BitmapImage image = null;

            do
            {
                try
                {
                    image = LoadFromPath(path);
                }
                catch (Exception)
                {
                    attempts++;

                    if (attempts == loadAttempts)
                    {
                        throw new FileNotFoundException("La imagen no se ha podido cargar.", path);
                    }

                    Thread.Sleep(200);
                }

            } while (image == null);

            return image;
        }

        public static Int32Rect ToInt32Rect(this Rect rect)
        {
            return new Int32Rect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        public static BitmapSource Rotate(this BitmapSource bitmap, double angle)
        {
            var copy = new TransformedBitmap(bitmap, new RotateTransform(angle));
            return copy;
        }

        public static BitmapSource Transform(this BitmapSource bitmap, Core.Transform transform)
        {
            var rotated = bitmap.Rotate(transform.Rotation);
            var cropInPixels = transform.Bounds.ConverToRawPixels(bitmap.DpiX, bitmap.DpiY);
            return rotated.Crop(cropInPixels);
        }

        public static BitmapSource Crop(this BitmapSource bitmap, Rect rect)
        {
            rect = rect.ConvertFrom96pppToBitmapDpi(bitmap.DpiX, bitmap.DpiY);
            var cropped = new CroppedBitmap(bitmap, rect.ToInt32Rect());
            return cropped;
        }

        public static Rect ConverToRawPixels(this Rect rect, double horzDpi, double vertDpi)
        {
            const int dpi = 96;

            var x = rect.X * horzDpi / dpi;
            var y = rect.Y * vertDpi / dpi;
            var width = rect.Width * horzDpi / dpi;
            var height = rect.Height * vertDpi / dpi;
            return new Rect(x, y, width, height);
        }

        public static Rect ConvertFrom96pppToBitmapDpi(this Rect rect, double horzDpi, double vertDpi)
        {
            const int dpi = 96;

            var x = rect.X * horzDpi / dpi;
            var y = rect.Y * vertDpi / dpi;
            var width = rect.Width * horzDpi / dpi;
            var height = rect.Height * vertDpi / dpi;
            return new Rect(x, y, width, height);
        }
    }
}
