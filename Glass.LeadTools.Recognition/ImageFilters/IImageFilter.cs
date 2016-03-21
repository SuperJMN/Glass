namespace Glass.LeadTools.Recognition.ImageFilters
{
    using System.Windows.Media;

    public interface IImageFilter
    {
        ImageSource Apply(ImageSource image);
    }
}