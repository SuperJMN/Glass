namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Imaging;
    using Leadtools.ImageProcessing.Color;

    public class AutoContrastImageFilter : IImageFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            var raster = image.ToRasterImage();
            new AutoColorLevelCommand { Type = AutoColorLevelCommandType.Contrast}.Run(raster);
            return raster.ToBitmapSource();
        }

        public override string ToString()
        {
            return "IncreaseContrastImageFilter";
        }
    }
}