namespace Glass.LeadTools.Recognition
{
    using System.Windows.Media;
    using ImagingExtensions;
    using Leadtools.ImageProcessing.Color;

    internal class AutoColorStrategy : IStrategy
    {
        public ImageSource Apply(ImageSource image)
        {
            var rasterImage = image.ToRasterImage();
            new AutoColorLevelCommand().Run(rasterImage);
            return rasterImage.ToBitmapSource();
        }
    }
}