namespace Glass.Imaging.Filters
{
    using System.Windows.Media.Imaging;
    using Core;
    using LeadTools.ImagingExtensions.ImageFilters;
    public class AutoContrastFilterFree : IImageFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            var filter = new AForge.Imaging.Filters.ContrastStretch();
            var bmp = filter.Apply(image.ToBitmap());
            return bmp.ToBitmapImage();
        }
    }
}