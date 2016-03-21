namespace Glass.LeadTools.Recognition.ImageFilters
{
    using System.Windows.Media;
    using ImagingExtensions;
    using Leadtools.ImageProcessing.Color;

    internal class IncreaseContrastImageFilter : IImageFilter
    {
        public ImageSource Apply(ImageSource image)
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