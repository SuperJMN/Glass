namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media.Imaging;
    using Leadtools.ImageProcessing.Color;

    public class AutoColorLevelFilter : IImageFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            var raster = image.ToRasterImage();
            new AutoColorLevelCommand().Run(raster);
            return raster.ToBitmapSource();
        }

        public override string ToString()
        {
            return "AutoColorLevelFilter";
        }
    }
}