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

    public class OtsuThresholdFilterFree : GrayscaleFilter, IBitmapFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            var grayScale = ToGrayScale(image);
            var filter = new OtsuThreshold();
            var bmp = filter.Apply(grayScale);
            return bmp.ToBitmapImage();
        }
    }
}