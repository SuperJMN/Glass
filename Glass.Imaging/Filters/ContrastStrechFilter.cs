namespace Glass.Imaging.Filters
{
    using Accord.Extensions.Imaging;
    using AForge.Imaging.Filters;
    using Core;
    using DotImaging;

    public class ContrastStrechFilter : IBitmapFilter
    {
        public IImage Apply(IImage image)
        {
            var filter = new ContrastStretch();
            var bmp = filter.Apply(image.ToBgr().Lock().AsAForgeImage());
            return bmp.AsImage();
        }
    }
}