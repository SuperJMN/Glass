namespace Glass.Imaging
{
    using System.Windows.Media.Imaging;

    public interface IBitmapFilter
    {
        BitmapSource Apply(BitmapSource image);
    }
}