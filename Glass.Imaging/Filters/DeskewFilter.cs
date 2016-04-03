namespace Glass.Imaging.Filters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    using Core;

    public class DeskewFilter : IBitmapFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            var filter = new DocumentSkewChecker();
            var bitmap = image.ToBitmap();
            var grayscale = new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmap);
            var angle = filter.GetSkewAngle(grayscale);
            var rotationFilter = new RotateBilinear(-angle);
            return rotationFilter.Apply(grayscale).ToBitmapImage();
        }
    }
}