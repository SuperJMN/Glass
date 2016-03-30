namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Leadtools.ImageProcessing.Color;

    internal class AutoColorImageFilter : IImageFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            var rasterImage = image.ToRasterImage();
            new AutoColorLevelCommand().Run(rasterImage);
            return rasterImage.ToBitmapSource();
        }

        public override string ToString()
        {
            return "AutoColorImageFilter";
        }
    }
}