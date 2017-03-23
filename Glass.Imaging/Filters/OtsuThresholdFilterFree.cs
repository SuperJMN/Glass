namespace Glass.Imaging.Filters
{
    using System.Windows.Media.Imaging;
    using Accord.Extensions.Imaging;
    using AForge.Imaging.Filters;
    using Core;
    using DotImaging;

    public class OtsuThresholdFilterFree : GrayscaleFilter, IBitmapFilter
    {
        public IImage Apply(IImage image)
        {
            var grayScale = ToGrayScale(image);
            var filter = new OtsuThreshold();
            var bmp = filter.Apply(grayScale.ToBgr().Lock().AsAForgeImage());
            return bmp.AsImage();
        }
    }
}