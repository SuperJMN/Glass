namespace Glass.Imaging.FullFx
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Accord.Extensions.Imaging;
    using AForge.Imaging.Filters;
    using DotImaging;

    public static class Extensions
    {
        public static BitmapImage LoadFromPath(string path)
        {
            return CreateInMemoryBitmap(new Uri(path, UriKind.Absolute));
        }

        private static BitmapImage CreateInMemoryBitmap(Uri source)
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

        public static IImage Rotate(this IImage bitmap, double angle)
        {
            var a = bitmap.ToBgr().Lock().AsAForgeImage();
            return new RotateBicubic(angle).Apply(a).AsImage();
        }
    }
}