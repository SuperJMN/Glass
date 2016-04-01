namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public interface IImageFilter
    {
        BitmapSource Apply(BitmapSource image);
    }
}