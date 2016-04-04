namespace Glass.Imaging.Filters
{
    using System.Windows.Media.Imaging;
    using AForge.Imaging.Filters;
    using Core;

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