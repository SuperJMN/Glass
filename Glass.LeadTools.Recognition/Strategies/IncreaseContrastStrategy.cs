namespace Glass.LeadTools.Recognition.Strategies
{
    using System.Windows.Media;
    using ImagingExtensions;
    using Leadtools.ImageProcessing.Color;

    internal class IncreaseContrastStrategy : IStrategy
    {
        public ImageSource Apply(ImageSource image)
        {
            var raster = image.ToRasterImage();
            new HistogramContrastCommand().Run(raster);
            return raster.ToBitmapSource();
        }
    }
}