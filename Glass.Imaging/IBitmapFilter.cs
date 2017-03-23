namespace Glass.Imaging
{
    using System.Windows.Media.Imaging;
    using DotImaging;

    public interface IBitmapFilter
    {
        IImage Apply(IImage image);
    }
}