namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using DotImaging;
    using Imaging;
    using Leadtools.ImageProcessing.Color;

    internal class AutoColorBitmapFilter : IBitmapFilter
    {
        public IImage Apply(IImage image)
        {
            var rasterImage = image.ToBgr().ToBitmapSource().ToRasterImage();
            new AutoColorLevelCommand().Run(rasterImage);
            return rasterImage.ToImage();
        }

        public override string ToString()
        {
            return "AutoColorBitmapFilter";
        }
    }
}