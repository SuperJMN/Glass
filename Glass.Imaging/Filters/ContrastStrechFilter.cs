namespace Glass.Imaging.Filters
{
    using System.Windows.Media.Imaging;
    using AForge.Imaging.Filters;
    using Core;

    public class ContrastStrechFilter : IBitmapFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            var filter = new ContrastStretch();
            var bmp = filter.Apply(image.ToBitmap());
            return bmp.ToBitmapImage();
        }
    }
}