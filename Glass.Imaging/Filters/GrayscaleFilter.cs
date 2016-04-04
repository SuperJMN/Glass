namespace Glass.Imaging.Filters
{
    using System.Drawing;
    using System.Windows.Media.Imaging;
    using AForge.Imaging.Filters;
    using Core;

    public class GrayscaleFilter
    {
        protected Bitmap ToGrayScale(BitmapSource image)
        {
            var bitmap = image.ToBitmap();
            var grayScale = new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmap);
            return grayScale;
        }
    }
}