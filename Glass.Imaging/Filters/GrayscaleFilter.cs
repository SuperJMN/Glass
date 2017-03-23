namespace Glass.Imaging.Filters
{
    using System.Drawing;
    using System.Windows.Media.Imaging;
    using Accord.Extensions.Imaging;
    using AForge.Imaging.Filters;
    using Core;
    using DotImaging;

    public class GrayscaleFilter
    {
        protected IImage ToGrayScale(IImage image)
        {
            var bitmap = image.ToBgr().Lock().AsAForgeImage();
            var grayScale = new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmap);
            return grayScale.AsImage();
        }
    }
}