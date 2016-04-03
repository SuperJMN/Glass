namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Imaging;
    using Leadtools.ImageProcessing.Color;

    public class HistogramContrastBitmapFilter : IBitmapFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            var raster = image.ToRasterImage();
            new HistogramContrastCommand() { Contrast = 1000 }.Run(raster);
            return raster.ToBitmapSource();
        }

        public override string ToString()
        {
            return "IncreaseContrastImageFilter";
        }
    }
}