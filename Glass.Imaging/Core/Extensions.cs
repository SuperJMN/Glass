namespace Glass.Imaging.Core
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows;
    using System.Windows.Media.Imaging;

    public static class Extensions
    {
        public static Int32Rect ToInt32Rect(this Rect rect)
        {
            return new Int32Rect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
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

        public static Bitmap ToBitmap(this BitmapSource bitmapImage)
        {
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public static BitmapSource ToBitmapImage(this Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
    }
}
