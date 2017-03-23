namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media.Imaging;
    using DotImaging;
    using Imaging;
    using Leadtools.ImageProcessing.Color;

    public class AutoColorLevelFilter : IBitmapFilter
    {
        public IImage Apply(IImage image)
        {
            var raster = image.FromImageToRasterImage();
            new AutoColorLevelCommand().Run(raster);
            return raster.ToImage();
        }

        public override string ToString()
        {
            return "AutoColorLevelFilter";
        }
    }
}