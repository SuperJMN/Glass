namespace Glass.Imaging.Core
{
    using System.Windows;

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
    }
}
