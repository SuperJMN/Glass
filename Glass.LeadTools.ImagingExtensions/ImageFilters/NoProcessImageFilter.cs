namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class NoProcessImageFilter : IImageFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            return (BitmapSource) image.Clone();
        }

        public override string ToString()
        {
            return "NoProcessImageFilter";
        }
    }
}