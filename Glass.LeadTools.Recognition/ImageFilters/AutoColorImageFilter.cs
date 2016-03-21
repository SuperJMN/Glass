namespace Glass.LeadTools.Recognition.ImageFilters
{
    using System.Windows.Media;
    using ImagingExtensions;
    using Leadtools.ImageProcessing.Color;

    internal class AutoColorImageFilter : IImageFilter
    {
        public ImageSource Apply(ImageSource image)
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