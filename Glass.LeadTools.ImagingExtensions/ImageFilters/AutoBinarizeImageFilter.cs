namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Imaging;
    using Leadtools.ImageProcessing.Core;

    internal class AutoBinarizeImageFilter : IImageFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            using (var r = image.ToRasterImage())
            {
                new AutoBinarizeCommand().Run(r);
                return r.ToBitmapSource();
            }
        }

        public override string ToString()
        {
            return "AutoBinarizeImageFilter";
        }
    }
}