namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using DotImaging;
    using Imaging;

    public class NoProcessBitmapFilter : IBitmapFilter
    {
        public IImage Apply(IImage image)
        {
            return image;
        }

        public override string ToString()
        {
            return "NoProcessBitmapFilter";
        }
    }
}