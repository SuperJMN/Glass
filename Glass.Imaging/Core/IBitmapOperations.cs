namespace Glass.Imaging.Core
{
    using System.Windows;
    using System.Windows.Media.Imaging;
    using DotImaging;

    public interface IBitmapOperations
    {
        IImage Rotate(IImage bitmap, double angle);
        IImage Crop(IImage bitmap, Rect cropBounds);
    }
}