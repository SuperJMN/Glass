namespace Glass.Imaging.Core
{
    using System.Windows;
    using System.Windows.Media.Imaging;

    public interface IBitmapOperations
    {
        BitmapSource Rotate(BitmapSource bitmap, double angle);
        BitmapSource Crop(BitmapSource bitmap, Rect cropBounds);
    }
}