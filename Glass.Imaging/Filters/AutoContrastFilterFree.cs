namespace Glass.Imaging.Filters
{
    using System.Windows.Media.Imaging;
    using AForge.Imaging.Filters;
    using Core;

    public class AutoContrastFilterFree : IImageFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            var bitmap = image.ToBitmap();
            var grayScale = new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmap);
            var filter = new OtsuThreshold();
            var bmp = filter.Apply(grayScale);
            return bmp.ToBitmapImage();
        }
    }
}