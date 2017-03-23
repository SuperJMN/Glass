namespace Glass.Imaging.Filters
{
    using Accord.Extensions.Imaging;
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    using Core;
    using DotImaging;

    public class DeskewFilter : IBitmapFilter
    {
        public IImage Apply(IImage image)
        {
            var filter = new DocumentSkewChecker();
            var bitmap = image.ToBgr().Lock().AsAForgeImage();
            var grayscale = new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmap);
            var angle = filter.GetSkewAngle(grayscale);
            var rotationFilter = new RotateBilinear(-angle);
            return rotationFilter.Apply(grayscale).AsImage();
        }
    }
}