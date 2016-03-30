namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Leadtools.ImageProcessing.Core;

    public class DeskewFilter : IImageFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            var rasterImage = image.ToRasterImage();
            new DeskewCommand { Flags = DeskewCommandFlags.RotateBicubic }.Run(rasterImage);
            return rasterImage.ToBitmapSource();
        }

        public override string ToString()
        {
            return "DeskewFilter";
        }
    }
}