namespace Glass.Imaging.Filters
{
    using System.Windows.Media.Imaging;
    using Accord.Extensions.Imaging;
    using AForge.Imaging.Filters;
    using Core;
    using DotImaging;

    public class ThresholdFilter : GrayscaleFilter, IBitmapFilter
    {
        private readonly int factor;

        public ThresholdFilter(int factor)
        {
            this.factor = factor;
        }

        public IImage Apply(IImage image)
        {
            var grayScale = ToGrayScale(image);
            var filter = new Threshold(factor);
            var bmp = filter.Apply(grayScale.ToGray().Lock().AsAForgeImage());
            return bmp.AsImage();
        }
    }
}
