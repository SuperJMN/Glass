namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using DotImaging;
    using Imaging;
    using Leadtools.ImageProcessing.Core;

    internal class AutoBinarizeBitmapFilter : IBitmapFilter
    {
        public IImage Apply(IImage image)
        {
            using (var r = image.FromImageToRasterImage())
            {
                new AutoBinarizeCommand().Run(r);
                return r.ToImage();
            }
        }

        public override string ToString()
        {
            return "AutoBinarizeBitmapFilter";
        }
    }
}