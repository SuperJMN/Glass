namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media.Imaging;
    using DotImaging;
    using Imaging;
    using Leadtools.ImageProcessing.Color;

    public class AutoContrastBitmapFilter : IBitmapFilter
    {
        public IImage Apply(IImage image)
        {
            var raster = image.ToBgr().ToBitmapSource().ToRasterImage();
            new AutoColorLevelCommand { Type = AutoColorLevelCommandType.Contrast}.Run(raster);
            return raster.ToImage();
        }

        public override string ToString()
        {
            return "IncreaseContrastImageFilter";
        }
    }
}