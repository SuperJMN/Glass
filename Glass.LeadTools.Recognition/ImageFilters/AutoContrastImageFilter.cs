namespace Glass.LeadTools.Recognition.ImageFilters
{
    using System.Windows.Media;
    using ImagingExtensions;
    using Leadtools.ImageProcessing.Color;

    internal class AutoContrastImageFilter : IImageFilter
    {
        public ImageSource Apply(ImageSource image)
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