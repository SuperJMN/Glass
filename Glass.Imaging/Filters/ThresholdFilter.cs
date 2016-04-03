namespace Glass.Imaging.Filters
{
    using System.Windows.Media.Imaging;
    using AForge.Imaging.Filters;
    using Core;

    public class ThresholdFilter : GrayscaleFilter, IBitmapFilter
    {
        private readonly int factor;

        public ThresholdFilter(int factor)
        {
            this.factor = factor;
        }

        public BitmapSource Apply(BitmapSource image)
        {
            var grayScale = ToGrayScale(image);
            var filter = new Threshold(factor);
            var bmp = filter.Apply(grayScale);
            return bmp.ToBitmapImage();
        }
    }
}
