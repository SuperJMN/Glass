namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Imaging;

    public class NoProcessBitmapFilter : IBitmapFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            return (BitmapSource) image.Clone();
        }

        public override string ToString()
        {
            return "NoProcessBitmapFilter";
        }
    }
}