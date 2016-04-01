namespace Glass.Imaging
{
    using System.Windows.Media.Imaging;

    public interface IImageFilter
    {
        BitmapSource Apply(BitmapSource image);
    }
}