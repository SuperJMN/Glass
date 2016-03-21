namespace Glass.LeadTools.Recognition.ImageFilters
{
    using System.Windows.Media;

    internal class NoProcessImageFilter : IImageFilter
    {
        public ImageSource Apply(ImageSource image)
        {
            return image.Clone();
        }

        public override string ToString()
        {
            return "NoProcessImageFilter";
        }
    }
}