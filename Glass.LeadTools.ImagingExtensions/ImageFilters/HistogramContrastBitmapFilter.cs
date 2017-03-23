namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using DotImaging;
    using Imaging;
    using Leadtools.ImageProcessing.Color;

    public class HistogramContrastBitmapFilter : IBitmapFilter
    {
        public IImage Apply(IImage image)
        {
            var raster = image.FromImageToRasterImage();
            new HistogramContrastCommand() { Contrast = 1000 }.Run(raster);
            return raster.ToImage();
        }

        public override string ToString()
        {
            return "IncreaseContrastImageFilter";
        }
    }
}